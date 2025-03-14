using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Generation
{
    [RequireComponent(typeof(Terrain)), ExecuteInEditMode]
    public class TerrainPropsSpawnerHandler : MonoBehaviour
    {
        [Header("<size=14><color=#E74C3C>   SETTINGS</color></size>")]
        [Space(3)]
        [Header("<size=13><color=#58D68D>🌍 Terrain Settings</color></size>")]
        [SerializeField] private bool _isTerrainOffset;
        [SerializeField] private bool _isRandomSpawn;
        [SerializeField] private float _terrainOffsetSpeed;
        [Header("<size=13><color=#5DADE2>🌱 Seed & Noise Settings</color></size>")]
        [SerializeField] private string _seed;
        [SerializeField, Range(0f, 0.5f)] private float _frequency;
        [SerializeField] private float _minHeight;
        [SerializeField] private float _maxHeight;
        [SerializeField, Range(1f, 8f)] private float _density;
        [Header("<size=13><color=#F5B041>📦 Object Prefabs Settings</color></size>")]
        [SerializeField] private Transform _parent;
        [SerializeField] private List<GameObject> _objectPrefabs = new List<GameObject>();
    
        private Terrain _terrain;
        private TerrainData _terrainData;
        private int _heightmapResolution;
    
        private int _startingPosition;
        private float _terrainOffsetX;
        private float _terrainOffsetZ;
    
        private List<GameObject> _spawnObjects = new List<GameObject>();
        private Random _random;
    
        private Vector3 _lastTerrainPosition;
        private bool _lastIsRandomSpawn;
        private string _lastSeed;
        private float _lastFrequency;
        private float _lastMinHeight;
        private float _lastMaxHeight;
        private float _lastSoilFertilityRate;
        private float _lastTerrainOffsetSpeed;

        private void Start()
        {
            _terrain = GetComponent<Terrain>();
            _terrainData = _terrain.terrainData;
            _heightmapResolution = _terrainData.heightmapResolution;
        
            _startingPosition = Mathf.Abs(_seed.GetHashCode()) / 10000;
        
            _lastIsRandomSpawn = !_isRandomSpawn;
            
            _lastTerrainPosition = _terrain.transform.position;
            _lastIsRandomSpawn = _isRandomSpawn;
            _lastSeed = _seed;
            _lastFrequency = _frequency;
            _lastMinHeight = _minHeight;
            _lastMaxHeight = _maxHeight;
            _lastSoilFertilityRate = _density;
            _lastTerrainOffsetSpeed = _terrainOffsetSpeed;
        }

        void Update()
        {
            if (_lastTerrainPosition == transform.position && _lastIsRandomSpawn == _isRandomSpawn && _lastSeed == _seed && Mathf.Approximately(_lastFrequency, _frequency)
                && Mathf.Approximately(_lastMinHeight, _minHeight) && Mathf.Approximately(_lastMaxHeight, _maxHeight) && Mathf.Approximately(_lastSoilFertilityRate, _density)
                && Mathf.Approximately(_lastTerrainOffsetSpeed, _terrainOffsetSpeed)) return;
            
            Debug.Log("Spawn");
        
            foreach (GameObject obj in _spawnObjects)
            {
                DestroyImmediate(obj);
            }
        
            if (_isTerrainOffset)
            {
                _terrainOffsetX = transform.position.x / _terrainOffsetSpeed;
                _terrainOffsetZ = transform.position.z / _terrainOffsetSpeed;
            }
       
            _random = new Random(_startingPosition);
        
            for (int i = 0; i < _heightmapResolution; i++)
            {
                for (int j = 0; j < _heightmapResolution; j++)
                {
                    float value = Mathf.PerlinNoise((i + _startingPosition + _terrainOffsetZ) * _frequency, (j + _startingPosition + _terrainOffsetX) * _frequency);

                    if (_isRandomSpawn)
                    {
                        float soilFertilityValue = value * _density;

                        for (int k = 1; k <= soilFertilityValue; k++)
                        {
                            float valueX = i + (float) _random.Next(-5, 5) / 10;
                            float valueY = j + (float) _random.Next(-5, 5) / 10;
                    
                            Vector3 position = HeightmapToWorldPosition(valueX, valueY);
                    
                            if (position.y < _minHeight || position.y > _maxHeight 
                                                        || position.x > _terrainData.size.x + _terrain.transform.position.x || position.x < _terrain.transform.position.x
                                                        || position.z > _terrainData.size.z + _terrain.transform.position.z || position.z < _terrain.transform.position.z) continue;
                    
                            GameObject newTree = Instantiate(_objectPrefabs[_random.Next(0, _objectPrefabs.Count)], position, Quaternion.Euler(0, _random.Next(-180, 180), 0), _parent);
                            _spawnObjects.Add(newTree);
                            Debug.Log("Nombre d'arbres instantié : " + _spawnObjects.Count);
                        }
                    }
                    else
                    {
                        Vector3 position = HeightmapToWorldPosition(i, j);
                    
                        if (position.y < _minHeight || position.y > _maxHeight || value > (_density - 1) / (8 - 1)
                            || position.x > _terrainData.size.x + _terrain.transform.position.x || position.x < _terrain.transform.position.x
                            || position.z > _terrainData.size.z + _terrain.transform.position.z || position.z < _terrain.transform.position.z) continue;
                    
                        GameObject newTree = Instantiate(_objectPrefabs[_random.Next(0, _objectPrefabs.Count)], position, Quaternion.identity);
                        _spawnObjects.Add(newTree);
                    }
                }
            }
        
            _lastTerrainPosition = _terrain.transform.position;
            _lastIsRandomSpawn = _isRandomSpawn;
            _lastSeed = _seed;
            _lastFrequency = _frequency;
            _lastMinHeight = _minHeight;
            _lastMaxHeight = _maxHeight;
            _lastSoilFertilityRate = _density;
            _lastTerrainOffsetSpeed = _terrainOffsetSpeed;
        }
    
        private Vector3 HeightmapToWorldPosition(float heightmapX, float heightmapY)
        {
            float worldX = _terrain.transform.position.x + heightmapX / _terrainData.heightmapResolution * _terrainData.size.x;
            float worldZ = _terrain.transform.position.z + heightmapY / _terrainData.heightmapResolution * _terrainData.size.z;
            float worldY = _terrain.transform.position.y + _terrain.SampleHeight(new Vector3(worldX, 0, worldZ));

            return new Vector3(worldX, worldY, worldZ);
        }
    }
}