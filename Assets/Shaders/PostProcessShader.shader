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
		
		UsePass "Custom/DepthShader/SHADOWCASTER"

		Pass
		{
			name "DoF"
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _CameraDepthTexture;
			sampler2D _LastCameraDepthTexture;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				
				o.uv = v.uv;
				#if defined (UNITY_UV_STARTS_AT_BOTTOM)
					o.uv.y = 1 - o.uv.y;
				#endif

				return o;
			}
			
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;

			#define NUM_STEPS 7
			// float offset[3] = { 0.0, 1.3846153846, 3.2307692308 };
			// float weight[3] = { 0.2270270270, 0.3162162162, 0.0702702703 };

			fixed4 frag (v2f i) : SV_Target
			{
				float _Offset[NUM_STEPS] = { 0, 1, 2, 4, 6, 8, 10 };
				float _Weight[NUM_STEPS] = { 0.15, 0.17, 0.05, 0.01, 0.005, 0.005, 0.005 };
				float2 tSize = _MainTex_TexelSize.xy * 2;
				
				fixed4 fragColor = tex2D(_MainTex, i.uv);

				fixed4 blurColor = tex2D(_MainTex, i.uv) * _Weight[0];
				blurColor += tex2D(_MainTex, i.uv) * _Weight[0];

				for (int j = 1; j < NUM_STEPS; ++j) {
					blurColor += tex2D( _MainTex, i.uv + float2(0, _Offset[j] * tSize.y) ) * _Weight[j];
					blurColor += tex2D( _MainTex, i.uv + float2(_Offset[j] * tSize.x, 0) ) * _Weight[j];

					// blurColor += texture2D( _MainTex, ( vec2(gl_FragCoord)+vec2(0.0, _Offset[j]) )/1024.0 )
					// 		* _Weight[j];

					blurColor += tex2D( _MainTex, i.uv - float2(0, _Offset[j] * tSize.y) ) * _Weight[j];
					blurColor += tex2D( _MainTex, i.uv - float2(_Offset[j] * tSize.x, 0) ) * _Weight[j];

					// blurColor += texture2D( _MainTex, ( vec2(gl_FragCoord)-vec2(0.0, _Offset[j]) )/1024.0 )
					// 		* _Weight[j];
				}

				// just invert the colors
				// fragColor = 1 - fragColor;

				float3 normalValues;
				half4 depth = tex2D(_CameraDepthTexture, i.uv);
				float depthValue = 1 - Linear01Depth(depth);

				depth.r = depthValue;
				depth.g = depthValue;
				depth.b = depthValue;
				depth.a = 1;

				return depth;
				// return lerp(fragColor, blurColor, depthValue);
			}
			ENDCG
		}
	}
}
