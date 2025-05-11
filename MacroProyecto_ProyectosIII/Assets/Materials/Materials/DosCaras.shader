Shader "Custom/UnlitDoubleSided" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader {
        Tags { "Queue"="Geometry" }
        Lighting Off
        Cull Off
        ZWrite On
        ZTest LEqual
        Pass {
            SetTexture [_MainTex] {
                combine texture * primary
            }
        }
    }
}

