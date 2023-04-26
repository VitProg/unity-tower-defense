Shader "Custom/HexFlat"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white" {}
        _GridColor("Grid Color", Color) = (1,1,1,1)
        _BgColor("BG Color", Color) = (0,0,0,0)
        _GridWidth("Grid Width", Range(0,50)) = 0
        _Shift("Shift", Vector) = (0,0,0,0)
        _LightPosition("Light Position", Vector) = (1, 1, 0, 0)
        _LightRadius("Light Radius", Range(0, 20)) = 1
        _LightPower("Light Power", Range(0, 20)) = 1
        _YCompress("Compress by Y", Range(0.1, 1)) = 0.85
        _LightSnapToGrid("Snaping light position to grid", Range(0, 1)) = 0.0
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

            sampler2D   _MainTex;
            
            fixed4 _BgColor;
            fixed4 _GridColor;
            fixed _GridWidth;

            fixed4 _Shift;
            
            fixed2 _LightPosition;
            fixed _LightRadius;
            fixed _LightPower;
            fixed _YCompress;
            fixed _LightSnapToGrid;

            fixed sqr3 = 1.7320508076;
            fixed2 vec2_05 = fixed2(0.5, 0.5);
            fixed4 vec4_0 = fixed4(0, 0, 0, 1);

            /////////////////////////
            
            fixed2 get_shift()
            {
                return fixed2(
                    _Shift.x + 1.879/*- 0.075*/,
                    (_Shift.y /*- 0.035*/) / (1 /_YCompress * 2)
                );
            }

            fixed hex(fixed2 p, fixed h1)
            {
                fixed2 h = fixed2(h1, h1);
                fixed2 q = abs(p);
                return max(
                    q.x - h.y,
                    max(
                        q.x + q.y * 0.57735,
                        q.y * 1.1547
                    ) - h.x
                );
            }

            fixed4 hex_grid(const in fixed2 _pos)
            {
                fixed2 pos = fixed2(_pos.x, _pos.y / _YCompress);
                
                const fixed space = _GridWidth / 2.0;
                const fixed outer_radius = 0.867938799; //0.861 * 1.008059;
                const fixed inner_radius = outer_radius - space;
                const fixed inner_radius_sqr3 = inner_radius / 1.7320508076;

                const fixed2 grid = fixed2(outer_radius * 1.7320508076, outer_radius);

                const fixed2 p1 = fmod(abs(pos), grid) - grid * 0.5;
                const fixed d1 = hex(p1, inner_radius_sqr3);

                const fixed2 p2 = fmod(abs(pos + grid * 0.5), grid) - grid * 0.5;
                const fixed d2 = hex(p2, inner_radius_sqr3);

                fixed d = min(d1, d2);

                // const fixed4 c1 = d1 > 0.0 ? fixed4(.3, .35, 0, 0.5) : vec4_0;
                // const fixed4 c2 = d2 > 0.0 ? fixed4(0, .3, .2, 0.5) : vec4_0;

                fixed2 lightPos = fixed2(_LightPosition);
                lightPos.y= lightPos.y / _YCompress;

                if (_LightSnapToGrid > 0.5)
                {
                    // to cell coordinates
                    int x = floor(lightPos.x / 0.75);
                    int y = floor(((lightPos.y / _YCompress) - (abs(x) % 2) * 0.8660254 / 2) / 0.8660254);
                    
                    // to position
                    lightPos = fixed2(
                        0.75 * (x + 0.5f),
                        0.8660254 * (y + (fmod(abs(x), 2)) / 2.0 + 0.5) * _YCompress
                    );
                }
                lightPos = lightPos + get_shift();

                // todo
                fixed lightDistance = distance(pos, lightPos);
                float lightIntensity = smoothstep(_LightRadius, 0, lightDistance);
                lightIntensity = pow(lightIntensity, 1/_LightPower);
                
                fixed4 color = d > 0 ?
                    fixed4(_GridColor.x, _GridColor.y, _GridColor.z, _GridColor.w * lightIntensity) :
                    fixed4(_BgColor.x, _BgColor.y, _BgColor.z, _BgColor.w * lightIntensity);

                // float4 texture = tex2D(_MainTex, pos);

                return color;
            }


            /////////////////////////

            v2f vert(const appdata_t v)
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

            fixed4 frag(const v2f input) : COLOR
            {
                fixed4 col = hex_grid(input.worldSpacePos + get_shift());

                UNITY_APPLY_FOG(input.fogCoord, col);

                return col;
            }
            ENDCG
        }
    }
}