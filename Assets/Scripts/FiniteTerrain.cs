using System.Collections.Generic;
using UnityEngine;

public class FiniteTerrain : MonoBehaviour
{
    [SerializeField] private Vector2 previewChunkSize;
    
    private Dictionary<Vector2, TerrainChunk> terrainChunkDic = new Dictionary<Vector2, TerrainChunk>();

    public void GenerateTerrain()
    {
        for (int x = 0; x < previewChunkSize.x; x++)
        {
            for (int y = 0; y < previewChunkSize.y; y++)
            {
                // terrainChunkDic.Add();
            }
        }
    }
}
