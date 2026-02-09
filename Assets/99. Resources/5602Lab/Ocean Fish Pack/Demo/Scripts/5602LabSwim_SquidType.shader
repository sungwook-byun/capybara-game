Shader "Custom/5602LabSwim_SquidType" {
	Properties{
		_MainTex("Albedo (RGB)", 2D) = "white" {}


		_Speed("Speed", Range(0, 20)) = 0
		_Frequency("Frequency", Range(0, 10)) = 0
		_Amplitude("Amplitude", Range(0, 1)) = 0


		_HeadPosition("Head Position", Range(-5,  5)) = 0

		_UpDownDistance("UpDownDistance", Range(0, 1)) = 0
		_UpDownSync("UpDownSync", Range(-1, 1)) = 0
	}
		SubShader{
		Tags{  "Queue" = "Transparent" "RenderType"="Transparent" }
		//Tags{ "RenderType" = "Opaque" }

		LOD 200

		
		Cull off
		ZWrite On
		Pass{
		Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM

	#pragma vertex vert Lambert alpha:fade 
	#pragma fragment frag
	#include "UnityCG.cginc"

	sampler2D _MainTex;
	float4 _MainTex_ST;

	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
	};


	float _Amplitude;
	float _Frequency;
	float _Speed;

	float _HeadPosition;

	float _Wave;
	float _UpDownDistance;
	float _UpDownSync;


	v2f vert(appdata_base v)
	{
		v2f o;

		if ( -v.vertex.z < _HeadPosition) {
			if (v.vertex.x > 0) {
				_Wave = sin((v.vertex.z + _Time.y * _Speed) * _Frequency) * _Amplitude;
				v.vertex.x += _Wave;

			}
			else {
				_Wave = sin((v.vertex.z + _Time.y * _Speed) * _Frequency) * _Amplitude;
				v.vertex.x -= _Wave;

			}

		}

		


		_Wave = sin((_Time.y * _Speed + (_Speed*_UpDownSync)) * _Frequency) * _UpDownDistance;
		v.vertex.z += _Wave;



		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
		return o;
		
	}

	fixed4 frag(v2f i) : SV_Target
	{
		return tex2D(_MainTex, i.uv);
	}

		ENDCG

	}
	}
		FallBack "Diffuse"
}