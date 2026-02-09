Shader "Custom/5602LabSwim_FishType" {
	Properties{
		_MainTex("Albedo (RGB)", 2D) = "white" {}

		_SpeedSide("SpeedSide", Range(0, 20)) = 1
		_FrequencySide("FrequencySide", Range(0, 4)) = 1
		_AmplitudeSide("AmplitudeSide", Range(0, 1)) = 0.2


		_SpeedUpSide("SpeedUpSide", Range(0, 20)) = 0
		_FrequencyUpSide("FrequencyUpSide", Range(0, 4)) = 0
		_AmplitudeUpSide("AmplitudeUpSide", Range(0, 1)) = 0


		_HeadPositionX("Head Position X", Range(-5,  5)) = 0
		[Toggle] _HeadWaveOn("HeadWaveOn?", Float) = 0

		_UpDownDistance("UpDownDistance", Range(0, 1)) = 0
		_UpDownSync("UpDownSync", Range(-1, 1)) = 0



	}
		SubShader{
		Tags{  "Queue" = "Transparent" "RenderType" = "Transparent" }
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

	float _AmplitudeSide;
	float _FrequencySide;
	float _SpeedSide;

	float _AmplitudeUpSide;
	float _FrequencyUpSide;
	float _SpeedUpSide;

	float _HeadPositionX;
	float _HeadWaveOn;

	float _UpDownDistance;
	float _UpDownSync;


	float _WaveSide;
	float _WaveUpSide;


	v2f vert(appdata_base v)
	{
		v2f o;




		if (_HeadPositionX > v.vertex.x) { 


			_WaveSide = sin(_Time.y * _SpeedSide + v.vertex.x * _FrequencySide) * _AmplitudeSide;
			v.vertex.x += _WaveSide;


			_WaveUpSide = sin(_Time.y * _SpeedUpSide + v.vertex.x * _FrequencyUpSide) * _AmplitudeUpSide * v.vertex.x;
			v.vertex.z += _WaveUpSide;




		}
		else {

			if (_HeadWaveOn > 0.5) {

				_WaveUpSide = sin(_Time.y * _SpeedUpSide + v.vertex.x * _FrequencyUpSide) * _AmplitudeUpSide * v.vertex.x;
				v.vertex.z += _WaveUpSide;
			}
		}


		_WaveUpSide = sin(_Time.y * _SpeedUpSide + (_SpeedUpSide * _UpDownSync) * _FrequencyUpSide) * _UpDownDistance;
		v.vertex.z += _WaveUpSide;



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