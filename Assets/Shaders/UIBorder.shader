// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)
// very simple edit to turn into scrolling texture setup by @Minionsart
Shader "UI/Border"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _SpeedX("Speed X", Range(-4,4)) = 1.0
        _SpeedY("Speed Y", Range(-4,4)) = 1.0
        _Gradient("Color Map", 2D) = "white" {}
 
        _Fade("Fade Top", Range(0,1)) = 1.0
        _Fade2("Fade Bottom", Range(0,1)) = 1.0
        [Toggle(FadeEdges)] _FadeEdges("Fade Edges", Float) = 0
 
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
 
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }
 
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
 
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
 
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]
 
        Pass
        {
            Name "Default"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
 
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
 
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP
            #pragma shader_feature FadeEdges
 
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
 
            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float2 texcoord2 : TEXCOORD2;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };
 
            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
            float _SpeedX, _SpeedY;
            sampler2D _Gradient;
            float _Fade, _Fade2;
 
            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
 
                OUT.texcoord = v.texcoord;
                OUT.texcoord2 = TRANSFORM_TEX(v.texcoord, _MainTex);
                OUT.color = v.color * _Color;
                return OUT;
            }
 
            fixed4 frag(v2f IN) : SV_Target
            {
                // create scrolling uvs
                float2 movingUV = float2(IN.texcoord2.x + (_Time.x * _SpeedX), IN.texcoord2.y + (_Time.y * _SpeedY));
                // main texture
                half4 mainTex = (tex2D(_MainTex, movingUV) + _TextureSampleAdd) * IN.color;
 
                #ifdef UNITY_UI_CLIP_RECT
                    mainTex.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif
                // clipping
                #ifdef UNITY_UI_ALPHACLIP
                    clip (mainTex.a - 0.001);
                #endif
                // use the main texture to colorize the whole thing
                float4 colorMap = tex2D(_Gradient, mainTex)  ;
                colorMap *= mainTex.a;
                // fade the edges
                #if FadeEdges
                    float gradientfalloff =   smoothstep(0.99, _Fade, IN.texcoord.y) * smoothstep(0.99, _Fade2,1- IN.texcoord.y);
                    colorMap *= gradientfalloff;
                #endif
                return colorMap;
            }
            ENDCG
        }
    }
}