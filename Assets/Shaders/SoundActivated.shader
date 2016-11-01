Shader "Custom/SoundActivated"
{
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_DefaultColor("Default Color", Color) = (1, 0, 0, 1)
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" "IgnoreProjector"="True" }

		Pass {
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			#define MAX_CIRCLES 120 // Maximum circles allowed at once !!!CHANGE IN ShaderController AS WELL!!!

			float3 _Center[MAX_CIRCLES];
			float _Radius[MAX_CIRCLES];
			float _MaxRadius[MAX_CIRCLES];
			float _Frequency[MAX_CIRCLES];
			int _NumCircles;

			float4 _DefaultColor;

			sampler2D _MainTex;
			float4 _MainTex_ST;

			struct v2f {
				float3 worldPos : TEXCOORD0;
				float2 uv : TEXCOORD1;
				float4 pos : SV_POSITION;
			};

			v2f vert(float4 vertex : POSITION, float2 uv : TEXCOORD0) {
				v2f o;
				o.pos = UnityObjectToClipPos(vertex);
				o.worldPos = mul(unity_ObjectToWorld, vertex).xyz;

				o.uv = TRANSFORM_TEX(uv, _MainTex);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target {
				fixed4 finalColor = tex2D(_MainTex, i.uv) * _DefaultColor;
				finalColor.a = 0;

				for (int j = 0; j < _NumCircles; ++j) {
					float dist = distance(_Center[j], i.worldPos); // Distance from wave center to current fragment
					
					finalColor.a += (dist <= _Radius[j]) ? 0.1 : 0;
				}
				
				return finalColor;
			}
			ENDCG
		}
	}
}
