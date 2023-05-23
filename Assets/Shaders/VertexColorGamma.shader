Shader "Custom/VertexColor"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"
        }
        Lighting Off
        Fog {Mode Off}
        ZWrite On
        //        Blend SrcAlpha OneMinusSrcAlpha
        Blend SrcAlpha OneMinusSrcAlpha
        Cull off
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct VertexData
            {
                float4 vertex : POSITION;
                half4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct FragmentData
            {
                fixed4 vertex : SV_POSITION;
                half4 color : COLOR;
                fixed4 uv : TEXCOORD0;
            };

            float4 _Color;
            sampler2D _MainTex;

            FragmentData vert(VertexData v)
            {
                FragmentData o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.vertex;
// #if UNITY_COLORSPACE_GAMMA
                o.color = pow(v.color, fixed4(2.2, 2.2, 2.2, 1.0));
// #else
                // o.color = v.color;
// #endif

                return o;
            }

            fixed4 frag(FragmentData i) : Color
            {
                fixed4 color = i.color;

                return color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}