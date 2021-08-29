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

    public string generationEquation; //(-y / 64f) + 1f + noiseMap[x, z]
    
    [SerializeField] private int chunkWidth;
    [SerializeField] private int chunkLength;
    [SerializeField] private int chunkHeight;
    [SerializeField] private float threshold = 0;
    public int seed = 1000;
    
    [SerializeField] private Transform mapParent;

    private List<Mesh> meshes = new List<Mesh>();


    [ContextMenu("Generate")]
    public void Generate()
    {
        for (int i = mapParent.childCount - 1; i >= 0; i--)
        {
            Destroy(mapParent.GetChild(i).gameObject);
        }
        
        var worldMap = GetMap();

        List<CombineInstance> blockData = CreateMeshData(worldMap);

        var blockDataLists = SeparateMeshData(blockData);

        CreateMesh(blockDataLists);
    }

    private float[,,] GetMap()
    {
        float[,,] finalMap = new float[chunkWidth, chunkHeight, chunkLength];
        for (int x = 0; x < chunkWidth; x++)
        {
            for (int z = 0; z < chunkLength; z++)
            {
                for (int y = 0; y < chunkHeight; y++)
                {
                    finalMap[x, y, z] = ParseEquation(generationEquation, x,y,z);
                }
            }
        }

        return finalMap;
    }
    
    
    private float ParseEquation(string equation, int x, int y, int z)
    {
        Expression e = new Expression(equation);
        EquationContext context = new EquationContext(seed, x, y, z);
        
        Func<EquationContext, float> f = e.ToLambda<EquationContext, float>();

        return f(context);
    }

    #region Mesh Handling
    
    private List<CombineInstance> CreateMeshData(float[,,] map)
    {
        List<CombineInstance> blockData = new List<CombineInstance>();

        MeshFilter blockMesh = Instantiate(cube, Vector3.zero, Quaternion.identity).GetComponent<MeshFilter>();

        for (int x = 0; x < chunkWidth; x++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                for (int z = 0; z < chunkLength; z++)
                {
                    float noiseValue = map[Mathf.FloorToInt(x), Mathf.FloorToInt(y), Mathf.FloorToInt(z)];

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

class EquationContext
{
    public int x { get; set; }
    public int y { get; set; }
    public int z { get; set; }
    
    FastNoiseLite noise = new FastNoiseLite();

    public EquationContext(int seed, int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        noise.SetSeed(seed);
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        noise.SetFractalType(FastNoiseLite.FractalType.FBm);
        noise.SetFrequency(0.0075f);
        noise.SetFractalOctaves(4);
    }
    
    public float noise2(int x, int y)
    {
        return noise.GetNoise(x, y);
    }
}
