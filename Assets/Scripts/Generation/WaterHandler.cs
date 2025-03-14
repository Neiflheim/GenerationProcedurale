using UnityEngine;

namespace Generation
{
    [RequireComponent(typeof(Terrain)), ExecuteInEditMode]
    public class WaterHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TerrainData _terrainData;
    
        [Header("Water Settings")]
        [SerializeField] private int _height;
    
        private void Update()
        {
            int positionX = Mathf.RoundToInt(_terrainData.size.x / 2);
            int positionZ = Mathf.RoundToInt(_terrainData.size.z / 2);
            transform.position = new Vector3(positionX, _height, positionZ);
        
            int scaleX = Mathf.RoundToInt(_terrainData.size.x / 10);
            int scaleZ = Mathf.RoundToInt(_terrainData.size.z / 10);
            transform.localScale = new Vector3(scaleX, 1, scaleZ);
            
            Vector3 position = transform.position;
            position.y = _height;
            transform.position = position;
        }
    }
}