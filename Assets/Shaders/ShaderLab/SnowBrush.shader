Shader "Hidden/SnowBrush"
{
    Properties { _MainTex ("Texture", 2D) = "white" {} }
    SubShader
    {
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _BrushParams; 
            float _Erase;

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(float4 vertex : POSITION, float2 uv : TEXCOORD0) {
                v2f o;
                o.pos = UnityObjectToClipPos(vertex);
                o.uv = uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target {
                float current = tex2D(_MainTex, i.uv).r;
                float dist = distance(i.uv, _BrushParams.xy);
                float falloff = saturate(1.0 - (dist / _BrushParams.z));
                float paint = falloff * _BrushParams.w;

                float result = (_Erase > 0.5) ? (current - paint) : (current + paint);
                
                return saturate(result);
            }
            ENDHLSL
        }
    }
}