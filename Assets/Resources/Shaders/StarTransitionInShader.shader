Shader "Unlit/StarTransitionIn"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 1)
        _Ctrl("Ctrl", Float) = 1
        _StepCtrl("StepCtrl", Float) = 0
    }
    SubShader
    {
        Blend SrcAlpha OneMinusSrcAlpha
        Tags { "Queue" = "Transparent" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;

            float _StepCtrl;
            float _Ctrl;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float mod(float x, float y) {
                return x - y * floor(x / y);
            }

            float sdStar(float2 p, float r, int n, float m) // m=[2,n]
            {
                // these 4 lines can be precomputed for a given shape
                float an = 3.141593 / float(n);
                float en = 3.141593 / m;
                float2  acs = float2(cos(an), sin(an));
                float2  ecs = float2(cos(en), sin(en)); // ecs=vec2(0,1) and simplify, for regular polygon,
                
                // reduce to first sector
                float bn = mod(atan(p.y / p.x), 2.0 * an) - an;
                p = length(p) * float2(cos(bn), abs(sin(bn)));

                // line sdf
                p -= r * acs;
                p += ecs * clamp(-dot(p, ecs), 0.0, r * acs.y / ecs.y);
                //return bn;
                return length(p) * sign(p.x);
            }
            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                uv -= 0.5;
                uv.x *= _ScreenParams.x/ _ScreenParams.y* 0.8;
                //fixed4 col = tex2D(_MainTex, i.uv);

                float size = 3.0;
                //float duration = 1.0;
                //float time = _Time.y / duration;

                float t = 1.7;
                float n = 3.0 + floor(t);  // n, number of sides
                float a = frac(t);                 // angle factor
                float w = 2.0 + 0.7 * a * (n - 2.0);        // angle divisor, between 2 and n

                float d = sdStar(uv, size, int(n), w);
                float star = step(d, -_StepCtrl);
                return float4(_Color.xyz, star);
            }
            ENDCG
        }
    }
}
