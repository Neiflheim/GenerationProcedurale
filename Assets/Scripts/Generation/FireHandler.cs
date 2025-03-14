using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generation
{
    public class FireHandler : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Vector2Int _startingPosition;
        [SerializeField] private float _propagationSpeed;
        [SerializeField] private int _minHeight;
        [SerializeField] private GameObject _firePrefab;
        
        // Trees
        private GameObject[] _trees;
        private HashSet<GameObject>[,] _treesMatrix;
        
        // Automata Cellular
        private CellularData[,] _cellularMatrixData;
        
        // Terrain
        private Terrain _terrain;
        private TerrainData _terrainData;
        private int _heightmapResolution;
        
        // Coroutine
        private IEnumerator _coroutine;
        
        // Debug
        private List<GameObject> _fireObjects = new List<GameObject>();

        private void Awake()
        {
            _coroutine = Coroutine(_propagationSpeed);
        }

        private void Start()
        {
            // Terrain
            _terrain = GetComponent<Terrain>();
            _terrainData = _terrain.terrainData;
            _heightmapResolution = _terrainData.heightmapResolution;
            
            // Initialize matrix
            _treesMatrix = new HashSet<GameObject>[_heightmapResolution, _heightmapResolution];
            _cellularMatrixData = new CellularData[_heightmapResolution,_heightmapResolution];
            
            for (int i = 0; i < _heightmapResolution; i++)
            {
                for (int j = 0; j < _heightmapResolution; j++)
                {
                    // Tree matrix
                    _treesMatrix[i, j] = new HashSet<GameObject>();
                    
                    // Cellular matrix
                    if (new Vector2Int(i, j) == _startingPosition)
                    {
                        _cellularMatrixData[i, j] = new CellularData(true, 0);
                    }
                    else
                    {
                        _cellularMatrixData[i, j] = new CellularData(false, 0);
                    }
                }
            }
            
            // Get trees and add to tree matrix
            _trees = GameObject.FindGameObjectsWithTag("Tree");
            Debug.Log("Nombre d'arbres trouvé : " + _trees.Length);

            foreach (GameObject tree in _trees)
            {
                Vector3 position = tree.transform.position;
                
                // World position to heightmapResolution position
                int heightmapX = Mathf.RoundToInt((position.x - _terrain.transform.position.x) / _terrainData.size.x * _heightmapResolution);
                int heightmapY = Mathf.RoundToInt((position.z - _terrain.transform.position.z) / _terrainData.size.z * _heightmapResolution);
                
                _treesMatrix[heightmapX, heightmapY].Add(tree);
            }
            
            // Start propagation
            StartCoroutine(_coroutine);
        }

        private IEnumerator Coroutine(float delay)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);
                // Clone la matrice et fait les calculs de qui reste onfire en fonction de l'original puis l'applique sur le clone et remplace l'originale par le clone
                Debug.Log("NextStep");
                CellularData[,] cellularMatrixDataClone = (CellularData[,]) _cellularMatrixData.Clone();
            
                for (int i = 0; i < _heightmapResolution; i++)
                {
                    for (int j = 0; j < _heightmapResolution; j++)
                    {
                        int neighborsNumber = _cellularMatrixData[i, j].GetNeighborsCount(_cellularMatrixData, new Vector2Int(i, j));
                        
                        int height = (int)HeightmapToWorldPosition(i, j).y;

                        if (height >= _minHeight && neighborsNumber == 1 || height >= _minHeight && neighborsNumber == 2)
                        {
                            cellularMatrixDataClone[i, j] = new CellularData(true, 0);
                        }
                        else
                        {
                            cellularMatrixDataClone[i, j] = new CellularData(false, 0);
                        }
                    }
                }
            
                _cellularMatrixData = cellularMatrixDataClone;
            
                SetFire();
            }
        }
        
        private void SetFire()
        {
            foreach (GameObject fireObject in _fireObjects)
            {
                Destroy(fireObject);
            }
            _fireObjects.Clear();
            
            for (int i = 0; i < _heightmapResolution; i++)
            {
                for (int j = 0; j < _heightmapResolution; j++)
                {
                    if (_cellularMatrixData[i, j].IsOnFire)
                    {
                        Vector3 position = HeightmapToWorldPosition(i, j);
                        GameObject newFire = Instantiate(_firePrefab, new Vector3(position.x, 500, position.z), Quaternion.identity);
                        _fireObjects.Add(newFire);
                        
                        foreach (GameObject obj in _treesMatrix[i, j])
                        {
                            DestroyImmediate(obj);
                        }
                        _treesMatrix[i, j].Clear();
                    }
                }
            }
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