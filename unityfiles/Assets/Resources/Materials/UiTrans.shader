// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/hogehoge"
{
	Properties
	{
	[HideInInspector] _MainTex("Texture", 2D) = "white" {}
	_BGTex("BGTexture", 2D) = "white" {}
	_Blend("Blend", Range(0,1)) = 0.5
	}
		SubShader
	{
		Blend SrcAlpha OneMinusSrcAlpha
		Tags{ "Queue" = "Transparent" "PreviewType" = "Plane" }

		Pass
	{
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
	sampler2D _BGTex;
	float _Clip;
	float4 _SrcCol;
	float4 _DstCol;
	float _Blend;

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		float4 mask = tex2D(_MainTex, i.uv); // 表示したい画像
		float4 bg = tex2D(_BGTex, i.uv); // トラジション画像
		float4 col = lerp(mask, bg, _Blend); // 画像合成( A ⇒ B に移ろう)
		return col;
	}
	ENDCG
	}
	}
}