using System;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    public const float maxViewDst = 20;
    [SerializeField] private Transform viewer;
    [SerializeField] private MapGenerator generator;
    [SerializeField] private Transform mapParent;
    
    public static Vector2 viewerPosition;
    private int chunkSize;
    private int chunksVisibleInViewDst;

    private Dictionary<Vector2, TerrainChunk> terrainChunkDic = new Dictionary<Vector2, TerrainChunk>();
    private List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();
    private void Start()
    {
        chunkSize = generator.chunkSize;
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);
        
    }

    private void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisibleChunks();
    }


    private void UpdateVisibleChunks()
    {
        // foreach (var terrainChunk in terrainChunksVisibleLastUpdate)
        // {
        //     terrainChunk.SetVisible(false);
        // }
        // terrainChunksVisibleLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordZ = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for (int zOffset = -chunksVisibleInViewDst; zOffset <= chunksVisibleInViewDst ; zOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordZ + zOffset);

                if (terrainChunkDic.ContainsKey(viewedChunkCoord))
                {
                    terrainChunkDic[viewedChunkCoord].UpdateChunk();
                    if (terrainChunkDic[viewedChunkCoord].IsVisible())
                    {
                        terrainChunksVisibleLastUpdate.Add(terrainChunkDic[viewedChunkCoord]);
                    }
                }
                else
                {
                    terrainChunkDic.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, generator, mapParent));
                }
            }
        }
    }

    public class TerrainChunk
    {
        private MapGenerator generator;
        private Vector2 position;
        private GameObject chunkObject;
        private Bounds bounds;
        
        public TerrainChunk(Vector2 coord, int size, MapGenerator generator, Transform mapParent)
        {
            this.generator = generator;
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);
            chunkObject = new GameObject("Chunk Terrain");
            chunkObject.transform.parent = mapParent;
            chunkObject.transform.position = positionV3;
            // SetVisible(false);
            
            generator.RequestMapData(OnMapDataReceive, position);
        }

        void OnMapDataReceive(MapData mapData)
        {
            List<CombineInstance> blockData = generator.CreateMeshData(mapData.noiseMap);
        
            var blockDataLists = generator.SeparateMeshData(blockData);
        
            generator.CreateMesh(blockDataLists, chunkObject.transform);
        }

        public void UpdateChunk()
        {
            float viewerDistanceFromNearEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            // bool visible = viewerDistanceFromNearEdge <= maxViewDst;
            // SetVisible(visible);
        }

        public void SetVisible(bool visible)
        {
            chunkObject.SetActive(visible);
        }

        public bool IsVisible()
        {
            return chunkObject.activeSelf;
        }
    }
}