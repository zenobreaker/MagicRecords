// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.04 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.04;sub:START;pass:START;ps:flbk:,lico:0,lgpr:1,nrmq:1,limd:0,uamb:True,mssp:True,lmpd:False,lprd:False,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:2,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:False,dith:2,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:7480,x:32719,y:32712,varname:node_7480,prsc:2|emission-7773-OUT;n:type:ShaderForge.SFN_Tex2d,id:1621,x:31737,y:32661,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_1621,prsc:2,tex:9192184a1121bec4abd85c91975d3829,ntxv:0,isnm:False|UVIN-5853-OUT;n:type:ShaderForge.SFN_Append,id:8764,x:31159,y:32510,varname:node_8764,prsc:2|A-5468-OUT,B-3194-OUT;n:type:ShaderForge.SFN_Vector1,id:5468,x:30944,y:32521,varname:node_5468,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:3194,x:30944,y:32588,varname:node_3194,prsc:2,v1:1;n:type:ShaderForge.SFN_TexCoord,id:9306,x:31159,y:32369,varname:node_9306,prsc:2,uv:0;n:type:ShaderForge.SFN_Add,id:990,x:31349,y:32510,varname:node_990,prsc:2|A-9306-UVOUT,B-8764-OUT;n:type:ShaderForge.SFN_Add,id:5853,x:31575,y:32628,varname:node_5853,prsc:2|A-990-OUT,B-6455-OUT;n:type:ShaderForge.SFN_Time,id:8939,x:31006,y:32657,varname:node_8939,prsc:2;n:type:ShaderForge.SFN_Multiply,id:3130,x:31202,y:32657,varname:node_3130,prsc:2|A-8939-T,B-7162-OUT;n:type:ShaderForge.SFN_OneMinus,id:6455,x:31387,y:32657,varname:node_6455,prsc:2|IN-3130-OUT;n:type:ShaderForge.SFN_VertexColor,id:4561,x:31922,y:32893,varname:node_4561,prsc:2;n:type:ShaderForge.SFN_Multiply,id:4145,x:31930,y:32739,varname:node_4145,prsc:2|A-1621-RGB,B-4561-RGB;n:type:ShaderForge.SFN_Slider,id:7162,x:30849,y:32806,ptovrint:False,ptlb:TimeScale,ptin:_TimeScale,varname:node_7162,prsc:2,min:0,cur:0.2136752,max:1;n:type:ShaderForge.SFN_Multiply,id:6491,x:32111,y:32687,varname:node_6491,prsc:2|A-7175-OUT,B-4145-OUT;n:type:ShaderForge.SFN_Slider,id:7175,x:31646,y:32498,ptovrint:False,ptlb:Multiply,ptin:_Multiply,varname:node_7175,prsc:2,min:0,cur:0.6837607,max:2;n:type:ShaderForge.SFN_Fresnel,id:5126,x:31747,y:33046,varname:node_5126,prsc:2|EXP-3238-OUT;n:type:ShaderForge.SFN_Power,id:4035,x:31922,y:33046,varname:node_4035,prsc:2|VAL-5126-OUT,EXP-872-OUT;n:type:ShaderForge.SFN_Vector1,id:872,x:31731,y:33211,varname:node_872,prsc:2,v1:3;n:type:ShaderForge.SFN_Add,id:1248,x:32306,y:32799,varname:node_1248,prsc:2|A-6491-OUT,B-7022-OUT;n:type:ShaderForge.SFN_Color,id:9915,x:31943,y:33211,ptovrint:False,ptlb:OutlineColor,ptin:_OutlineColor,varname:node_9915,prsc:2,glob:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:7022,x:32209,y:33007,varname:node_7022,prsc:2|A-4035-OUT,B-9915-RGB;n:type:ShaderForge.SFN_Multiply,id:7773,x:32492,y:32857,varname:node_7773,prsc:2|A-1248-OUT,B-4561-A;n:type:ShaderForge.SFN_Slider,id:3238,x:31418,y:33073,ptovrint:False,ptlb:Outline,ptin:_Outline,varname:node_3238,prsc:2,min:0,cur:1,max:2;proporder:1621-7162-7175-9915-3238;pass:END;sub:END;*/

Shader "My/YPanner_Add" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _TimeScale ("TimeScale", Range(0, 1)) = 0.2136752
        _Multiply ("Multiply", Range(0, 2)) = 0.6837607
        _OutlineColor ("OutlineColor", Color) = (0.5,0.5,0.5,1)
        _Outline ("Outline", Range(0, 2)) = 1
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _TimeScale;
            uniform float _Multiply;
            uniform float4 _OutlineColor;
            uniform float _Outline;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = mul(unity_ObjectToWorld, float4(v.normal,0)).xyz;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float4 node_8939 = _Time + _TimeEditor;
                float2 node_5853 = ((i.uv0+float2(0.0,1.0))+(1.0 - (node_8939.g*_TimeScale)));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_5853, _MainTex));
                float3 emissive = (((_Multiply*(_MainTex_var.rgb*i.vertexColor.rgb))+(pow(pow(1.0-max(0,dot(normalDirection, viewDirection)),_Outline),3.0)*_OutlineColor.rgb))*i.vertexColor.a);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
