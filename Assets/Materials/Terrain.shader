Shader "Custom/Terrain"
{
    Properties
    {
        _GrassColour ("Grass Colour", Color) = (0,1,0,1)
        _RockColour ("Rock Colour", Color) = (1,1,1,1)
        _GrassFlatnessThreshold ("Grass Flatness Threshold", Range(0,1)) = .5
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        struct Input
        {
            float3 worldPos;
            float3 worldNormal;
        };

        half _MaxHeight;
        half _GrassFlatnessThreshold;
        half _Glossiness;
        half _Metallic;
        fixed4 _GrassColour;
        fixed4 _RockColour;



        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float flatness = IN.worldNormal.y;
            fixed4 c = fixed4(0,0,0,0);
            if (_GrassFlatnessThreshold < flatness) {
                c = _GrassColour;
            }
            else {
                c = _RockColour;
            }
            
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
