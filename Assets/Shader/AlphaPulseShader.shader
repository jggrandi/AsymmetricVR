 Shader "Custom/AlphaPulseShader" {
     Properties{
         _MainTex ("Base (RGB) Trans (A)", 2D) = "blue" {}
         _BumpMap ("Normalmap", 2D) = "bump" {}
         _EmissionMap ("Emission Map", 2D) = "black" {}
         _Color ("Color", Color) = (1,0,0,1)
         _PDistance ("PulseDistance", Float) = 0
         _PFadeDistance ("FadeDistance", Float) = 10
         _PEdgeSoftness ("EdgeSoftness", Float) = 5
         _Origin ("PulseOrigin", Vector) = (0, 0, 0, 0)
 
         _RimColor ("Rim Color", Color) = (1,1,1,1)
         _RimPower ("Rim Power", Range(0.5, 8.0)) = 1.059702
         [Toggle(USE_RIM)]_RimOn ("Rim On", Int) = 0.0
         _EmissionMultiplier ("Emission Multiplier", Range(-10,10)) = 1.0
     }
 
     SubShader{
         Tags{ "RenderType"="Opaque" }
         LOD 200
 
         CGPROGRAM
         //LIGHTING
         #include "AutoLight.cginc"
         // define finalcolor and vertex programs:
         #pragma surface surf Lambert finalcolor:mycolor// alpha
 
         #pragma shader_feature USE_RIM
 
         struct Input {
             float2 uv_MainTex;
             float2 uv_BumpMap;
             float2 uv_EmissionMap;
             float3 worldPos;
             float3 viewDir;
             half alpha;
         };
 
         fixed4 _Color;
         sampler2D _MainTex;
         sampler2D _BumpMap;
         sampler2D _EmissionMap;
         half _PDistance;
         half _PFadeDistance;
         half _PEdgeSoftness;
         float4 _Origin;
 
         float4 _RimColor;
         float _RimPower;
         float _RimOn;
         float _EmissionMultiplier;
 
         void mycolor (Input IN, SurfaceOutput o, inout fixed4 color) {
             // set the vertex color alpha to the value calculated: 
             fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
             color.a = c.a;
             half dis;
             half interval;
             half origin;
             dis = sqrt (pow ((IN.worldPos.x - _Origin.x),2) + pow ((IN.worldPos.y - _Origin.y), 2) + pow ((IN.worldPos.z - _Origin.z), 2));
 
             if (dis >= _PDistance)
             {
                 color.rgb = color.rgb * (1 - saturate (abs (_PDistance - dis) / _PEdgeSoftness));
             }
             else
             {
                 color.rgb = color.rgb * (1 - saturate (abs (_PDistance - dis) / _PFadeDistance));
             }
 
         }
 
         void surf (Input IN, inout SurfaceOutput o) {
             // simply copy the corresponding texture element color:
             fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
             o.Albedo = c.rgb;
             // o.Alpha = tex2D(_MainTex,IN.uv_MainTex).a;
             o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
             //  half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
             #ifdef USE_RIM
                 half rim = 1.0 - saturate (dot (normalize (IN.viewDir), o.Normal));
                 o.Emission = _RimColor.rgb * pow (rim, _RimPower) + _EmissionMultiplier * tex2D (_EmissionMap, IN.uv_EmissionMap);
             #endif
         }
         ENDCG
     }
     // Fallback "Diffuse"
     Fallback "VertexLit"
 }