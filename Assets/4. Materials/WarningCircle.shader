Shader "Unlit/WarningCircle"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white"{} 
        _TexAlpha("Texture Alpha", Range(0,1)) = 1
        _CenterUV_X("Center UV X", Float) = 0.5 // ���� �߽��� UV ��ǥ
        _CenterUV_Y("Center UV Y", Float) = 0.5 // ���� �߽��� UV ��ǥ
        _Color("Color", Color) = (1,1,1,1)
        _Radius("Radius", Float) = 0.5 // ��ä���� ������
        _Angle("Angle", Float) = 45 // ��ä���� ���� 
    }

        SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
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
            sampler2D _SecondTex;
            float _TexAlpha;
            float _CenterUV_X;
            float _CenterUV_Y;
            float _Radius; 
            float _Angle; 
            fixed4  _Color;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // �ؽ�ó ��ǥ�κ��� �ȼ� ������ �����ɴϴ�.
                fixed4 col = tex2D(_MainTex, i.uv);
                col *= _Color;

                // UV ��ǥ�� ��ä�� ������� ���͸�
                float2 centerToUV = i.uv - float2(_CenterUV_X, _CenterUV_Y);
                float dist = length(centerToUV);
                float angle = atan2(centerToUV.y, centerToUV.x) * 180 / 3.14159;
                angle -= 90;
                float alpha = smoothstep(_Radius - 0.05, _Radius + 0.05f,  dist) * smoothstep(_Angle - 5, _Angle + 5, abs(angle));  // ��ä�� ���� ���� ���� ���

                col *= alpha;
             
                // �� ��° �ؽ�ó�� ù ��° �ؽ�ó�� ��� ��ȯ
                return col;
            }
            ENDCG
        }
    }
}
