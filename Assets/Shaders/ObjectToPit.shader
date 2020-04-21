Shader "Custom/ObjectToPit"
{
    //Properties
    //{
    //    _Color ("Color", Color) = (1,1,1,1)
    //    _MainTex ("Albedo (RGB)", 2D) = "white" {}
    //    _Glossiness ("Smoothness", Range(0,1)) = 0.5
    //    _Metallic ("Metallic", Range(0,1)) = 0.0
    //}

	Properties{
		_Colour("Totally Rad Colour!", Color) = (1, 1, 1, 1)
		//_MainTex("Base (RGB)", 2D) = "white" {}
	}
	SubShader{
		Tags { "RenderType" = "Opaque" }

		Stencil {
			Ref 1
			Comp NotEqual
			Pass keep
		}


		CGPROGRAM
		#pragma surface surf Lambert


		sampler2D _MainTex;


		struct Input {
			float2 uv_MainTex;
		};


		void surf(Input IN, inout SurfaceOutput o) {
			half4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}

//#pragma vertex vertexFunction
//#pragma fragment fragmentFunction
//
#include "UnityCG.cginc"
//
		struct appdata {
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};

		struct v2f {
			float4 position : SV_POSITION;
			float2 uv : TEXCOORD0;
		};

		// ★追加
		float4 _Colour;

		v2f vertexFunction(appdata IN) {
			v2f OUT;
			OUT.position = UnityObjectToClipPos(IN.vertex);
			return OUT;
		}

		fixed4 fragmentFunction(v2f IN) : SV_TARGET{
			return fixed4(0, 1, 0, 1);
		}

		ENDCG
	}
    //SubShader
    //{
    //    Tags { "RenderType"="Opaque" }
    //    LOD 200

    //    CGPROGRAM
    //    // Physically based Standard lighting model, and enable shadows on all light types
    //    #pragma surface surf Standard fullforwardshadows

    //    // Use shader model 3.0 target, to get nicer looking lighting
    //    #pragma target 3.0

    //    sampler2D _MainTex;

    //    struct Input
    //    {
    //        float2 uv_MainTex;
    //    };

    //    half _Glossiness;
    //    half _Metallic;
    //    fixed4 _Color;

    //    // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
    //    // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
    //    // #pragma instancing_options assumeuniformscaling
    //    UNITY_INSTANCING_BUFFER_START(Props)
    //        // put more per-instance properties here
    //    UNITY_INSTANCING_BUFFER_END(Props)

    //    void surf (Input IN, inout SurfaceOutputStandard o)
    //    {
    //        // Albedo comes from a texture tinted by color
    //        fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
    //        o.Albedo = c.rgb;
    //        // Metallic and smoothness come from slider variables
    //        o.Metallic = _Metallic;
    //        o.Smoothness = _Glossiness;
    //        o.Alpha = c.a;
    //    }
    //    ENDCG
    //}
    FallBack "Diffuse"
}
