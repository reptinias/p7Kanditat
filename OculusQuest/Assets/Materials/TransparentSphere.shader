Shader "Custom/TransparentSphere"
{
    Properties {
	[PerRendererData]_Color ("Color", Color) = (1,0,0,1)
       _Alpha ("Alpha", Range(0,1)) = 0.3
       _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
       _CutOff("Cut off", Range(0,1)) = 0.1
    }
    SubShader {
       Tags { "Queue"="Transparent" "RenderType"="Transparent" }
       Blend SrcAlpha OneMinusSrcAlpha
       AlphaTest Greater 0.1
       Pass {   
          CGPROGRAM 
 
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            
            #include "UnityCG.cginc"
  
          // User-specified uniforms            
          uniform sampler2D _MainTex;
          uniform float _CutOff;
          uniform float _Alpha;   
		          fixed4 _Color;
  
           struct appdata
           {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };            
			struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };
            
            float4 _MainTex_ST;

           v2f vert (appdata v)
           {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
           }
  
          float4 frag(v2f input) : COLOR
          {
  
			 float4 color =tex2D(_MainTex, float2(input.vertex.xy)) * _Color;
             //float4 color = tex2D(_MainTex, float2(input.vertex.xy));   

           // _Alpha = c.a;

            //
            if(color.a < _CutOff) discard;
            else color.a = _Alpha;
            
            // color = Color(color.r, color.g, color.b, 0.2);
            // if(color.a < _CutOff) discard;
            // else (color = Color(color.r, color.g, color.b, 0.2)
             return color;
          }
  
          ENDCG
       }
    }
 }