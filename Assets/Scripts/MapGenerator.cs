using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NCalc;
using UnityEngine;
using Random = System.Random;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject cube;
    [SerializeField] private Material meshMaterial;
    [SerializeField] private GameObject waterPreview;
    public float waterLevel;
    private bool waterOn = true;
    
    public string generationEquation = "(-y/64)+1 + noise2(x, z)"; //(-y / 64f) + 1f + noiseMap[x, z]
    
    public int chunkWidth = 32;
    public int chunkLength = 32;
    [SerializeField] private int chunkHeight;
    public float ChunkHeight { get; }

    [SerializeField] private float threshold = 0;
    
    [SerializeField] private Transform mapParent;

    private List<Mesh> meshes = new List<Mesh>();


    
    public EquationHandler equationHandler = new EquationHandler();

    [ContextMenu("Generate")]
    public void Generate()
    {
        if (equationHandler == null)
        {
            equationHandler = new EquationHandler();
        }
        
        for (int i = mapParent.childCount - 1; i >= 0; i--)
        {
            Destroy(mapParent.GetChild(i).gameObject);
        }
        
        var worldMap = GetMap();

        List<CombineInstance> blockData = CreateMeshData(worldMap);

        var blockDataLists = SeparateMeshData(blockData);

        CreateMesh(blockDataLists);
        
        UpdateWaterPreview();
    }

    private double[,,] GetMap()
    {
        double[,,] finalMap = new double[chunkWidth, chunkHeight, chunkLength];
        for (int x = 0; x < chunkWidth; x++)
        {
            for (int z = 0; z < chunkLength; z++)
            {
                for (int y = 0; y < chunkHeight; y++)
                {
                    finalMap[x, y, z] = equationHandler.ParseEquation(generationEquation, x,y,z);
                }
            }
        }

        return finalMap;
    }

    public void ShowWater()
    {
        waterOn = !waterOn;
        waterPreview.SetActive(waterOn);
    }
    
    public void SetWaterLevel(float level)
    {
        waterLevel = level;
    }
    
    public void UpdateWaterPreview()
    {
        waterPreview.transform.position = new Vector3((chunkWidth / 2), (waterLevel/2), (chunkLength / 2));
        waterPreview.transform.localScale = new Vector3(chunkWidth - 1.01f, waterLevel, chunkLength + .99f);
    }
    

    #region Mesh Handling
    
    private List<CombineInstance> CreateMeshData(double[,,] map)
    {
        List<CombineInstance> blockData = new List<CombineInstance>();

        MeshFilter blockMesh = Instantiate(cube, Vector3.zero, Quaternion.identity).GetComponent<MeshFilter>();

        for (int x = 0; x < chunkWidth; x++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                for (int z = 0; z < chunkLength; z++)
                {
                    double noiseValue = map[Mathf.FloorToInt(x), Mathf.FloorToInt(y), Mathf.FloorToInt(z)];

                    if (noiseValue >= threshold)
                    {
                        blockMesh.transform.position = new Vector3(x, y, z);
                        CombineInstance ci = new CombineInstance()
                        {
                            mesh = blockMesh.sharedMesh,
                            transform = blockMesh.transform.localToWorldMatrix
                        };
                        blockData.Add(ci);
                    }
                }
            }
        }

        DestroyImmediate(blockMesh.gameObject);

        return blockData;
    }

    private List<List<CombineInstance>> SeparateMeshData(List<CombineInstance> blockData)
    {
        List<List<CombineInstance>> blockDataLists = new List<List<CombineInstance>>();
        int vertexCount = 0;
        blockDataLists.Add(new List<CombineInstance>());

        for (int i = 0; i < blockData.Count; i++)
        {
            vertexCount += blockData[i].mesh.vertexCount;
            if (vertexCount > 65536)
            {
                vertexCount = 0;
                blockDataLists.Add(new List<CombineInstance>());
                i--;
            }
            else
            {
                blockDataLists.Last().Add(blockData[i]);
            }
        }

        return blockDataLists;
    }

    private void CreateMesh(List<List<CombineInstance>> blockDataLists)
    {
        foreach (List<CombineInstance> data in blockDataLists)
        {
            GameObject g = new GameObject($"Chunk {blockDataLists.IndexOf(data)}");
            g.transform.parent = mapParent;
            MeshFilter mf = g.AddComponent<MeshFilter>();
            MeshRenderer mr = g.AddComponent<MeshRenderer>();
            mr.sharedMaterial = meshMaterial;
            mf.mesh.CombineMeshes(data.ToArray());

            meshes.Add(mf.mesh);
        }
    }
    
    #endregion
}