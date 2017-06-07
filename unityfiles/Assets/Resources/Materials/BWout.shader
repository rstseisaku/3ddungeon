Shader "Custom/BWout" {
	Properties{
		[HideInInspector] 
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
		_Rule("Rule", 2D) = "white" {}
		_Value("Value", Range(0,1)) = 0.5
		_Blackout("Blackout", Range(0,0.99)) = 0
		_Whiteout("Whiteout", Range(0,0.99)) = 0
		_Albedo("Albedo", Range(0,0.99)) = 1
	}
		SubShader{
			Tags{ "RenderType" = "Opaque" }
			Blend SrcAlpha OneMinusSrcAlpha
			Pass{
			CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

			struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};

		struct v2f
		{
			float2 uv : TEXCOORD0;
			float4 vertex : SV_POSITION;
		};

		sampler2D _MainTex;
		float4 _MainTex_ST;
		sampler2D _Rule;
		float _Clip;
		float4 _SrcCol;
		float4 _DstCol;
		float _Value;
		float _Blackout;
		float _Whiteout;
		float _Albedo;

		v2f vert(appdata v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = TRANSFORM_TEX(v.uv, _MainTex);
			return o;
		}

		fixed4 frag(v2f i) : SV_Target
		{
			float4 main = tex2D(_MainTex, i.uv); // 表示したい画像
			float4 rule = tex2D(_Rule, i.uv); // ルール画像

			/* ルール画像のRGB値をもとにしたい
			 * ┗ルール画像をグレースケールのやつだけに限定できないかしら…？
			 * 最悪R値だけをもとにしても良いのかも
			 */
			// if ( (( rule.r + rule.g + rule.b)/3) < _Cutoff)
			if ( rule.r < _Value)

			// 元のやつ
			// if (rule.a < _Cutoff)
			{
				 main.rgb *= (1 - _Blackout);
				 main.rgb /= (1 - _Whiteout);
				 main.a = _Albedo;
			}

			return main;
		}
		ENDCG
		}
		}
}