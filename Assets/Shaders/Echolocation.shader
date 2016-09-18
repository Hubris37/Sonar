// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/Echolocation" {
	Properties{
		/*_Color("Color", Color) = (1, 1, 1, 1)
		_Center("CenterX", vector) = (0, 0, 0)
		_Radius("Radius", float) = 0*/
		_Width("Circle Width", Range(0.05, 0.5)) = 0.3
		_WallO("Wall Opacity", Range(0.001,1)) = 1.0
		_BumpMap ("Normal Map", 2D) = "bump" {}
	}
		SubShader{
			Pass{
			Tags{ "RenderType" = "Transparent" }

			Blend SrcAlpha OneMinusSrcAlpha
			//Cull Back 	// Don’t render polygons facing away from the viewer. Performance stuff?

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			#define MAX_CIRCLES 200

			float4 _Color[MAX_CIRCLES];
			float3 _Center[MAX_CIRCLES];
			float _Radius[MAX_CIRCLES];
			float _MaxRadius[MAX_CIRCLES];
			float _Width; // Circle Width	
			float _WallO; // Wall Opacity		
			int _NumCircles = 0;

			struct v2f {
				float3 worldPos : TEXCOORD0;
                // these three vectors will hold a 3x3 rotation matrix
                // that transforms from tangent to world space
                half3 tspace0 : TEXCOORD1; // tangent.x, bitangent.x, normal.x
                half3 tspace1 : TEXCOORD2; // tangent.y, bitangent.y, normal.y
                half3 tspace2 : TEXCOORD3; // tangent.z, bitangent.z, normal.z
                // texture coordinate for the normal map
                float2 uv : TEXCOORD4;
                float4 pos : SV_POSITION;
			};

			v2f vert(float4 vertex : POSITION, float3 normal : NORMAL, float4 tangent : TANGENT, float2 uv : TEXCOORD0) {
				v2f o;
				o.pos = UnityObjectToClipPos(vertex);
				o.worldPos = mul(unity_ObjectToWorld, vertex).xyz;
				
				half3 wNormal = UnityObjectToWorldNormal(normal);
                half3 wTangent = UnityObjectToWorldDir(tangent.xyz);
                // compute bitangent from cross product of normal and tangent
                half tangentSign = tangent.w * unity_WorldTransformParams.w;
                half3 wBitangent = cross(wNormal, wTangent) * tangentSign;
                // output the tangent space matrix
                o.tspace0 = half3(wTangent.x, wBitangent.x, wNormal.x);
                o.tspace1 = half3(wTangent.y, wBitangent.y, wNormal.y);
                o.tspace2 = half3(wTangent.z, wBitangent.z, wNormal.z);
				o.uv = uv;
				return o;
			}

			sampler2D _NormalMap;

			fixed4 frag(v2f i) : SV_Target {
				// sample the normal map, and decode from the Unity encoding
                half3 tnormal = UnpackNormal(tex2D(_NormalMap, i.uv));
                // transform normal from tangent to world space
                half3 worldNormal;
                worldNormal.x = dot(i.tspace0, tnormal);
                worldNormal.y = dot(i.tspace1, tnormal);
                worldNormal.z = dot(i.tspace2, tnormal);

				half3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                half3 worldRefl = reflect(-worldViewDir, worldNormal);

				fixed4 finalColor = fixed4(0, 0, 0, 0.4);

				for (int j = 0; j < _NumCircles; ++j) {
					float dist = distance(_Center[j], i.worldPos); // Distance from wave center to current fragment
					//float val = 1 - step(dist, _Radius[j] - 0.1) * 0.5; // Creates small edge on circle

					float val = lerp(0, _Radius[j], dist/_Radius[j]) * step(dist, _Radius[j]);
					
					float bump = max(0.0, dot(worldNormal, normalize(_Center[j]+worldNormal-i.worldPos)));
					finalColor.rgb += (1 - _Radius[j]/_MaxRadius[j]) * _Color[j].rgb * val * bump;
				}
				// finalColor.a = 1;
				return finalColor;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}