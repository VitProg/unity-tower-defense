Shader "Custom/Shard2"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white" {}
        _ColorsTex("ColorsTex", 2D) = "white" {}
        _UtilTex("Utils Texture", 2D) = "white" {}
        _innerRadius("inner Radius", Range(0, 1)) = 0.15
        _outerRadius("Outer Radius", Range(0, 1)) = 0.5
        _scale("Scale", Range(1, 500)) = 1

        _rotateSpeed("RotateSpeed", Range(0, 100)) = 0
        _blurAnim("Animate Blur", Range(0, 100)) = 0

        _smooth("Smooth", Range(0, 10)) = 0.1
        _smoothMul("SmoothMul", Range(0, 100)) = 40.0
        _threshold("threshold", Range(0, 0.1)) = 0.015
        _transpatenrSmooth("TranspatenrSmooth", Range(0, 0.5)) = 0.05

        //        _color1("Color 1", Color) = (1.0, 0.0, 0.0)
        //        _color2("Color 2", Color) = (0.0, 1.0, 0.0)
        //        _color3("Color 3", Color) = (0.0, 0.0, 1.0)
        //        _color4("Color 4", Color) = (0.8, 0.5, 0.0)
        //        _color5("Color 5", Color) = (1.0, 0.4, 0.8)
        //        _color6("Color 6", Color) = (0.9, 1.0, 0.0)
        //        _color7("Color 7", Color) = (0.4, 0.0, 0.9)
        //        _color8("Color 8", Color) = (0.0, 0.9, 0.7)

        //        _backgroundColor("Background Color", Color) = (0.0, 0.0, 0.0)

        _value1234("Value 1, 2, 3, 4", Vector) = (1.0, 1.0, 1.0, 1.0)
        _value5678("Value 5, 6, 7, 8", Vector) = (1.0, 1.0, 1.0, 1.0)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"
        }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull back
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            // #include "DebugNode.hlsl"

            struct appdata_t
            {
                fixed4 vertex : POSITION;
                fixed4 texcoord : TEXCOORD0;
                // fixed4 worldSpacePos : TEXCOORD1;

                half4 uv1 : TEXCOORD0;
                half4 uv2 : TEXCOORD1;
                half4 uv3 : TEXCOORD2;
                half4 uv4 : TEXCOORD3;
                half4 uv5 : TEXCOORD4;
            };

            struct v2f
            {
                fixed4 vertex : SV_POSITION;
                half4 uv : TEXCOORD0;
                // fixed4 worldSpacePos : TEXCOORD3;

                half4 uv1 : TEXCOORD1;
                half4 uv2 : TEXCOORD2;
                half4 uv3 : TEXCOORD3;
                half4 uv4 : TEXCOORD4;
            };

            sampler2D _MainTex;
            sampler2D _ColorsTex;
            sampler2D _UtilTex;

            fixed _innerRadius;
            fixed _outerRadius;
            fixed _scale;

            fixed _rotateSpeed;
            fixed _blurAnim;

            // fixed4 _color1;
            // fixed4 _color2;
            // fixed4 _color3;
            // fixed4 _color4;
            // fixed4 _color5;
            // fixed4 _color6;
            // fixed4 _color7;
            // fixed4 _color8;

            // fixed4 _backgroundColor;

            fixed4 _value1234;
            fixed4 _value5678;

            fixed _smooth;
            fixed _smoothMul;
            fixed _threshold;
            fixed _transpatenrSmooth;

            ////////////////////////////////////////

            #define C_STEP 0.125
            #define C_OFFSET 0.0625

            ////////////////////////////////////////
            #ifndef POLAR_COORDINATES
            #define POLAR_COORDINATES

            fixed2 toPolar(fixed2 cartesian)
            {
                fixed distance = length(cartesian);
                fixed angle = atan2(cartesian.y, cartesian.x);
                return fixed2(angle / UNITY_TWO_PI, distance);
            }

            fixed2 toCartesian(fixed2 polar)
            {
                fixed2 cartesian;
                sincos(polar.x * UNITY_TWO_PI, cartesian.y, cartesian.x);
                return cartesian * polar.y;
            }

            #endif
            ////////////////////////////////////////

            v2f vert(const appdata_t v)
            {
                v2f o;
                // UNITY_SETUP_INSTANCE_ID(v);
                // UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.vertex;
                o.uv1 = v.uv1;
                o.uv2 = v.uv2;
                o.uv3 = v.uv3;
                o.uv4 = v.uv4;
                // o.worldSpacePos = mul(unity_ObjectToWorld, v.vertex);
                // UNITY_TRANSFER_FOG(o, o.vertex);

                // o.value1234 = fixed4(v.value12.x, v.value12.y, v.value34.x, v.value34.y);
                // o.value5678 = fixed4(v.value56.x, v.value56.y, v.value78.x, v.value78.y);

                return o;
            }

            // fixed4 getPrevColor(uint sectorIndex, const fixed values[8], const fixed4 colors[8])
            // {
            //     for (uint index = sectorIndex - 1 + 8; index > sectorIndex - 1; index--)
            //     {
            //         uint correctIndex = index % 8;
            //
            //         if (values[correctIndex] >= _threshold)
            //         {
            //             return colors[correctIndex];
            //         }
            //     }
            //     return fixed4(0, 0, 0, 1);
            //     // uint startIndex = (sectorIndex + 7) % 8;
            //     // uint startIndex = sectorIndex - 1 == 0 ? 7 : sectorIndex - 1;
            //     //
            //     // for (uint i = 0; i < 8; i++)
            //     // {
            //     //     // const uint index = startIndex - i == 0 ? 7 : startIndex - i;
            //     //     const uint index = startIndex - i == 0 ? 7 : startIndex - i;
            //     //     if (values[index] >= _threshold)
            //     //     {
            //     //         return colors[index];
            //     //     } 
            //     // }
            //     //
            //     // return colors[startIndex];
            // }
            //
            // fixed4 findNextColor(uint sectorIndex, const fixed values[8], const fixed4 colors[8])
            // {
            //     for (uint index = sectorIndex + 1; index < sectorIndex + 1 + 8; index++)
            //     {
            //         uint correctIndex = index % 8;
            //
            //         if (values[correctIndex] >= _threshold)
            //         {
            //             return colors[correctIndex];
            //         }
            //     }
            //     return fixed4(0, 0, 0, 1);
            //
            //     // fixed startIndex = sectorIndex + 1 == 8 ? 0 : sectorIndex + 1;
            //
            //     // for (uint i = 0; i < 8; i++)
            //     // {
            //     // const uint index = sectorIndex + 1 == 8 ? 0 : sectorIndex + 1;
            //     // if (values[index] >= _threshold)
            //     // {
            //     // return colors[index];
            //     // } 
            //     // }
            //
            //     // return colors[startIndex];
            // }

            fixed4 getColor(uint index)
            {
                return tex2D(_ColorsTex, fixed2(C_OFFSET + C_STEP * index, 0.0));
            }

            fixed4 getPrevColor(uint index, fixed4 colors[8], uint activeColors)
            {
                uint correctIndex = (index - 1 + activeColors) % activeColors;
                return colors[correctIndex];
            }

            fixed4 getNextColor(uint index, fixed4 colors[8], uint activeColors)
            {
                uint correctIndex = (index + 1) % activeColors;
                return colors[correctIndex];
            }


            fixed4 frag(const v2f input) : COLOR
            {
                fixed2 coord = input.uv; // * 1000.0;

                half4 a = tex2D(_UtilTex, input.uv1);
                half4 b = tex2D(_UtilTex, float2(input.uv1.x + 0.0009765625, input.uv1.y));

                // return float4(
                //     input.uv1.x, input.uv1.y, 0.0, 1.0
                // );

                fixed inputValues[8] = {
                    a.x, a.y, a.z, a.w,
                    b.x, b.y, b.z, b.w,
                };

                fixed all = inputValues[0] + inputValues[1] + inputValues[2] + inputValues[3] +
                    inputValues[4] + inputValues[5] + inputValues[6] + inputValues[7]; 

                // fixed inputValues[8] = {
                    // input.uv1.x, input.uv1.y, input.uv2.x, input.uv2.y,
                    // input.uv3.x, input.uv3.y, input.uv4.x, input.uv4.y
                // };

                // const fixed4 colors[8] = {
                // tex2D(_ColorsTex, fixed2(cto + cts, 0.0)),
                // tex2D(_ColorsTex, fixed2(cto + cts * 2.0, 0.0)),
                // tex2D(_ColorsTex, fixed2(cto + cts * 3.0, 0.0)),
                // tex2D(_ColorsTex, fixed2(cto + cts * 4.0, 0.0)),
                // tex2D(_ColorsTex, fixed2(cto + cts * 5.0, 0.0)),
                // tex2D(_ColorsTex, fixed2(cto + cts * 6.0, 0.0)),
                // tex2D(_ColorsTex, fixed2(cto + cts * 7.0, 0.0)),
                // tex2D(_ColorsTex, fixed2(cto + cts * 8.0, 0.0)),
                // };
                // const fixed4 colors[8] = {
                // _color1, _color2, _color3, _color4,
                // _color5, _color6, _color7, _color8
                // };

                fixed values[8];
                fixed4 colors[8];
                uint activeColors = 0;

                for (fixed i = 0; i < 8; i++)
                {
                    // if (inputValues[i] / all < _threshold)
                    // {
                    //all -= values[i];
                    // values[activeColors] = 0;
                    // colors[activeColors]
                    // }
                    // else
                    if (inputValues[i] / all >= _threshold)
                    {
                        values[activeColors] = inputValues[i];
                        colors[activeColors] = getColor(i);
                        activeColors++;
                    }
                }

                fixed corrAll = 0.0;
                for (fixed i = 0; i < activeColors; i++)
                {
                    corrAll += values[i];
                }

                for (fixed i = 0; i < activeColors; i++)
                {
                    values[i] /= corrAll;
                }

                const fixed shift = 1.0;

                //

                fixed rotateSpeed = _Time.x * _rotateSpeed;

                fixed2 polar = toPolar(input.uv) + .5 * shift + rotateSpeed; //+ fixed2(0.0, shift);
                fixed polarX = fmod(polar.x, 1.0);

                fixed4 color = fixed4(0.0, 0.0, 0.0, 0.0);

                fixed sectorIndex = -1;
                fixed currentStep = 0.0;
                fixed sectorStart = 0.0;
                fixed sectorEnd = 0.0;
                // fixed4 sectorColor = color;

                for (fixed i = 0; i < activeColors; i++)
                {
                    fixed val = values[i];
                    if (polarX > currentStep && polarX < currentStep + val)
                    {
                        sectorIndex = i;
                        sectorStart = currentStep;
                        sectorEnd = currentStep + val;
                        // sectorColor = getColor(8 + i);
                        break;
                    }
                    currentStep += val;
                }

                const fixed radius = length(coord);
                const fixed outerRadius = _outerRadius * _scale;
                const fixed innerRadius = _innerRadius * _scale;
                const fixed smoothMul = _smoothMul;
                const fixed smooth = _smooth / 100.0f;
                const fixed transpatenrSmooth = _transpatenrSmooth * _scale;

                if (sectorIndex >= 0)
                {
                    if (activeColors > 1)
                    {
                        fixed4 sectorColor = colors[sectorIndex];
                        fixed sectorLength = abs(sectorEnd - sectorStart);
                        fixed sectorMiddle = sectorLength / (2.1);
                        fixed smoothEdge = smooth + ((sectorMiddle / 100.0) * (smoothMul * (_blurAnim > 0.0
                            ? sin(_Time.x * _blurAnim) / 8.0 + 0.8
                            : 1.0)));

                        color = sectorColor;

                        if (polarX - sectorStart < smoothEdge && polarX < sectorStart + sectorMiddle)
                        {
                            fixed4 prevColor = getPrevColor(sectorIndex, colors, activeColors);
                            //findPrevColor(sectorIndex, values, colors);//colors[(sectorIndex + 7) % 8];
                            // color = color / 3.0;
                            // color = (prevColor + color) / 2.0;//fixed4(1.0, 0.9, 0.8, 0.5);
                            fixed progress = (polarX - sectorStart) / min(smoothEdge, sectorMiddle);
                            color = lerp(color, (prevColor + color) / 2.0, 1.0 - smoothstep(0.0, 1.0, sqrt(progress)));
                            // if (outerRadius - outerRadius < transpatenrSmooth*2)
                            // {
                            //     color = prevColor;
                            // }
                        }
                        else if (sectorEnd - polarX < smoothEdge && polarX > sectorEnd - sectorMiddle)
                        {
                            fixed4 nextColor = getNextColor(sectorIndex, colors, activeColors);
                            //findNextColor(sectorIndex, values, colors);//colors[(sectorIndex + 1) % 8];
                            // color = color + 0.15;
                            // color = fixed4(0.5, 1.0, 0.7, 0.3);
                            fixed progress = (sectorEnd - polarX) / smoothEdge;
                            color = lerp((nextColor + color) / 2.0, color, smoothstep(0.0, 1.0, sqrt(progress)));
                            // if (outerRadius - outerRadius < transpatenrSmooth)
                            // {
                            //     color = nextColor;
                            // }
                        }
                    }
                    else
                    {
                        color = colors[sectorIndex];
                    }
                }

                if (radius - innerRadius < transpatenrSmooth && radius < outerRadius)
                {
                    // color += 0.15;
                    fixed progress = (radius - innerRadius) / transpatenrSmooth;
                    color = lerp(fixed4(color.rgb, 0.0), color, sqrt(smoothstep(0.0, 1.0, (progress))));
                }
                if (outerRadius - radius < transpatenrSmooth && radius < outerRadius)
                {
                    // color /= 2;
                    fixed progress = (outerRadius - radius) / transpatenrSmooth;
                    color = lerp(fixed4(color.rgb, 0.0), color, sqrt(smoothstep(0.0, 1.0, (progress))));
                }
                else if (radius >= outerRadius)
                {
                    color = fixed4(0.0, 0.0, 0.0, 0.0);
                }

                return color;
            }
            ENDCG
        }
    }
}