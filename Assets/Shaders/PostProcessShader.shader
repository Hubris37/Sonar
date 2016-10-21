Shader "Custom/PostProcessShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			name "PostEffects"
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _CameraDepthTexture;
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 uvd : TEXCOORD1; // For depth texture
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				
				o.uv = v.uv;
				o.uvd = v.uv;

				// On non-GL when AA is used, the main Texture and scene depth Texture
				// will come out in different vertical orientations.
				// So flip sampling of the Texture when that is the case (main Texture
				// texel size will have negative Y).
				// https://docs.unity3d.com/Manual/SL-PlatformDifferences.html
				#if UNITY_UV_STARTS_AT_TOP
					if (_MainTex_TexelSize.y < 0)
        				o.uvd.y = 1 - o.uvd.y;
				#endif

				return o;
			}

			// http://rastergrid.com/blog/2010/09/efficient-gaussian-blur-with-linear-sampling/
			// https://github.com/Jam3/glsl-fast-gaussian-blur
			// 9 tap gaussian blur
			float4 blur(sampler2D image, float2 uv, float2 resolution, float2 direction) {
				float4 color = float4(0, 0, 0, 0);

				float2 off1 = float2(1.3846153846, 1.3846153846) * direction;
				float2 off2 = float2(3.2307692308, 3.2307692308) * direction;

				color += tex2D(image, uv) * 0.2270270270;
				color += tex2D(image, uv + (off1 / resolution)) * 0.3162162162;
				color += tex2D(image, uv - (off1 / resolution)) * 0.3162162162;
				color += tex2D(image, uv + (off2 / resolution)) * 0.0702702703;
				color += tex2D(image, uv - (off2 / resolution)) * 0.0702702703;
				return color;
			}

			// 13 tap gaussian blur
			// float4 blur(sampler2D image, float2 uv, float2 resolution, float2 direction) {
			// 	float4 color = float4(0, 0, 0, 0);
			// 	float2 off1 = float2(1.411764705882353, 1.411764705882353) * direction;
			// 	float2 off2 = float2(3.2941176470588234, 3.2941176470588234) * direction;
			// 	float2 off3 = float2(5.176470588235294, 5.176470588235294) * direction;
			// 	color += tex2D(image, uv) * 0.1964825501511404;
			// 	color += tex2D(image, uv + (off1 / resolution)) * 0.2969069646728344;
			// 	color += tex2D(image, uv - (off1 / resolution)) * 0.2969069646728344;
			// 	color += tex2D(image, uv + (off2 / resolution)) * 0.09447039785044732;
			// 	color += tex2D(image, uv - (off2 / resolution)) * 0.09447039785044732;
			// 	color += tex2D(image, uv + (off3 / resolution)) * 0.010381362401148057;
			// 	color += tex2D(image, uv - (off3 / resolution)) * 0.010381362401148057;
			// 	return color;
			// }

			fixed4 frag (v2f i) : SV_Target
			{
				
				fixed4 fragColor = fixed4(0, 0, 0, 0); //tex2D(_MainTex, i.uv);

				fixed4 blurColor = fixed4(0, 0, 0, 0);
				blurColor += blur( _MainTex, i.uv, _ScreenParams.xy, float2(0, 1.2) ) * .5;
				blurColor += blur( _MainTex, i.uv, _ScreenParams.xy, float2(1.2, 0) ) * .5;

				half4 depth = tex2D(_CameraDepthTexture, i.uvd);
				float depthValue = Linear01Depth(depth);
				depth.r = depthValue;
				depth.g = depthValue;
				depth.b = depthValue;
				depth.a = 1;

				// Start: Chromatic Aberration
				fixed2 varyingOffset = fixed2(sin(_Time.g*0.4), cos(_Time.g*0.04)); // Moves in a circle
				// Offset the different colors in x and y
				fixed2 rOffset = fixed2(0.007 * depthValue, -0.005 * depthValue) * varyingOffset;
				fixed2 gOffset = fixed2(-0.005 * depthValue, 0.007 * depthValue);
				fixed2 bOffset = fixed2(-0.01 * depthValue, -0.01 * depthValue) * varyingOffset * 2;
				fragColor.r = tex2D(_MainTex, i.uv + rOffset).r;
				fragColor.g = tex2D(_MainTex, i.uv + gOffset).g;
				fragColor.b = tex2D(_MainTex, i.uv + bOffset).b;
				// End: Chromatic Aberration

				// return depth;
				return lerp(fragColor, blurColor, pow(depthValue,2));
				// return blurColor;
				// return fragColor;
			}
			ENDCG
		}
	}
}
