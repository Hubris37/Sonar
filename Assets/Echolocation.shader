// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/Echolocation" {
	Properties{
		/*_Color("Color", Color) = (1, 1, 1, 1)
		_Center("CenterX", vector) = (0, 0, 0)
		_Radius("Radius", float) = 0*/
	}
	SubShader{
	Pass{
	Tags{ "RenderType" = "Opaque" }

	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#include "UnityCG.cginc"
	
	#define MAX_CIRCLES 10

	float4 _Color[MAX_CIRCLES];
	float3 _Center[MAX_CIRCLES];
	float _Radius[MAX_CIRCLES];

	struct v2f {
		float4 pos : SV_POSITION;
		float3 worldPos : TEXCOORD1;
	};

	v2f vert(appdata_base v) {
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
		return o;
	}

	fixed4 frag(v2f i) : COLOR {
		fixed4 finalColor;
		/*float dist = distance(_Center, i.worldPos);
		float val = 1 - step(dist, _Radius - 0.1) * 0.5;
		val = step(_Radius - 1.5, dist) * step(dist, _Radius) * val;
		return fixed4(val * _Color.r, val * _Color.g,val * _Color.b, 1.0);*/
		for (int j = 0; j < MAX_CIRCLES; ++j) {
			float dist = distance(_Center[j], i.worldPos);
			float val = 1 - smoothstep(dist, _Radius[j] - 0.1, 3) * 0.5;
			val = smoothstep(_Radius[j] - 1.5, dist, 7) * step(dist, _Radius[j]) * val;
			finalColor = finalColor + fixed4(val * _Color[j].r, val * _Color[j].g, val * _Color[j].b, 1.0);
		}
		return finalColor;
	}

		ENDCG
	}
	}
		FallBack "Diffuse"
}