Shader "Shader/Hex2"
{
    Properties
    {
        _GridColor("Grid Color", Color) = (1,1,1,1)
        _BgColor("Back Ground Color", Color) = (0,0,0,0)
        _Colmun("Colmun", float) = 1
        _GridWidth("Grid Width", Range(0,1)) = 0
        _Center("Center(xy) OffsetX(zw)", Vector) = (0,0,0,0)

        _Target("A(xy)", Vector) = (0,0,0,0)
        _Shift("Shift", Vector) = (0,0,0,0)
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"
        }
        LOD 100

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 texcoord : TEXCOORD0;
                float4 worldSpacePos : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 uv : TEXCOORD0;
                float4 worldSpacePos : TEXCOORD1;
                UNITY_FOG_COORDS(0)
                UNITY_VERTEX_OUTPUT_STEREO
            };

            fixed4 _BgColor;
            fixed4 _GridColor;
            fixed _Colmun;
            fixed _GridWidth;

            fixed4 _Target;


            fixed4 _Center;
            fixed4 _Shift;

            /////////////////////////

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.vertex;
                o.worldSpacePos = mul(unity_ObjectToWorld, v.vertex);
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag(v2f input) : COLOR
            {
                float size = 1.0;
                float2 pixel = input.worldSpacePos + _Shift;
                
                fixed q = (pixel.x * 0.57735026918963 - pixel.y * 0.33333333333) / size;
                fixed r = pixel.y * 0.66666666666 / size;
                
                fixed rx = round(q);
                fixed ry = round(-q - r);
                fixed rz = round(r);
                
                fixed yx = step(abs(ry + q + r), abs(rx - q));
                fixed zx = step(abs(rz - r), abs(rx - q));
                fixed zy = step(abs(rz - r), abs(ry + q + r));
                
                rx = (yx * zx) * (-ry - rz) + (1 - yx * zx) * rx;
                ry = (zy - yx * zx * zy) * (-rx - rz) + (1 - zy + yx * zx * zy) * ry;
                rz = (1 - zy + yx * zx * zy) * (-rx - ry) + (zy - yx * zx * zy) * rz;
                
                fixed3 n[6] = {
                    fixed3(0, 1, -1),
                    fixed3(1, 0, -1),
                    fixed3(-1, 1, 0),
                    fixed3(1, -1, 0),
                    fixed3(-1, 0, 1),
                    fixed3(0, -1, 1),
                };
                
                fixed mindis = 2;
                for (int i = 0; i < 6; i++)
                {
                    fixed2 pos = fixed2(size * 1.73205080756887 * ((rx + n[i].x) + (rz + n[i].z) * 0.5),
                                        size * 1.5 * (rz + n[i].z));
                    fixed a = distance(pixel.xy, pos);
                    mindis = min(mindis, a);
                }
                
                fixed isEdge = step(
                    abs(distance(pixel.xy, fixed2(size * 1.73205080756887 * (rx + rz * 0.5), size * 1.5 * rz)) - mindis),
                    size * _GridWidth);
                
                fixed4 col = isEdge * _GridColor + (1 - isEdge) * _BgColor;
                
                UNITY_APPLY_FOG(i.fogCoord, col);
                
                return col;
            }
            ENDCG
        }
    }

}