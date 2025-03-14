using System;
using UnityEngine;

namespace Generation
{
    [Serializable]
    internal struct TerrainParameters
    {
        [Header("<size=14><color=#E74C3C>   SETTINGS</color></size>")]
        [Space(3)]
        [Header("<color=#58D68D>🌱 Seed & Noise Settings</color>")]
        public string Seed;
        [Range(0f, 1f)] public float Frequency;
        [Range(-2f, 2f)] public float Amplitude;
        [Header("<color=#5DADE2>🌍 Terrain Position Settings</color>")]
        public Vector2Int TerrainHeightmapResolutionPositionZ;
        public Vector2Int TerrainHeightmapResolutionPositionX;
        [Header("<color=#F5B041>🎨 Blending Settings</color>")]
        public int BlendingSize;

        public TerrainParameters(string seed, float frequency, float amplitude,Vector2Int terrainHeightmapResolutionPositionZ, Vector2Int terrainHeightmapResolutionPositionX, int blendingSize)
        {
            Seed = seed;
            Frequency = frequency;
            Amplitude = amplitude;
            TerrainHeightmapResolutionPositionZ = terrainHeightmapResolutionPositionZ;
            TerrainHeightmapResolutionPositionX = terrainHeightmapResolutionPositionX;
            BlendingSize = blendingSize;
        }
    
        public int GetStartingPosition()
        {
            return Mathf.Abs(Seed.GetHashCode()) / 10000;
        }

        public void ClampPositions(int min, int max)
        {
            TerrainHeightmapResolutionPositionX.x = Mathf.Clamp(TerrainHeightmapResolutionPositionX.x, min, max);
            TerrainHeightmapResolutionPositionX.y = Mathf.Clamp(TerrainHeightmapResolutionPositionX.y, min, max);
            TerrainHeightmapResolutionPositionZ.x = Mathf.Clamp(TerrainHeightmapResolutionPositionZ.x, min, max);
            TerrainHeightmapResolutionPositionZ.y = Mathf.Clamp(TerrainHeightmapResolutionPositionZ.y, min, max);
        }
    }
}