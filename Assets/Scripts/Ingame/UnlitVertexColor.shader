Shader "Custom/UnlitVertexColor" {
    Properties {
        // 이 셰이더는 프로퍼티를 사용하지 않지만, 기본적으로 넣어둡니다.
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc" // 기본적인 유니티 셰이더 유틸리티 포함

            // 정점 셰이더 입력 구조체
            struct appdata {
                float4 vertex : POSITION; // 정점 위치
                float4 color : COLOR;    // 정점 색상
            };

            // 정점 셰이더에서 프래그먼트 셰이더로 넘겨줄 데이터 구조체
            struct v2f {
                float4 vertex : SV_POSITION; // 클립 공간의 정점 위치
                float4 color : TEXCOORD0;   // 보간될 정점 색상
            };

            // 정점 셰이더 함수
            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex); // 정점 위치를 클립 공간으로 변환
                o.color = v.color;                         // 정점 색상을 그대로 전달
                return o;
            }

            // 프래그먼트(픽셀) 셰이더 함수
            fixed4 frag (v2f i) : SV_Target {
                return i.color; // 보간된 정점 색상을 그대로 출력
            }
            ENDCG
        }
    }
}