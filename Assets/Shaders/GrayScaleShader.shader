Shader "Hidden/GrayScale"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
			#pragma exclude_renderers d3d11 gles

			// Upgrade NOTE: excluded shader from DX11 because it uses wrong array syntax (type[size] name)
			#pragma exclude_renderers d3d11
			#pragma vertex vert_img
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;			

			fixed4 frag (v2f_img i) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, i.uv);

				return fixed4
				(
					c.r / 3 + c.g / 3 + c.b / 3,
					c.r / 3 + c.g / 3 + c.b / 3,
					c.r / 3 + c.g / 3 + c.b / 3,
					c.a
				);
			}

			ENDCG
		}
	}
}