using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Generation
{
    [RequireComponent(typeof(Terrain)), ExecuteInEditMode]
    public class TerrainLevelingHandler : MonoBehaviour
    {
        [Header("<size=14><color=#E74C3C>   SETTINGS</color></size>")]
        [Space(3)]
        [Header("<size=13><color=#58D68D>🌍 Terrain Settings</color></size>")]
        [SerializeField] private bool _isRandomleveling;
        [SerializeField] private bool _isTerrainOffset;
        [SerializeField] private float _terrainOffsetSpeed;
        [Header("<size=13><color=#F39C12>⚙️ Leveling Settings</color></size>")]
        [SerializeField] private bool _levelingRealTime;
        [Header("<size=13><color=#5DADE2>📦 Terrain Parameters Settings</color></size>")]
        [SerializeField] private List<TerrainParameters> _terrainParameters;
    
        private Terrain _terrain;
        private TerrainData _terrainData;
        private int _heightmapResolution;
    
        private float _terrainOffsetX;
        private float _terrainOffsetZ;
    
        private void Awake()
        {
            _terrain = GetComponent<Terrain>();
            _terrainData = _terrain.terrainData;
            _heightmapResolution = _terrainData.heightmapResolution;
        }

        private void Start()
        {
            if (!_isRandomleveling) return;
        
            _terrainParameters.Add(new TerrainParameters("coucou", Random.Range(0.1f, 0.9f), Random.Range(-2f, 2f), new Vector2Int(0, 129), new Vector2Int(0, 129), 0));
        }

        void Update()
        {
            if (!_levelingRealTime) return;
        
            float[,] heights = new float[_heightmapResolution, _heightmapResolution];

            if (_isTerrainOffset)
            {
                _terrainOffsetX = transform.position.x / _terrainOffsetSpeed;
                _terrainOffsetZ = transform.position.z / _terrainOffsetSpeed;
            }

            foreach (TerrainParameters parameter in _terrainParameters)
            {
                parameter.ClampPositions(0, _heightmapResolution);
            
                for (int i = parameter.TerrainHeightmapResolutionPositionZ.x; i < parameter.TerrainHeightmapResolutionPositionZ.y; i++)
                {
                    for (int j = parameter.TerrainHeightmapResolutionPositionX.x; j < parameter.TerrainHeightmapResolutionPositionX.y; j++)
                    {
                        float value = Mathf.PerlinNoise((i + parameter.GetStartingPosition() + _terrainOffsetZ) * parameter.Frequency, 
                            (j + parameter.GetStartingPosition() + _terrainOffsetX) * parameter.Frequency);

                        float blendFactor = 1.0f;

                        if (i < parameter.TerrainHeightmapResolutionPositionZ.x + parameter.BlendingSize)
                        {
                            float blendProgress = (float)(i - parameter.TerrainHeightmapResolutionPositionZ.x) / parameter.BlendingSize;
                            blendFactor = Mathf.SmoothStep(0f, 1f, blendProgress);
                        }
                        else if (i > parameter.TerrainHeightmapResolutionPositionZ.y - parameter.BlendingSize)
                        {
                            float blendProgress = (float)(parameter.TerrainHeightmapResolutionPositionZ.y - i) / parameter.BlendingSize;
                            blendFactor = Mathf.SmoothStep(0f, 1f, blendProgress);
                        }

                        if (j < parameter.TerrainHeightmapResolutionPositionX.x + parameter.BlendingSize)
                        {
                            float blendProgress = (float)(j - parameter.TerrainHeightmapResolutionPositionX.x) / parameter.BlendingSize;
                            blendFactor *= Mathf.SmoothStep(0f, 1f, blendProgress);
                        }
                        else if (j > parameter.TerrainHeightmapResolutionPositionX.y - parameter.BlendingSize)
                        {
                            float blendProgress = (float)(parameter.TerrainHeightmapResolutionPositionX.y - j) / parameter.BlendingSize;
                            blendFactor *= Mathf.SmoothStep(0f, 1f, blendProgress);
                        }

                        heights[i, j] += value / _terrainParameters.Count * parameter.Amplitude * blendFactor;
                    }
                }
            }
        
            _terrainData.SetHeights(0, 0, heights);
        }
    }
}