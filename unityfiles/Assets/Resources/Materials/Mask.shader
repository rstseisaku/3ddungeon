Shader "Custom/Mask" {
	Properties{
		[HideInInspector] 
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
		_Rule("Rule", 2D) = "white" {}
		_Value("Value", Range(0,1)) = 0.5
		_Mask("Base (RGB) Trans (A)", 2D) = "white" {}
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
	sampler2D _Mask;

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
		float4 rule = tex2D(_Rule, i.uv); // トラジション画像
		float4 mask = tex2D(_Mask, i.uv);
		// if (rule.a < _Cutoff)
		if (rule.r < _Value)
		{
			main.rgb = lerp(main, mask, _Value);
		}
		return main;
	}
		ENDCG
	}
	}
}