Shader "Converted/Template"
{
    Properties
    {
        _MainTex("iChannel0", 2D) = "white" {}
        _SecondTex("iChannel1", 2D) = "white" {}
        _ThirdTex("iChannel2", 2D) = "white" {}
        _FourthTex("iChannel3", 2D) = "white" {}
        _Mouse("Mouse", Vector) = (0.5, 0.5, 0.5, 0.5)
        [ToggleUI] _GammaCorrect("Gamma Correction", Float) = 1
        _Resolution("Resolution (Change if AA is bad)", Range(1, 1024)) = 1
    }
        SubShader
        {
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                // Built-in properties
                sampler2D _MainTex;   float4 _MainTex_TexelSize;
                sampler2D _SecondTex; float4 _SecondTex_TexelSize;
                sampler2D _ThirdTex;  float4 _ThirdTex_TexelSize;
                sampler2D _FourthTex; float4 _FourthTex_TexelSize;
                float4 _Mouse;
                float _GammaCorrect;
                float _Resolution;

                // GLSL Compatability macros
                #define glsl_mod(x,y) (((x)-(y)*floor((x)/(y))))
                #define texelFetch(ch, uv, lod) tex2Dlod(ch, float4((uv).xy * ch##_TexelSize.xy + ch##_TexelSize.xy * 0.5, 0, lod))
                #define textureLod(ch, uv, lod) tex2Dlod(ch, float4(uv, 0, lod))
                #define iResolution float3(_Resolution, _Resolution, _Resolution)
                #define iFrame (floor(_Time.y / 60))
                #define iChannelTime float4(_Time.y, _Time.y, _Time.y, _Time.y)
                #define iDate float4(2020, 6, 18, 30)
                #define iSampleRate (44100)
                #define iChannelResolution float4x4(                      \
                    _MainTex_TexelSize.z,   _MainTex_TexelSize.w,   0, 0, \
                    _SecondTex_TexelSize.z, _SecondTex_TexelSize.w, 0, 0, \
                    _ThirdTex_TexelSize.z,  _ThirdTex_TexelSize.w,  0, 0, \
                    _FourthTex_TexelSize.z, _FourthTex_TexelSize.w, 0, 0)

                // Global access to uv data
                static v2f vertex_output;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                static float time;
                float roundBox(float2 coord, float2 pos, float2 b, float c)
                {
                    return 1. - floor(length(max(abs(coord - pos) - b, c)));
                }

                float circle(float2 coord, float2 pos, float size)
                {
                    return min(floor(distance(coord, pos) - size), 0.);
                }

                float sdCapsule(float2 p, float2 a, float2 b, float r)
                {
                    float2 pa = p - a, ba = b - a;
                    float h = clamp(dot(pa, ba) / dot(ba, ba), 0., 1.);
                    return min(floor(length(pa - ba * h) - r), 0.);
                }

                float circle(float2 coord, float2 pos, float size, float start, float end)
                {
                    float angle = atan2(coord.x, coord.y);
                    if (angle > start && angle < end)
                        return 0.;

                    return min(floor(distance(coord, pos) - size), 0.);
                }

                float repeat_circle(float2 coord, float2 pos, float size, float slice)
                {
                    float angle = atan2(coord.x, coord.y);
                    if (glsl_mod(angle * slice, 3.141 * 2.) < 3.141)
                        return 0.;

                    return min(floor(distance(coord, pos) - size), 0.);
                }

                float2x2 rotate(float Angle)
                {
                    float2x2 rotation = transpose(float2x2(float2(cos(Angle), sin(Angle)), float2(-sin(Angle), cos(Angle))));
                    return rotation;
                }

                float rand(float2 co)
                {
                    return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.547);
                }

                float triangle_(float2 coord, float2 pos, float angle, float thick, float size)
                {
                    float2 original_coord = coord;
                    coord += pos;
                    coord = mul(coord,rotate(angle));
                    float collision = 0.;
                    collision += sdCapsule(coord, float2(0., 0.333) * size, float2(0.3, -0.2) * size, thick);
                    collision += sdCapsule(coord, float2(0., 0.333) * size, float2(-0.3, -0.2) * size, thick);
                    collision += sdCapsule(coord, float2(0.3, -0.2) * size, float2(-0.3, -0.2) * size, thick);
                    collision += 0.5 * tex2D(_MainTex, original_coord * 0.6 + ((float2)time * 0.02)).b * 0.9;
                    collision += 0.5 * tex2D(_MainTex, original_coord * 0.1 + ((float2)time * 0.001)).b * 0.9;
                    return -min(max(-collision, 0.), 1.);
                }

                float3 bubble_effect()
                {
                    float2 p = (vertex_output.uv * _Resolution).xy / iResolution.xy;
                    float aspectCorrection = iResolution.x / iResolution.y;
                    float2 coordinate_entered = 2. * p - 1.;
                    float2 coord = float2(aspectCorrection, 1.) * coordinate_entered;
                    float2 s = coord;
                    float bubble_time = time - 4.;
                    coord = mul(coord,rotate(-bubble_time * 0.163));
                    float3 COLOR = float3(0.9, 0.4, 0.5) + float3(0.2, 0.5, 0.2);
                    if (glsl_mod(coord.y * 200. + coord.x * 200., 8.) < 1.1)
                    {
                        COLOR -= ((float3)0.05);
                    }
                    else if (glsl_mod(coord.y * 200. - coord.x * 200., 8.) < 1.1)
                    {
                        COLOR -= ((float3)0.05);
                    }

                    coord /= 0.1 * bubble_time * bubble_time;
                    for (float i = 0.; i < 22.; i++)
                    {
                        float final_I = i + max(bubble_time / 32., 0.);
                        float Tfast = max(bubble_time * 4. - final_I, 0.) * 10.;
                        float bouncing = 0.5 - 0.5 * cos(Tfast * 0.2) / (1. + Tfast * Tfast * 0.001);
                        float2 boxpos = tex2D(_SecondTex, float2(final_I / 20., sin(final_I * 0.01))).xy - ((float2)0.5);
                        if (circle(coord, boxpos * (1. + final_I * 0.5), bouncing * 0.5 * (1. + final_I * 0.1)) < 0.)
                            COLOR += 0.1 * float3(1.5, 0.5 + 0.41 * sin(final_I), 1. + 0.5 * sin(final_I));

                    }
                    s *= 0.2 * max(time - 6., 0.) * 2.;
                    float2 offset = float2(-0.025, 0.025);
                    if (triangle_(s, offset + ((float2)0.), _Time.y, 0.1, 2.) < 0. && glsl_mod((s.x + s.y) * 11., 3.) < 2.)
                        COLOR.rgb = float3(1.1, 0.6, 0.4);

                    if (triangle_(s, ((float2)0.), _Time.y, 0.1, 2.) < 0. && glsl_mod((s.x + s.y + offset.x) * 11., 3.) < 2.)
                        COLOR.rgb = ((float3)0.2);

                    return COLOR;
                }

                float4 frag(v2f __vertex_output) : SV_Target
                {
                    vertex_output = __vertex_output;
                    float4 fragColor = 0;
                    float2 fragCoord = vertex_output.uv * _Resolution;
                    float2 p = fragCoord.xy / iResolution.xy;
                    float aspectCorrection = iResolution.x / iResolution.y;
                    float2 coordinate_entered = 2. * p - 1.;
                    float2 coord = float2(aspectCorrection, 1.) * coordinate_entered;
                    float2 s = coord;
                    float vignette = 1. / max(0.25 + 0.3 * dot(coord, coord), 1.);
                    time = glsl_mod(_Time.y, 12.);
                    coord /= time * 0.4;
                    float3 COLOR = float3(0.2, 0.06, 0.55) + float3(0.2, 0.5, 0.2);
                    if (glsl_mod(s.y * 200. + s.x * 200., 8.) < 1.1)
                    {
                        COLOR -= ((float3)0.05);
                    }
                    else if (glsl_mod(s.y * 200. - s.x * 200., 8.) < 1.1)
                    {
                        COLOR -= ((float3)0.05);
                    }

                    coord = mul(coord,rotate(3.4 + time));
                    float collision = 0.;
                    collision += 4. * circle(coord, ((float2)0.), 1.29);
                    collision -= 4. * circle(coord, ((float2)0.), 0.9);
                    collision += 4. * circle(coord, ((float2)0.), 0.45);
                    if (collision < 0. && glsl_mod((p.x - p.y) * 40., 3.) < 1.8)
                    {
                        COLOR = float3(0.16, 0.06, 0.3);
                    }

                    collision += 4. * circle(coord, ((float2)0.), 0.68);
                    collision += 4. * repeat_circle(coord, ((float2)0.), 0.77, 10.);
                    collision -= 8. * circle(coord, ((float2)0.), 0.68);
                    collision -= 2. * repeat_circle(coord, ((float2)0.), 1.7, 10.);
                    collision += 2. * repeat_circle(coord, ((float2)0.), 1.9, 5.);
                    if (collision < 0.)
                    {
                        COLOR = (((float3)1.) - float3(0.2, 0.8, 0.9) / 6.) / 6.;
                    }

                    collision = 0.;
                    collision += 4. * circle(coord, ((float2)0.), 0.58 + time / 7.);
                    collision += 4. * repeat_circle(coord, ((float2)0.), 0.61 + time / 12., 10.);
                    collision -= 8. * circle(coord, ((float2)0.), 0.48 + time / 7.);
                    collision -= 2. * repeat_circle(coord, ((float2)0.), 1.5, 10.);
                    collision += 2. * repeat_circle(coord, ((float2)0.), 1.6, 5.);
                    if (collision < 0.)
                    {
                        COLOR += float3(0.9, 0.2, 0.4) * floor(glsl_mod((coord.x + coord.y) * 10., 2.));
                    }

                    collision = 0.;
                    for (float i = 0.; i < 15.; i++)
                    {
                        float2 triangle_position = 2. * time * float2(-0.5 + rand(((float2)i)), -0.5 + rand(((float2)i * i)));
                        float3 c = ((float3)0.);
                        COLOR += 0.2 * triangle_(coord, triangle_position, time + i, 0.05 + 0.02 * sin(time + i), sin(i + time) + 1.);
                        COLOR -= 1.33 * triangle_(coord, triangle_position, time + i, 0.02 + 0.02 * sin(time + i), sin(i + time) + 1.);
                    }
                    if (collision < 0.)
                    {
                        COLOR = ((float3)1.);
                    }

                    COLOR += 1. - min(time, 1.);
                    COLOR = COLOR * (1. - min(max(time - 5., 0.), 1.)) + bubble_effect() * min(max(time - 5., 0.), 1.);
                    COLOR += ((float3)max(min(time - 10., 1.), 0.));
                    fragColor = float4(COLOR * vignette, 1.);
                    if (_GammaCorrect) fragColor.rgb = pow(fragColor.rgb, 2.2);
                    return fragColor;
                }
                ENDCG
            }
        }
}
