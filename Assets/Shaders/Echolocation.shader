Shader "Custom/Echolocation" {
	Properties {
		_DefaultColor("Default Color", Color) = (0, 0, 0, 0)
		_EdgeWidth("Circle Edge Width", Range(0.0, 0.5)) = 0.15
		_DistortScale("Distort Scale", range(0.005, 0.1)) = 0.01
		_WallO("Wall Opacity", Range(0.00,1)) = 1.0
		[MaterialToggle] _UseNormalMap("Use Normal Map", Float) = 0
		_NormalMap ("Normal Map", 2D) = "bump" {}
		[MaterialToggle] _UseDepth("Use Depth Map", Float) = 1
	}

	SubShader {
		Tags { "RenderType"="Opaque" "IgnoreProjector"="True" }

		// https://docs.unity3d.com/Manual/SL-CullAndDepth.html
		// extra pass that renders to depth buffer only
		// Pass {
		// 	Name "ShadowCaster"
		// 	ZWrite On
		// 	ColorMask 0
		// }

		// Show depth in gray scale
		// Pass {
		// 	Blend SrcAlpha OneMinusSrcAlpha
		// 	ZWrite Off

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

		Pass {
			Blend SrcAlpha OneMinusSrcAlpha
			// Cull Back 	// Don’t render polygons facing away from the viewer. Performance stuff?
			// ZWrite On

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			#define MAX_CIRCLES 120 // Maximum circles allowed at once !!!CHANGE IN ShaderController AS WELL!!!

			float4 _Color[MAX_CIRCLES];
			float3 _Center[MAX_CIRCLES];
			float _Radius[MAX_CIRCLES];
			float _MaxRadius[MAX_CIRCLES];
			float _Frequency[MAX_CIRCLES];

			float4 _DefaultColor;
			float _EdgeWidth; // Circle Edge Width
			float _DistortScale; // Amplitude of bump map distortion
			float _WallO; // Wall Opacity	
			float _UseNormalMap;
			float _UseDepth;	
			int _NumCircles = 0;

			sampler2D _NormalMap;
			float4 _NormalMap_ST;

			struct v2f {
				float3 worldPos : TEXCOORD0;

				// these three vectors will hold a 3x3 rotation matrix
				// that transforms from tangent to world space
				half3 tspace0 : TEXCOORD1; // tangent.x, bitangent.x, normal.x
				half3 tspace1 : TEXCOORD2; // tangent.y, bitangent.y, normal.y
				half3 tspace2 : TEXCOORD3; // tangent.z, bitangent.z, normal.z

				float2 uv : TEXCOORD4; // texture coordinate for the normal map
				float depth: DEPTH;
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
				o.uv = TRANSFORM_TEX(uv, _NormalMap);
				o.depth = -mul(UNITY_MATRIX_MV, vertex).z * _ProjectionParams.w;;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target {
				half3 tnormal;
				half3 worldNormal;

				float invDepth = 1-i.depth;

				// half3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				// half3 worldRefl = reflect(-worldViewDir, worldNormal);
				// half3 refractDir = -refract(normalize(i.worldPos-_WorldSpaceCameraPos), worldNormal, .5);

				fixed4 finalColor = fixed4(_DefaultColor.rgb, _WallO);
				float bestAlpha = 0.1;

				for (int j = 0; j < _NumCircles; ++j) {
					float dist = distance(_Center[j], i.worldPos); // Distance from wave center to current fragment
					float val;

					// Used for wavy effect
					val = step(dist, _Radius[j]) * step(_Radius[j] - _EdgeWidth*6, dist); // Hollow circle
					i.uv.x += val * sin( (i.uv.x+i.uv.y)*dist + _Time.y ) * _DistortScale;
					// i.uv.x += sin(10*(i.uv.x+i.uv.y)*dist*1/_Radius[j] + _Time.y)*_DistortScale;
					i.uv.y += val * sin( (i.uv.x-i.uv.y)*dist + _Time.x*_Frequency[j] ) * _DistortScale*2;
					// END Used for wavy effect

					// sample the normal map, and decode from the Unity encoding
					tnormal = UnpackNormal(tex2D(_NormalMap, i.uv));
					// transform normal from tangent to world space
					worldNormal.x = dot(i.tspace0, tnormal);
					worldNormal.y = dot(i.tspace1, tnormal);
					worldNormal.z = dot(i.tspace2, tnormal);

					// Create circle
					// val += (1 - step(dist, _Radius[j] - _EdgeWidth) * 0.5) * step(dist, _Radius[j]);
					float weight = clamp(dist/_Radius[j], 0, 1);
					val = step(dist - _EdgeWidth*2, _Radius[j]) * lerp(3, 0, weight) * step(_Radius[j] - _EdgeWidth*2, dist);
					val += step(dist - _EdgeWidth*2, _Radius[j]) * lerp(0, 3, weight) * step(dist, _Radius[j] - _EdgeWidth*2);

					// val = step(dist, _Radius[j]) * lerp(5, 0, dist/_Radius[j]);

					float bump = (_UseNormalMap==1) ? max(0.0, dot(worldNormal, normalize(_WorldSpaceCameraPos-i.worldPos))) : 1;

					finalColor.rgb += (1 - _Radius[j]/_MaxRadius[j]) * val * _Color[j].rgb * bump;
					
					//finalColor.a += 0.05 + _MaxRadius[j]/200 * (1 - _Radius[j]/_MaxRadius[j]) * val;

					float curAlpha = 0.1 + _MaxRadius[j]/60 * (1 - _Radius[j]/_MaxRadius[j]) * val;
					if(curAlpha > bestAlpha)
						bestAlpha = curAlpha;
				}

				finalColor.a = bestAlpha;
				
				if(_UseDepth)
					return fixed4(finalColor.rgb * invDepth, finalColor.a);
				else
					return fixed4(finalColor.rgb, finalColor.a);
			}
			ENDCG
		}
		
		// Pass {

		// }
	}
	FallBack "Diffuse"
}