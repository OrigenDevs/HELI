Shader "HELI/SpriteCrop"
{
    Properties {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _CropLeft ("Crop Left", Range(0,1)) = 0.0
        _CropRight ("Crop Right", Range(0,1)) = 0.0
        _CropTop ("Crop Top", Range(0,1)) = 0.0
        _CropBottom ("Crop Bottom", Range(0,1)) = 0.0
    }
    
    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        
        Pass {
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };
            
            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };
            
            v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }
            
            sampler2D _MainTex;
            fixed4 _Color;
            float _CropLeft, _CropRight, _CropTop, _CropBottom;
            
            fixed4 frag(v2f i) : SV_Target {
                float2 uv = i.uv;
                
                // True crop - just discard pixels outside crop region
                if (uv.x < _CropLeft || uv.x > (1.0 - _CropRight) || 
                    uv.y < _CropBottom || uv.y > (1.0 - _CropTop)) {
                    discard;
                }
                
                fixed4 col = tex2D(_MainTex, uv) * _Color;
                col *= i.color;
                return col;
            }
            ENDCG
        }
    }
    
    Fallback "Sprites/Default"
}