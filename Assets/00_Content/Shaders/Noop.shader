Shader "Unlit/Noop"
{
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		// Don't write anything to the render target
		ColorMask 0
		ZWrite Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			float4 vert(float4 vertex : POSITION) : SV_POSITION {
				return UnityObjectToClipPos(vertex);
			}

			fixed4 frag() : SV_Target {
				return 0;
			}
			ENDCG
		}
	}
}