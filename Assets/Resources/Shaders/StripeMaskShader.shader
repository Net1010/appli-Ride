Shader "StripeMaskShader"
{
    Properties
    {
        _Color("Color", Color) = (0.7372549, 0.5411765, 0.5411765, 0)
        _StripeAmt("StripeAmt", Int) = 8
        _MoveAmount("MoveAmount", Float) = 1
        _UOff("UOff", Float) = 0.2
        _VOff("VOff", Float) = 0.2
        _RotateAmt("RotateAmt", Range(0, 360)) = 0
        [NoScaleOffset]_TextureMask("TextureMask", 2D) = "white" {}
        [ToggleUI]_UseTexMask("UseTexMask", Float) = 0
        _Width("Width", Float) = 1
        _Height("Height", Float) = 1
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
        [NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
    }
        SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue" = "Transparent"
        }
        // mask
        Stencil
        {
            Ref 2
            Comp always
            Pass replace
        }
        Pass
        {
            Name "Sprite Unlit"
            Tags
            {
                "LightMode" = "Universal2D"
            }

        // Render State
        Cull Off
    Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
    ZTest LEqual
    ZWrite Off

        // Debug
        // <None>

        // --------------------------------------------------
        // Pass

        HLSLPROGRAM

        // Pragmas
        #pragma target 2.0
    #pragma exclude_renderers d3d11_9x
    #pragma vertex vert
    #pragma fragment frag

        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>

        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>

        // Defines
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITEUNLIT
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

        // --------------------------------------------------
        // Structs and Packing

        struct Attributes
    {
        float3 positionOS : POSITION;
        float3 normalOS : NORMAL;
        float4 tangentOS : TANGENT;
        float4 uv0 : TEXCOORD0;
        float4 color : COLOR;
        #if UNITY_ANY_INSTANCING_ENABLED
        uint instanceID : INSTANCEID_SEMANTIC;
        #endif
    };
    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float4 texCoord0;
        float4 color;
        #if UNITY_ANY_INSTANCING_ENABLED
        uint instanceID : CUSTOM_INSTANCE_ID;
        #endif
        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
        uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
        #endif
        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
        uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
        #endif
    };
    struct SurfaceDescriptionInputs
    {
        float4 uv0;
        float3 TimeParameters;
    };
    struct VertexDescriptionInputs
    {
        float3 ObjectSpaceNormal;
        float3 ObjectSpaceTangent;
        float3 ObjectSpacePosition;
    };
    struct PackedVaryings
    {
        float4 positionCS : SV_POSITION;
        float4 interp0 : TEXCOORD0;
        float4 interp1 : TEXCOORD1;
        #if UNITY_ANY_INSTANCING_ENABLED
        uint instanceID : CUSTOM_INSTANCE_ID;
        #endif
        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
        uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
        #endif
        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
        uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
        #endif
    };

        PackedVaryings PackVaryings(Varyings input)
    {
        PackedVaryings output;
        output.positionCS = input.positionCS;
        output.interp0.xyzw = input.texCoord0;
        output.interp1.xyzw = input.color;
        #if UNITY_ANY_INSTANCING_ENABLED
        output.instanceID = input.instanceID;
        #endif
        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
        output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
        #endif
        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
        output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        output.cullFace = input.cullFace;
        #endif
        return output;
    }
    Varyings UnpackVaryings(PackedVaryings input)
    {
        Varyings output;
        output.positionCS = input.positionCS;
        output.texCoord0 = input.interp0.xyzw;
        output.color = input.interp1.xyzw;
        #if UNITY_ANY_INSTANCING_ENABLED
        output.instanceID = input.instanceID;
        #endif
        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
        output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
        #endif
        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
        output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        output.cullFace = input.cullFace;
        #endif
        return output;
    }

    // --------------------------------------------------
    // Graph

    // Graph Properties
    CBUFFER_START(UnityPerMaterial)
float4 _Color;
float _StripeAmt;
float _MoveAmount;
float _UOff;
float _VOff;
float _RotateAmt;
float4 _TextureMask_TexelSize;
float _UseTexMask;
float _Width;
float _Height;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(_TextureMask);
SAMPLER(sampler_TextureMask);

// Graph Functions

void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
{
    RGBA = float4(R, G, B, A);
    RGB = float3(R, G, B);
    RG = float2(R, G);
}

void BooltoFloat_float(float Value, out float Out) {
    Out = Value * 1;
}

void Unity_Lerp_float3(float3 A, float3 B, float3 T, out float3 Out)
{
    Out = lerp(A, B, T);
}

void Unity_Maximum_float(float A, float B, out float Out)
{
    Out = max(A, B);
}

void Unity_Divide_float(float A, float B, out float Out)
{
    Out = A / B;
}

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
}

void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
{
    Out = UV * Tiling + Offset;
}

void Unity_DegreesToRadians_float(float In, out float Out)
{
    Out = radians(In);
}

void Unity_Rotate_Radians_float(float2 UV, float2 Center, float Rotation, out float2 Out)
{
    //rotation matrix
    UV -= Center;
    float s = sin(Rotation);
    float c = cos(Rotation);

    //center rotation matrix
    float2x2 rMatrix = float2x2(c, -s, s, c);
    rMatrix *= 0.5;
    rMatrix += 0.5;
    rMatrix = rMatrix * 2 - 1;

    //multiply the UVs by the rotation matrix
    UV.xy = mul(UV.xy, rMatrix);
    UV += Center;

    Out = UV;
}

void Unity_Fraction_float(float In, out float Out)
{
    Out = frac(In);
}

void Unity_Step_float(float Edge, float In, out float Out)
{
    Out = step(Edge, In);
}

void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
{
    Out = A * B;
}

// Graph Vertex
struct VertexDescription
{
    float3 Position;
    float3 Normal;
    float3 Tangent;
};

VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
{
    VertexDescription description = (VertexDescription)0;
    description.Position = IN.ObjectSpacePosition;
    description.Normal = IN.ObjectSpaceNormal;
    description.Tangent = IN.ObjectSpaceTangent;
    return description;
}

// Graph Pixel
struct SurfaceDescription
{
    float3 BaseColor;
    float Alpha;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
    SurfaceDescription surface = (SurfaceDescription)0;
    float4 _Property_d757e450105546818a5148247ee91cdb_Out_0 = _Color;
    UnityTexture2D _Property_1214cc2b1e3c4e62b248816675e7a5f3_Out_0 = UnityBuildTexture2DStructNoScale(_TextureMask);
    float4 _SampleTexture2D_edbfe6c53d7946cabac0cc1b359cafbe_RGBA_0 = SAMPLE_TEXTURE2D(_Property_1214cc2b1e3c4e62b248816675e7a5f3_Out_0.tex, _Property_1214cc2b1e3c4e62b248816675e7a5f3_Out_0.samplerstate, IN.uv0.xy);
    float _SampleTexture2D_edbfe6c53d7946cabac0cc1b359cafbe_R_4 = _SampleTexture2D_edbfe6c53d7946cabac0cc1b359cafbe_RGBA_0.r;
    float _SampleTexture2D_edbfe6c53d7946cabac0cc1b359cafbe_G_5 = _SampleTexture2D_edbfe6c53d7946cabac0cc1b359cafbe_RGBA_0.g;
    float _SampleTexture2D_edbfe6c53d7946cabac0cc1b359cafbe_B_6 = _SampleTexture2D_edbfe6c53d7946cabac0cc1b359cafbe_RGBA_0.b;
    float _SampleTexture2D_edbfe6c53d7946cabac0cc1b359cafbe_A_7 = _SampleTexture2D_edbfe6c53d7946cabac0cc1b359cafbe_RGBA_0.a;
    float4 _Combine_dc8d5bf5d82c429fb73e3254a9d6e02a_RGBA_4;
    float3 _Combine_dc8d5bf5d82c429fb73e3254a9d6e02a_RGB_5;
    float2 _Combine_dc8d5bf5d82c429fb73e3254a9d6e02a_RG_6;
    Unity_Combine_float(_SampleTexture2D_edbfe6c53d7946cabac0cc1b359cafbe_R_4, _SampleTexture2D_edbfe6c53d7946cabac0cc1b359cafbe_G_5, _SampleTexture2D_edbfe6c53d7946cabac0cc1b359cafbe_B_6, _SampleTexture2D_edbfe6c53d7946cabac0cc1b359cafbe_A_7, _Combine_dc8d5bf5d82c429fb73e3254a9d6e02a_RGBA_4, _Combine_dc8d5bf5d82c429fb73e3254a9d6e02a_RGB_5, _Combine_dc8d5bf5d82c429fb73e3254a9d6e02a_RG_6);
    float _Property_0c7daecd7b8e44769f06c24c72a758b1_Out_0 = _UseTexMask;
    float _BooltoFloatCustomFunction_e039a8f7d149414fa1287085fc8c5d24_Out_1;
    BooltoFloat_float(_Property_0c7daecd7b8e44769f06c24c72a758b1_Out_0, _BooltoFloatCustomFunction_e039a8f7d149414fa1287085fc8c5d24_Out_1);
    float3 _Lerp_754ba431ac8846668079237ba8a516ae_Out_3;
    Unity_Lerp_float3(float3(255, 255, 255), _Combine_dc8d5bf5d82c429fb73e3254a9d6e02a_RGB_5, (_BooltoFloatCustomFunction_e039a8f7d149414fa1287085fc8c5d24_Out_1.xxx), _Lerp_754ba431ac8846668079237ba8a516ae_Out_3);
    float _Property_4dacc95c1e2843efbfc93fe5146b5cce_Out_0 = _Width;
    float _Maximum_d0fbce08912e45999c6d08e73790e64e_Out_2;
    Unity_Maximum_float(_Property_4dacc95c1e2843efbfc93fe5146b5cce_Out_0, 1, _Maximum_d0fbce08912e45999c6d08e73790e64e_Out_2);
    float _Property_37a7f5c6a1da454494520fdea10e9c7c_Out_0 = _Height;
    float _Maximum_b80044435c284985865e57503b47c7ea_Out_2;
    Unity_Maximum_float(_Property_37a7f5c6a1da454494520fdea10e9c7c_Out_0, 1, _Maximum_b80044435c284985865e57503b47c7ea_Out_2);
    float _Divide_7bce1d28091e47b09166b552637a3406_Out_2;
    Unity_Divide_float(_Maximum_d0fbce08912e45999c6d08e73790e64e_Out_2, _Maximum_b80044435c284985865e57503b47c7ea_Out_2, _Divide_7bce1d28091e47b09166b552637a3406_Out_2);
    float4 _UV_59ac78cf5496476798aee234cd17e98c_Out_0 = IN.uv0;
    float _Split_05ad5f4309b44c40a6cd0e4c68ac91f2_R_1 = _UV_59ac78cf5496476798aee234cd17e98c_Out_0[0];
    float _Split_05ad5f4309b44c40a6cd0e4c68ac91f2_G_2 = _UV_59ac78cf5496476798aee234cd17e98c_Out_0[1];
    float _Split_05ad5f4309b44c40a6cd0e4c68ac91f2_B_3 = _UV_59ac78cf5496476798aee234cd17e98c_Out_0[2];
    float _Split_05ad5f4309b44c40a6cd0e4c68ac91f2_A_4 = _UV_59ac78cf5496476798aee234cd17e98c_Out_0[3];
    float _Multiply_4c832bfe468840cab2be70117e0e228b_Out_2;
    Unity_Multiply_float(_Divide_7bce1d28091e47b09166b552637a3406_Out_2, _Split_05ad5f4309b44c40a6cd0e4c68ac91f2_R_1, _Multiply_4c832bfe468840cab2be70117e0e228b_Out_2);
    float4 _Combine_3b72a999d82545039e70a1f96e524bc4_RGBA_4;
    float3 _Combine_3b72a999d82545039e70a1f96e524bc4_RGB_5;
    float2 _Combine_3b72a999d82545039e70a1f96e524bc4_RG_6;
    Unity_Combine_float(_Multiply_4c832bfe468840cab2be70117e0e228b_Out_2, _Split_05ad5f4309b44c40a6cd0e4c68ac91f2_G_2, _Split_05ad5f4309b44c40a6cd0e4c68ac91f2_B_3, _Split_05ad5f4309b44c40a6cd0e4c68ac91f2_A_4, _Combine_3b72a999d82545039e70a1f96e524bc4_RGBA_4, _Combine_3b72a999d82545039e70a1f96e524bc4_RGB_5, _Combine_3b72a999d82545039e70a1f96e524bc4_RG_6);
    float _Property_d58e401c1bb448b69d85e5acac57e7c2_Out_0 = _UOff;
    float _Property_2574dbedc870463782e9fbddb89ae884_Out_0 = _MoveAmount;
    float _Multiply_799a1e021b564f7bb08b3c9f36d21eec_Out_2;
    Unity_Multiply_float(_Property_2574dbedc870463782e9fbddb89ae884_Out_0, IN.TimeParameters.x, _Multiply_799a1e021b564f7bb08b3c9f36d21eec_Out_2);
    float _Multiply_7aa76ce4f75b44d3ad99ea0998fd083f_Out_2;
    Unity_Multiply_float(_Property_d58e401c1bb448b69d85e5acac57e7c2_Out_0, _Multiply_799a1e021b564f7bb08b3c9f36d21eec_Out_2, _Multiply_7aa76ce4f75b44d3ad99ea0998fd083f_Out_2);
    float _Property_cfbfdef9bdfa47f49c6ad70253ca43f5_Out_0 = _VOff;
    float _Multiply_116203c5c8044dd1ad6f3b6c15feb5b6_Out_2;
    Unity_Multiply_float(_Multiply_799a1e021b564f7bb08b3c9f36d21eec_Out_2, _Property_cfbfdef9bdfa47f49c6ad70253ca43f5_Out_0, _Multiply_116203c5c8044dd1ad6f3b6c15feb5b6_Out_2);
    float4 _Combine_fbfec90524e34f10a355676b67d4f2ea_RGBA_4;
    float3 _Combine_fbfec90524e34f10a355676b67d4f2ea_RGB_5;
    float2 _Combine_fbfec90524e34f10a355676b67d4f2ea_RG_6;
    Unity_Combine_float(_Multiply_7aa76ce4f75b44d3ad99ea0998fd083f_Out_2, _Multiply_116203c5c8044dd1ad6f3b6c15feb5b6_Out_2, 0, 0, _Combine_fbfec90524e34f10a355676b67d4f2ea_RGBA_4, _Combine_fbfec90524e34f10a355676b67d4f2ea_RGB_5, _Combine_fbfec90524e34f10a355676b67d4f2ea_RG_6);
    float2 _TilingAndOffset_6f545536a6f64b019c74827b675bca22_Out_3;
    Unity_TilingAndOffset_float(_Combine_3b72a999d82545039e70a1f96e524bc4_RG_6, float2 (1, 1), _Combine_fbfec90524e34f10a355676b67d4f2ea_RG_6, _TilingAndOffset_6f545536a6f64b019c74827b675bca22_Out_3);
    float _Property_a69362d9d74c485295c51e4785006053_Out_0 = _RotateAmt;
    float _DegreesToRadians_e039c0ee46b7448e9d5096b24bbc4663_Out_1;
    Unity_DegreesToRadians_float(_Property_a69362d9d74c485295c51e4785006053_Out_0, _DegreesToRadians_e039c0ee46b7448e9d5096b24bbc4663_Out_1);
    float2 _Rotate_88ddec717a72424b8d38fec0a89e334e_Out_3;
    Unity_Rotate_Radians_float(_TilingAndOffset_6f545536a6f64b019c74827b675bca22_Out_3, float2 (0.5, 0.5), _DegreesToRadians_e039c0ee46b7448e9d5096b24bbc4663_Out_1, _Rotate_88ddec717a72424b8d38fec0a89e334e_Out_3);
    float _Split_8203d365c8204d36b6bf30e49b2f83bf_R_1 = _Rotate_88ddec717a72424b8d38fec0a89e334e_Out_3[0];
    float _Split_8203d365c8204d36b6bf30e49b2f83bf_G_2 = _Rotate_88ddec717a72424b8d38fec0a89e334e_Out_3[1];
    float _Split_8203d365c8204d36b6bf30e49b2f83bf_B_3 = 0;
    float _Split_8203d365c8204d36b6bf30e49b2f83bf_A_4 = 0;
    float _Property_258e62b5205d4bd282d9d3e1ea81f47f_Out_0 = _StripeAmt;
    float _Multiply_994e26f0510d4ca3b012a73b47a21296_Out_2;
    Unity_Multiply_float(_Split_8203d365c8204d36b6bf30e49b2f83bf_R_1, _Property_258e62b5205d4bd282d9d3e1ea81f47f_Out_0, _Multiply_994e26f0510d4ca3b012a73b47a21296_Out_2);
    float _Fraction_48817f6e21c44f37856f4691b2cbebfc_Out_1;
    Unity_Fraction_float(_Multiply_994e26f0510d4ca3b012a73b47a21296_Out_2, _Fraction_48817f6e21c44f37856f4691b2cbebfc_Out_1);
    float _Step_990610fddfcd480fa5f43bd006876c39_Out_2;
    Unity_Step_float(_Fraction_48817f6e21c44f37856f4691b2cbebfc_Out_1, 0.5, _Step_990610fddfcd480fa5f43bd006876c39_Out_2);
    float3 _Multiply_1dda426f42d7470e9607fe2de85facbc_Out_2;
    Unity_Multiply_float(_Lerp_754ba431ac8846668079237ba8a516ae_Out_3, (_Step_990610fddfcd480fa5f43bd006876c39_Out_2.xxx), _Multiply_1dda426f42d7470e9607fe2de85facbc_Out_2);
    surface.BaseColor = (_Property_d757e450105546818a5148247ee91cdb_Out_0.xyz);
    surface.Alpha = (_Multiply_1dda426f42d7470e9607fe2de85facbc_Out_2).x;
    return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
    VertexDescriptionInputs output;
    ZERO_INITIALIZE(VertexDescriptionInputs, output);

    output.ObjectSpaceNormal = input.normalOS;
    output.ObjectSpaceTangent = input.tangentOS.xyz;
    output.ObjectSpacePosition = input.positionOS;

    return output;
}
    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





    output.uv0 = input.texCoord0;
    output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
#else
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
#endif
#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

    return output;
}

    // --------------------------------------------------
    // Main

    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SpriteUnlitPass.hlsl"

    ENDHLSL
}
Pass
{
    Name "Sprite Unlit"
    Tags
    {
        "LightMode" = "UniversalForward"
    }

        // Render State
        Cull Off
    Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
    ZTest LEqual
    ZWrite Off

        // Debug
        // <None>

        // --------------------------------------------------
        // Pass

        HLSLPROGRAM

        // Pragmas
        #pragma target 2.0
    #pragma exclude_renderers d3d11_9x
    #pragma vertex vert
    #pragma fragment frag

        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>

        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>

        // Defines
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITEFORWARD
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

        // --------------------------------------------------
        // Structs and Packing

        struct Attributes
    {
        float3 positionOS : POSITION;
        float3 normalOS : NORMAL;
        float4 tangentOS : TANGENT;
        float4 uv0 : TEXCOORD0;
        float4 color : COLOR;
        #if UNITY_ANY_INSTANCING_ENABLED
        uint instanceID : INSTANCEID_SEMANTIC;
        #endif
    };
    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float4 texCoord0;
        float4 color;
        #if UNITY_ANY_INSTANCING_ENABLED
        uint instanceID : CUSTOM_INSTANCE_ID;
        #endif
        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
        uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
        #endif
        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
        uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
        #endif
    };
    struct SurfaceDescriptionInputs
    {
        float4 uv0;
        float3 TimeParameters;
    };
    struct VertexDescriptionInputs
    {
        float3 ObjectSpaceNormal;
        float3 ObjectSpaceTangent;
        float3 ObjectSpacePosition;
    };
    struct PackedVaryings
    {
        float4 positionCS : SV_POSITION;
        float4 interp0 : TEXCOORD0;
        float4 interp1 : TEXCOORD1;
        #if UNITY_ANY_INSTANCING_ENABLED
        uint instanceID : CUSTOM_INSTANCE_ID;
        #endif
        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
        uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
        #endif
        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
        uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
        #endif
    };

        PackedVaryings PackVaryings(Varyings input)
    {
        PackedVaryings output;
        output.positionCS = input.positionCS;
        output.interp0.xyzw = input.texCoord0;
        output.interp1.xyzw = input.color;
        #if UNITY_ANY_INSTANCING_ENABLED
        output.instanceID = input.instanceID;
        #endif
        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
        output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
        #endif
        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
        output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        output.cullFace = input.cullFace;
        #endif
        return output;
    }
    Varyings UnpackVaryings(PackedVaryings input)
    {
        Varyings output;
        output.positionCS = input.positionCS;
        output.texCoord0 = input.interp0.xyzw;
        output.color = input.interp1.xyzw;
        #if UNITY_ANY_INSTANCING_ENABLED
        output.instanceID = input.instanceID;
        #endif
        #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
        output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
        #endif
        #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
        output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        output.cullFace = input.cullFace;
        #endif
        return output;
    }

    // --------------------------------------------------
    // Graph

    // Graph Properties
    CBUFFER_START(UnityPerMaterial)
float4 _Color;
float _StripeAmt;
float _MoveAmount;
float _UOff;
float _VOff;
float _RotateAmt;
float4 _TextureMask_TexelSize;
float _UseTexMask;
float _Width;
float _Height;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(_TextureMask);
SAMPLER(sampler_TextureMask);

// Graph Functions

void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
{
    RGBA = float4(R, G, B, A);
    RGB = float3(R, G, B);
    RG = float2(R, G);
}

void BooltoFloat_float(float Value, out float Out) {
    Out = Value * 1;
}

void Unity_Lerp_float3(float3 A, float3 B, float3 T, out float3 Out)
{
    Out = lerp(A, B, T);
}

void Unity_Maximum_float(float A, float B, out float Out)
{
    Out = max(A, B);
}

void Unity_Divide_float(float A, float B, out float Out)
{
    Out = A / B;
}

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
}

void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
{
    Out = UV * Tiling + Offset;
}

void Unity_DegreesToRadians_float(float In, out float Out)
{
    Out = radians(In);
}

void Unity_Rotate_Radians_float(float2 UV, float2 Center, float Rotation, out float2 Out)
{
    //rotation matrix
    UV -= Center;
    float s = sin(Rotation);
    float c = cos(Rotation);

    //center rotation matrix
    float2x2 rMatrix = float2x2(c, -s, s, c);
    rMatrix *= 0.5;
    rMatrix += 0.5;
    rMatrix = rMatrix * 2 - 1;

    //multiply the UVs by the rotation matrix
    UV.xy = mul(UV.xy, rMatrix);
    UV += Center;

    Out = UV;
}

void Unity_Fraction_float(float In, out float Out)
{
    Out = frac(In);
}

void Unity_Step_float(float Edge, float In, out float Out)
{
    Out = step(Edge, In);
}

void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
{
    Out = A * B;
}

// Graph Vertex
struct VertexDescription
{
    float3 Position;
    float3 Normal;
    float3 Tangent;
};

VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
{
    VertexDescription description = (VertexDescription)0;
    description.Position = IN.ObjectSpacePosition;
    description.Normal = IN.ObjectSpaceNormal;
    description.Tangent = IN.ObjectSpaceTangent;
    return description;
}

// Graph Pixel
struct SurfaceDescription
{
    float3 BaseColor;
    float Alpha;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
    SurfaceDescription surface = (SurfaceDescription)0;
    float4 _Property_d757e450105546818a5148247ee91cdb_Out_0 = _Color;
    UnityTexture2D _Property_1214cc2b1e3c4e62b248816675e7a5f3_Out_0 = UnityBuildTexture2DStructNoScale(_TextureMask);
    float4 _SampleTexture2D_edbfe6c53d7946cabac0cc1b359cafbe_RGBA_0 = SAMPLE_TEXTURE2D(_Property_1214cc2b1e3c4e62b248816675e7a5f3_Out_0.tex, _Property_1214cc2b1e3c4e62b248816675e7a5f3_Out_0.samplerstate, IN.uv0.xy);
    float _SampleTexture2D_edbfe6c53d7946cabac0cc1b359cafbe_R_4 = _SampleTexture2D_edbfe6c53d7946cabac0cc1b359cafbe_RGBA_0.r;
    float _SampleTexture2D_edbfe6c53d7946cabac0cc1b359cafbe_G_5 = _SampleTexture2D_edbfe6c53d7946cabac0cc1b359cafbe_RGBA_0.g;
    float _SampleTexture2D_edbfe6c53d7946cabac0cc1b359cafbe_B_6 = _SampleTexture2D_edbfe6c53d7946cabac0cc1b359cafbe_RGBA_0.b;
    float _SampleTexture2D_edbfe6c53d7946cabac0cc1b359cafbe_A_7 = _SampleTexture2D_edbfe6c53d7946cabac0cc1b359cafbe_RGBA_0.a;
    float4 _Combine_dc8d5bf5d82c429fb73e3254a9d6e02a_RGBA_4;
    float3 _Combine_dc8d5bf5d82c429fb73e3254a9d6e02a_RGB_5;
    float2 _Combine_dc8d5bf5d82c429fb73e3254a9d6e02a_RG_6;
    Unity_Combine_float(_SampleTexture2D_edbfe6c53d7946cabac0cc1b359cafbe_R_4, _SampleTexture2D_edbfe6c53d7946cabac0cc1b359cafbe_G_5, _SampleTexture2D_edbfe6c53d7946cabac0cc1b359cafbe_B_6, _SampleTexture2D_edbfe6c53d7946cabac0cc1b359cafbe_A_7, _Combine_dc8d5bf5d82c429fb73e3254a9d6e02a_RGBA_4, _Combine_dc8d5bf5d82c429fb73e3254a9d6e02a_RGB_5, _Combine_dc8d5bf5d82c429fb73e3254a9d6e02a_RG_6);
    float _Property_0c7daecd7b8e44769f06c24c72a758b1_Out_0 = _UseTexMask;
    float _BooltoFloatCustomFunction_e039a8f7d149414fa1287085fc8c5d24_Out_1;
    BooltoFloat_float(_Property_0c7daecd7b8e44769f06c24c72a758b1_Out_0, _BooltoFloatCustomFunction_e039a8f7d149414fa1287085fc8c5d24_Out_1);
    float3 _Lerp_754ba431ac8846668079237ba8a516ae_Out_3;
    Unity_Lerp_float3(float3(255, 255, 255), _Combine_dc8d5bf5d82c429fb73e3254a9d6e02a_RGB_5, (_BooltoFloatCustomFunction_e039a8f7d149414fa1287085fc8c5d24_Out_1.xxx), _Lerp_754ba431ac8846668079237ba8a516ae_Out_3);
    float _Property_4dacc95c1e2843efbfc93fe5146b5cce_Out_0 = _Width;
    float _Maximum_d0fbce08912e45999c6d08e73790e64e_Out_2;
    Unity_Maximum_float(_Property_4dacc95c1e2843efbfc93fe5146b5cce_Out_0, 1, _Maximum_d0fbce08912e45999c6d08e73790e64e_Out_2);
    float _Property_37a7f5c6a1da454494520fdea10e9c7c_Out_0 = _Height;
    float _Maximum_b80044435c284985865e57503b47c7ea_Out_2;
    Unity_Maximum_float(_Property_37a7f5c6a1da454494520fdea10e9c7c_Out_0, 1, _Maximum_b80044435c284985865e57503b47c7ea_Out_2);
    float _Divide_7bce1d28091e47b09166b552637a3406_Out_2;
    Unity_Divide_float(_Maximum_d0fbce08912e45999c6d08e73790e64e_Out_2, _Maximum_b80044435c284985865e57503b47c7ea_Out_2, _Divide_7bce1d28091e47b09166b552637a3406_Out_2);
    float4 _UV_59ac78cf5496476798aee234cd17e98c_Out_0 = IN.uv0;
    float _Split_05ad5f4309b44c40a6cd0e4c68ac91f2_R_1 = _UV_59ac78cf5496476798aee234cd17e98c_Out_0[0];
    float _Split_05ad5f4309b44c40a6cd0e4c68ac91f2_G_2 = _UV_59ac78cf5496476798aee234cd17e98c_Out_0[1];
    float _Split_05ad5f4309b44c40a6cd0e4c68ac91f2_B_3 = _UV_59ac78cf5496476798aee234cd17e98c_Out_0[2];
    float _Split_05ad5f4309b44c40a6cd0e4c68ac91f2_A_4 = _UV_59ac78cf5496476798aee234cd17e98c_Out_0[3];
    float _Multiply_4c832bfe468840cab2be70117e0e228b_Out_2;
    Unity_Multiply_float(_Divide_7bce1d28091e47b09166b552637a3406_Out_2, _Split_05ad5f4309b44c40a6cd0e4c68ac91f2_R_1, _Multiply_4c832bfe468840cab2be70117e0e228b_Out_2);
    float4 _Combine_3b72a999d82545039e70a1f96e524bc4_RGBA_4;
    float3 _Combine_3b72a999d82545039e70a1f96e524bc4_RGB_5;
    float2 _Combine_3b72a999d82545039e70a1f96e524bc4_RG_6;
    Unity_Combine_float(_Multiply_4c832bfe468840cab2be70117e0e228b_Out_2, _Split_05ad5f4309b44c40a6cd0e4c68ac91f2_G_2, _Split_05ad5f4309b44c40a6cd0e4c68ac91f2_B_3, _Split_05ad5f4309b44c40a6cd0e4c68ac91f2_A_4, _Combine_3b72a999d82545039e70a1f96e524bc4_RGBA_4, _Combine_3b72a999d82545039e70a1f96e524bc4_RGB_5, _Combine_3b72a999d82545039e70a1f96e524bc4_RG_6);
    float _Property_d58e401c1bb448b69d85e5acac57e7c2_Out_0 = _UOff;
    float _Property_2574dbedc870463782e9fbddb89ae884_Out_0 = _MoveAmount;
    float _Multiply_799a1e021b564f7bb08b3c9f36d21eec_Out_2;
    Unity_Multiply_float(_Property_2574dbedc870463782e9fbddb89ae884_Out_0, IN.TimeParameters.x, _Multiply_799a1e021b564f7bb08b3c9f36d21eec_Out_2);
    float _Multiply_7aa76ce4f75b44d3ad99ea0998fd083f_Out_2;
    Unity_Multiply_float(_Property_d58e401c1bb448b69d85e5acac57e7c2_Out_0, _Multiply_799a1e021b564f7bb08b3c9f36d21eec_Out_2, _Multiply_7aa76ce4f75b44d3ad99ea0998fd083f_Out_2);
    float _Property_cfbfdef9bdfa47f49c6ad70253ca43f5_Out_0 = _VOff;
    float _Multiply_116203c5c8044dd1ad6f3b6c15feb5b6_Out_2;
    Unity_Multiply_float(_Multiply_799a1e021b564f7bb08b3c9f36d21eec_Out_2, _Property_cfbfdef9bdfa47f49c6ad70253ca43f5_Out_0, _Multiply_116203c5c8044dd1ad6f3b6c15feb5b6_Out_2);
    float4 _Combine_fbfec90524e34f10a355676b67d4f2ea_RGBA_4;
    float3 _Combine_fbfec90524e34f10a355676b67d4f2ea_RGB_5;
    float2 _Combine_fbfec90524e34f10a355676b67d4f2ea_RG_6;
    Unity_Combine_float(_Multiply_7aa76ce4f75b44d3ad99ea0998fd083f_Out_2, _Multiply_116203c5c8044dd1ad6f3b6c15feb5b6_Out_2, 0, 0, _Combine_fbfec90524e34f10a355676b67d4f2ea_RGBA_4, _Combine_fbfec90524e34f10a355676b67d4f2ea_RGB_5, _Combine_fbfec90524e34f10a355676b67d4f2ea_RG_6);
    float2 _TilingAndOffset_6f545536a6f64b019c74827b675bca22_Out_3;
    Unity_TilingAndOffset_float(_Combine_3b72a999d82545039e70a1f96e524bc4_RG_6, float2 (1, 1), _Combine_fbfec90524e34f10a355676b67d4f2ea_RG_6, _TilingAndOffset_6f545536a6f64b019c74827b675bca22_Out_3);
    float _Property_a69362d9d74c485295c51e4785006053_Out_0 = _RotateAmt;
    float _DegreesToRadians_e039c0ee46b7448e9d5096b24bbc4663_Out_1;
    Unity_DegreesToRadians_float(_Property_a69362d9d74c485295c51e4785006053_Out_0, _DegreesToRadians_e039c0ee46b7448e9d5096b24bbc4663_Out_1);
    float2 _Rotate_88ddec717a72424b8d38fec0a89e334e_Out_3;
    Unity_Rotate_Radians_float(_TilingAndOffset_6f545536a6f64b019c74827b675bca22_Out_3, float2 (0.5, 0.5), _DegreesToRadians_e039c0ee46b7448e9d5096b24bbc4663_Out_1, _Rotate_88ddec717a72424b8d38fec0a89e334e_Out_3);
    float _Split_8203d365c8204d36b6bf30e49b2f83bf_R_1 = _Rotate_88ddec717a72424b8d38fec0a89e334e_Out_3[0];
    float _Split_8203d365c8204d36b6bf30e49b2f83bf_G_2 = _Rotate_88ddec717a72424b8d38fec0a89e334e_Out_3[1];
    float _Split_8203d365c8204d36b6bf30e49b2f83bf_B_3 = 0;
    float _Split_8203d365c8204d36b6bf30e49b2f83bf_A_4 = 0;
    float _Property_258e62b5205d4bd282d9d3e1ea81f47f_Out_0 = _StripeAmt;
    float _Multiply_994e26f0510d4ca3b012a73b47a21296_Out_2;
    Unity_Multiply_float(_Split_8203d365c8204d36b6bf30e49b2f83bf_R_1, _Property_258e62b5205d4bd282d9d3e1ea81f47f_Out_0, _Multiply_994e26f0510d4ca3b012a73b47a21296_Out_2);
    float _Fraction_48817f6e21c44f37856f4691b2cbebfc_Out_1;
    Unity_Fraction_float(_Multiply_994e26f0510d4ca3b012a73b47a21296_Out_2, _Fraction_48817f6e21c44f37856f4691b2cbebfc_Out_1);
    float _Step_990610fddfcd480fa5f43bd006876c39_Out_2;
    Unity_Step_float(_Fraction_48817f6e21c44f37856f4691b2cbebfc_Out_1, 0.5, _Step_990610fddfcd480fa5f43bd006876c39_Out_2);
    float3 _Multiply_1dda426f42d7470e9607fe2de85facbc_Out_2;
    Unity_Multiply_float(_Lerp_754ba431ac8846668079237ba8a516ae_Out_3, (_Step_990610fddfcd480fa5f43bd006876c39_Out_2.xxx), _Multiply_1dda426f42d7470e9607fe2de85facbc_Out_2);
    surface.BaseColor = (_Property_d757e450105546818a5148247ee91cdb_Out_0.xyz);
    surface.Alpha = (_Multiply_1dda426f42d7470e9607fe2de85facbc_Out_2).x;
    return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
    VertexDescriptionInputs output;
    ZERO_INITIALIZE(VertexDescriptionInputs, output);

    output.ObjectSpaceNormal = input.normalOS;
    output.ObjectSpaceTangent = input.tangentOS.xyz;
    output.ObjectSpacePosition = input.positionOS;

    return output;
}
    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





    output.uv0 = input.texCoord0;
    output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
#else
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
#endif
#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

    return output;
}

    // --------------------------------------------------
    // Main

    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SpriteUnlitPass.hlsl"

    ENDHLSL
}
    }
        FallBack "Hidden/Shader Graph/FallbackError"
}