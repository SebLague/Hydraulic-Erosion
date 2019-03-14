Shader "Custom/Terrain2" {
    Properties {
        _GrassColour ("Flat Colour", Color) = (0,1,0,1)
        _GrassSlopeThreshold ("Grass Slope Threshold", Range(0,1)) = .5
        _GrassBlendAmount ("Grass Blend Amount", Range(0,1)) = .5
        _GrassMaxHeight ("Grass Max Height", Range(0,1)) = .5
        _GrassHeightFadePower ("Grass Height Fade", Range(0,6)) = 1

        _MainCol ("Main Colour", Color) = (0,1,0,1)

        _RimColour ("Rim Colour", Color) = (0,1,0,1)
        _RimPower ("Rim Power", Float) = .5
        _RimFac ("Rim Fac", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        struct Input {
            float3 worldPos;
            float3 worldNormal;
            float3 viewDir;
        };

        half _RimFac;
        half _MaxHeight;
        half _RimPower;

        fixed4 _RimColour;
        fixed4 _MainCol;
        fixed4 _GrassColour;

        half _GrassSlopeThreshold;
        half _GrassBlendAmount;
        half _GrassMaxHeight;
        half _GrassHeightFadePower;
        
        void surf (Input IN, inout SurfaceOutputStandard o) {
            float height = IN.worldPos.y;
            float normalizedHeight = height/_MaxHeight;
            float grassGrowHeightPercent = smoothstep(0,_GrassMaxHeight,normalizedHeight);

            float slope = 1-IN.worldNormal.y; // slope = 0 when terrain is completely flat
            float grassBlendHeight = _GrassSlopeThreshold * (1-_GrassBlendAmount);
            float grassWeight = 1-saturate((slope-grassBlendHeight)/(_GrassSlopeThreshold-grassBlendHeight));
            grassWeight *= 1-pow(grassGrowHeightPercent,_GrassHeightFadePower);
            float3 terrainCol = _MainCol * (1-grassWeight) + grassWeight * _GrassColour;
            half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
            float rimWeight =  pow (rim, _RimPower) * _RimFac;
            o.Albedo = _RimColour * rimWeight + terrainCol * (1-rimWeight);
        }
        ENDCG
    }
}
