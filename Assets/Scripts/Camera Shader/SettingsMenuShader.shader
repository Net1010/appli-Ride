Shader "Unlit/SettingsMenuShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white"
        _BlendTex("Blend Tex", 2D) = "white"
        _Duration("Duration", Float) = 1
    }

        SubShader
    {
        Tags
        {
            "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline"
        }

        HLSLINCLUDE
        #pragma vertex vert
        #pragma fragment frag

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        struct Attributes
        {
            float4 positionOS : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct Varyings
        {
            float4 positionHCS : SV_POSITION;
            float2 uv : TEXCOORD0;
        };

        sampler2D _MainTex;
        sampler2D _BlendTex;
        float4 _MainTex_ST;
        float _TimeInit;
        float _Duration;

        float4 _ColorArray[3];
        float _DelayArray[3];
        float4 _CenterArray[3];

        Varyings vert(Attributes IN)
        {
            Varyings OUT;
            OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
            OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
            return OUT;
        }
        ENDHLSL

            Pass
        {
            Name "Settings Transition"

            HLSLPROGRAM

            float InverseLerp(float a, float b, float v) {
                return (v - a) / (b - a);
            }

            float LinearEase(float t, float duration) {
                return 3.0 * t / duration + 0;
            }

            float4 frag(Varyings IN) : SV_TARGET
            {
                float2 uv = IN.uv;
                float4 mainCamTexCol = tex2D(_MainTex, uv);
                // 
                float duration = _Duration;

                //float time = fmod(_Time.y - _TimeInit, (duration + _DelayArray[2]));                      // loops
                float time = _Time.y - _TimeInit;

                float2 uvPattern = uv - 0.5;
                uvPattern.x *= _ScreenParams.x / _ScreenParams.y;

                float4 col = mainCamTexCol;
                for (int i = 0; i < 3; i += 1) {
                    float dist = length(uvPattern - _CenterArray[i].xy);
                    float progress = LinearEase(saturate(time - _DelayArray[i]), duration);
                    float mixRatio = step(dist, progress);
                    float4 mixCol = _ColorArray[i];
                    if (i == 2) { mixCol = tex2D(_BlendTex, uv); }
                    col = lerp(col, mixCol, mixRatio);
                }
                return col;
            }
            ENDHLSL
        }
    }
}


