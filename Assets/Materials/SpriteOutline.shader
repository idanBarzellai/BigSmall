Shader "Custom/SpriteOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineSize ("Outline Size", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            fixed4 _OutlineColor;
            float _OutlineSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 main = tex2D(_MainTex, i.uv) * i.color;

                float2 offset = _MainTex_TexelSize.xy * _OutlineSize;

                float alpha = 0;
                alpha += tex2D(_MainTex, i.uv + float2(offset.x, 0)).a;
                alpha += tex2D(_MainTex, i.uv + float2(-offset.x, 0)).a;
                alpha += tex2D(_MainTex, i.uv + float2(0, offset.y)).a;
                alpha += tex2D(_MainTex, i.uv + float2(0, -offset.y)).a;

                if (main.a <= 0 && alpha > 0)
                    return _OutlineColor;

                return main;
            }
            ENDCG
        }
    }
}