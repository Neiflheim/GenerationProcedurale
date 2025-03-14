using System;
using System.Collections.Generic;
using UnityEngine;

namespace Generation
{
    [Serializable]
    internal struct CellularData
    {
        public bool IsOnFire;
        public int NeighborsNumber;
        
        private List<Vector2Int> _neighbors;

        public CellularData(bool isOnFire, int neighborsNumber)
        {
            IsOnFire = isOnFire;
            NeighborsNumber = neighborsNumber;
            
            _neighbors = new List<Vector2Int>
            {
                new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(0, -1)
            };
        }
        
        public int GetNeighborsCount(CellularData[,] matrix, Vector2Int position)
        {
            NeighborsNumber = 0;
            
            foreach (Vector2Int neighbor in _neighbors)
            {
                Vector2Int neighborPosition = position + neighbor;

                if (neighborPosition.x >=  matrix.GetLength(0) || neighborPosition.x <= 0 ||
                    neighborPosition.y >=  matrix.GetLength(0) || neighborPosition.y <= 0) continue;

                if (matrix[neighborPosition.x, neighborPosition.y].IsOnFire)
                {
                    NeighborsNumber++;
                }
            }
            
            return NeighborsNumber;
        }
    }
}