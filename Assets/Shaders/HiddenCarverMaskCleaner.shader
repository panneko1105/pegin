Shader "Hidden/CarverMaskCleaner"
{
    Properties
    {
    }
    SubShader
    {
        Pass
        {
            ColorMask 0
            Cull Back
            ZTest Always
            ZWrite Off
            Stencil
            {
                Ref 0
                WriteMask 1
                Pass Replace
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 vert(float3 vertex : POSITION) : SV_POSITION
            {
                return UnityObjectToClipPos(vertex);
            }

            fixed4 frag() : SV_Target
            {
                return 0;
            }
            ENDCG
        }
    }
}