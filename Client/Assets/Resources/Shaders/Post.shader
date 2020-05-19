Shader "Custom/Post" {
    Properties {
		_DarkAmount("DarkAmount", Range(0, 1)) = 0
		_SafeWallTex("SafeWall", 2D) = "white"{}
    }

    SubShader{
        Pass{
			Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"

			fixed _DarkAmount;
			sampler2D _SafeWallTex;

            fixed4 frag(v2f_img i) : SV_Target {
				return tex2D(_SafeWallTex, i.uv) + fixed4(0, 0, 0, _DarkAmount);
            }

            ENDCG
        }
    }

    FallBack "Diffuse"
}
