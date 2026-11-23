Shader "UmaSurvival/PopupBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _BlurDirection;

            fixed4 frag(v2f_img input) : SV_Target
            {
                float2 offset = _MainTex_TexelSize.xy * _BlurDirection.xy;

                fixed4 color = tex2D(_MainTex, input.uv) * 0.2270270270;
                color += tex2D(_MainTex, input.uv + offset * 1.3846153846) * 0.3162162162;
                color += tex2D(_MainTex, input.uv - offset * 1.3846153846) * 0.3162162162;
                color += tex2D(_MainTex, input.uv + offset * 3.2307692308) * 0.0702702703;
                color += tex2D(_MainTex, input.uv - offset * 3.2307692308) * 0.0702702703;
                color.a = 1;
                return color;
            }
            ENDCG
        }
    }
}
