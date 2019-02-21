Shader "Custom/Terrain"
{
    Properties
    {
        _GrassColour ("Grass Colour", Color) = (0,1,0,1)
        _RockColour ("Rock Colour", Color) = (1,1,1,1)
        _GrassSlopeThreshold ("Grass Slope Threshold", Range(0,1)) = .5
        _GrassBlendAmount ("Grass Blend Amount", Range(0,1)) = .5
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
        half _GrassSlopeThreshold;
        half _GrassBlendAmount;

        fixed4 _GrassColour;
        fixed4 _RockColour;

        float inverseLerp(float a, float b, float t) {
            return saturate((t-a)/(b-a));
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float slope = 1-IN.worldNormal.y; // slope = 0 when terrain is completely flat
            float grassWeight = 1-inverseLerp(_GrassSlopeThreshold * (1-_GrassBlendAmount), _GrassSlopeThreshold, slope);
        
            o.Albedo = _GrassColour * grassWeight + _RockColour * (1-grassWeight);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
