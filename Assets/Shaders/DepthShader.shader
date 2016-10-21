Shader "Custom/DepthShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass {
			Name "ShadowCaster"
			ZWrite On
			ColorMask 0

			// CGPROGRAM
            // #pragma vertex vert
            // #pragma fragment frag
            // #include "UnityCG.cginc"

            // struct v2f {
            //     float4 pos : SV_POSITION;
            //     float2 depth : TEXCOORD0;
            // };

            // v2f vert (appdata_base v) {
            //     v2f o;
            //     o.pos = UnityObjectToClipPos(v.vertex);
            //     UNITY_TRANSFER_DEPTH(o.depth);
            //     return o;
            // }

            // half4 frag(v2f i) : SV_Target {
            //     UNITY_OUTPUT_DEPTH(i.depth);
            // }
            // ENDCG
		}

		// Show depth in gray scale
		// Pass {
		// 	Blend SrcAlpha OneMinusSrcAlpha

		// 	ZWrite On

		// 	CGPROGRAM
		// 	#pragma vertex vert
		// 	#pragma fragment frag
		// 	#include "UnityCG.cginc"

		// 	struct appdata {
		// 		float4 vertex: POSITION;
		// 	};

		// 	struct v2f {
		// 		float4 vertex: SV_POSITION;
		// 		float depth: DEPTH;
		// 	};

		// 	v2f vert(appdata v) {
		// 		v2f o;
		// 		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		// 		o.depth = -mul(UNITY_MATRIX_MV, v.vertex).z * _ProjectionParams.w;
		// 		return o;
		// 	}

		// 	fixed4 frag(v2f i) : SV_Target {
		// 		float invert = 1-i.depth;
		// 		return fixed4(invert, invert, invert, 1);
		// 	}
		// 	ENDCG
		// }
	}
}
