using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

public class Tile : MonoBehaviour
{
    Mesh mesh;

    [HideInInspector]
    public Vector3[] vertexes;
    int[] triangles;
    Color[] colors;
    float maxY, minY;
    float noiseFrequency, noiseAmplitude, maxHeight, minHeight;
    [HideInInspector]
    public Vector3 mainPos;
    [HideInInspector]
    public int sizeXtile, sizeZtile;
    HeightColor[] HeightColors;
    Texture2D texture;
    public List<Spawn> spawns;
    int textureDetailMultiplier;

    public BiomeStats biome;

    //TEST
    public Vector2[] uvs;

    public void Initialize(BiomeStats biome, int sizeXtile, int sizeZtile, Vector3 mainPos, int textureDetailMultiplier, bool multiThreading)
    {
        spawns = new List<Spawn>();

        //this.biome = biome;
        this.biome = Biomes.biomes[2]; //Force mountain
        mesh = new Mesh();
        HeightColors = this.biome.color;
        this.mainPos = mainPos;
        this.sizeXtile = sizeXtile;
        this.sizeZtile = sizeZtile;
        this.textureDetailMultiplier = textureDetailMultiplier;

        vertexes = new Vector3[(sizeXtile + 1) * (sizeZtile + 1)];
        triangles = new int[sizeXtile * sizeZtile * 6];
        colors = new Color[(sizeXtile * this.textureDetailMultiplier) * (sizeZtile * this.textureDetailMultiplier)];
        uvs = new Vector2[vertexes.Length];
        noiseAmplitude = this.biome.Amp;
        noiseFrequency = this.biome.Freq;
        maxHeight = this.biome.MaxY;
        minHeight = this.biome.MinY;

        gameObject.GetComponent<MeshFilter>().mesh = mesh;
        gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;

        if (!multiThreading)
        {
            CreateShape(sizeZtile, sizeXtile);
        }
        else
        {
            //CHANGE
            CreateShape(sizeXtile, sizeZtile);
        }


        //Vertextest();
    }

    public void ApplyTexture(Texture2D texture)
    {
        //gameObject.GetComponent<MeshRenderer>().material.EnableKeyword("_NORMALMAP");
        //gameObject.GetComponent<MeshRenderer>().material.EnableKeyword("_METALLICGLOSSMAP");
        texture.filterMode = FilterMode.Point;
        //texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        gameObject.GetComponent<MeshRenderer>().material.mainTexture = texture;
        this.texture = texture;
    }

    public void Initialize(Material mat) //ONLY USE TO REDO TILE FROM PREMADE ONE
    {
        vertexes = new Vector3[vertexes.Length];
        triangles = new int[triangles.Length];
        //colors = new Color[(sizeXtile + 1) * (sizeZtile + 1)];

        gameObject.GetComponent<MeshRenderer>().material = mat;

        CreateShape(sizeZtile, sizeXtile);

        //Vertextest();
    }

    public void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertexes;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        gameObject.GetComponent<MeshCollider>().sharedMesh = null;
        gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    void CreateShape(int sizeZtile, int sizeXtile)
    {
        //creating vertexes

        float seed = UnityEngine.Random.Range(0f, 1f);
        for (int i = 0, z = 0; z <= sizeZtile; z++) //vertices go to 1 more than size
        {
            for (int x = 0; x <= sizeXtile; x++)
            {
                float y = Mathf.PerlinNoise((float)(x * noiseFrequency + seed), (float)(z * noiseFrequency + seed)) * noiseAmplitude;
                y += Mathf.PerlinNoise((float)(x * noiseFrequency + seed), (float)(z * noiseFrequency + seed)) * noiseAmplitude * 0.1f;

                if (y > maxY) //height storage and refinement
                {
                    if (y > maxHeight)
                    {
                        y = maxHeight;
                    }
                    maxY = y;
                }
                else if (y < minY)
                {
                    if (y < minHeight)
                    {
                        y = minHeight;
                    }
                    minY = y;
                }

                vertexes[i] = new Vector3(x, y, z) + mainPos;

                //uvs
                //uvs[i] = new Vector2((float)x / (float)sizeXtile, 1 - (float)z / (float)sizeZtile);
                uvs[i] = new Vector2((1f / (float)sizeXtile * (float)x), (1f / (float)sizeZtile * (float)z));

                i++;
            }
        }

        //creating triangles(clockwise)

        int tris = 0;
        int vert = 0;

        for (int z = 0; z < sizeZtile; z++)
        {
            for (int x = 0; x < sizeXtile; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + sizeXtile + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + sizeXtile + 1;
                triangles[tris + 5] = vert + sizeXtile + 2;
                vert++;
                tris += 6;
            }
            vert++;
        }
    }

    public void ChangeUVs(int xT, int yT)
    {
        for (int i = 0, z = 0; z <= sizeZtile; z++) //vertices go to 1 more than size
        {
            for (int x = 0; x <= sizeXtile; x++)
            {
                uvs[i] = new Vector2(Mathf.Abs(xT - ((float)x / (float)sizeXtile)), Mathf.Abs(yT - ((float)z / (float)sizeZtile)));
                i++;
            }
        }
        ApplyTexture(texture);
        mesh.uv = uvs;
    }

    public void CreateColors()
    {
        float y, height;
        for (int z = 0; z < sizeZtile * textureDetailMultiplier; z++) //vertices go to 1 more than size
        {
            for (int x = 0; x < sizeXtile * textureDetailMultiplier; x++)
            {
                //find height on certain point (works for points not in plane vertexes)
                y = HeightAt(new Vector2((1 / (float)textureDetailMultiplier) * x, (1 / (float)textureDetailMultiplier) * z));

                //Go through colors
                height = -1;
                for (int k = 0; k < HeightColors.Length; k++)
                {
                    if (y >= HeightColors[k].height && height < HeightColors[k].height)
                    {
                        colors[z * (sizeZtile * textureDetailMultiplier) + x] = HeightColors[k].color;
                        height = HeightColors[k].height;
                    }
                }
            }
        }
        ApplyTexture(CreateTextureFromCurrentColors());
    }

    public void MultiThreadingCreateColors()
    {
        NativeArray<float3> originalPointsArray = new NativeArray<float3>(vertexes.Length, Allocator.TempJob);
        for (int i = 0; i < originalPointsArray.Length; i++)
        {
            originalPointsArray[i] = vertexes[i];
        }

        NativeArray<float> resultsArray = new NativeArray<float>((sizeZtile * textureDetailMultiplier) * (sizeXtile * textureDetailMultiplier), Allocator.TempJob);

        VertexHeightJob vertexHeightJob = new VertexHeightJob
        {
            originalPoints = originalPointsArray,
            sizeZtile = sizeZtile,
            detailMultiplier = textureDetailMultiplier,
            results = resultsArray
        };

        JobHandle jobHandle1 = vertexHeightJob.Schedule(resultsArray.Length, 100);
        jobHandle1.Complete();


        //temp variables for jobs
        NativeArray<Color> colorArray = new NativeArray<Color>(colors, Allocator.TempJob);
        NativeArray<float> heightColorsHeightArray = new NativeArray<float>(HeightColors.Length, Allocator.TempJob);
        NativeArray<Color> heightColorsColorArray = new NativeArray<Color>(HeightColors.Length, Allocator.TempJob);

        for (int i = 0; i < HeightColors.Length; i++)
        {
            heightColorsHeightArray[i] = HeightColors[i].height;
            heightColorsColorArray[i] = HeightColors[i].color;
        }

        ColorJob paralelColorJob = new ColorJob
        {
            colors = colorArray,
            heights = resultsArray,
            heightsFromBiome = heightColorsHeightArray,
            colorsFromBiome = heightColorsColorArray
        };

        JobHandle jobHandle = paralelColorJob.Schedule(colors.Length, 100);
        jobHandle.Complete();

        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = colorArray[i];
        }

        colorArray.Dispose();
        heightColorsHeightArray.Dispose();
        heightColorsColorArray.Dispose();

        resultsArray.Dispose();
        originalPointsArray.Dispose();

        ApplyTexture(CreateTextureFromCurrentColors());
    }

    Texture2D CreateTextureFromCurrentColors()
    {
        Texture2D tex = new Texture2D(sizeXtile * textureDetailMultiplier, sizeZtile  * textureDetailMultiplier);

        tex.SetPixels(colors);
        tex.Apply();

        return tex;
    }

    public float HeightAt(Vector2 positionXZ)
    {
        Vector2 bottomLeft = new Vector2(Mathf.Floor(positionXZ.x), Mathf.Floor(positionXZ.y));
        Vector2 bottomRight = new Vector2(Mathf.Floor(positionXZ.x) + 1, Mathf.Floor(positionXZ.y));
        Vector2 topLeft = new Vector2(Mathf.Floor(positionXZ.x), Mathf.Floor(positionXZ.y) + 1);
        Vector2 topRight = new Vector2(Mathf.Floor(positionXZ.x) + 1, Mathf.Floor(positionXZ.y) + 1);

        float xDiff = positionXZ.x - bottomLeft.x;
        float yDiff = positionXZ.y - bottomLeft.y;

        float bottomY = FindPointHeight(bottomLeft) * (1 - xDiff) + FindPointHeight(bottomRight) * xDiff;
        float topY = FindPointHeight(topLeft) * (1 - xDiff) + FindPointHeight(topRight) * xDiff;
        float trueY = bottomY * (1 - yDiff) + topY * yDiff;

        return trueY;
    }

    public float FindPointHeight(Vector2 positionXZ)
    {
        for (int i = 0; i < vertexes.Length; i++)
        {
            if (positionXZ.x == vertexes[i].x && positionXZ.y == vertexes[i].z)
            {
                return vertexes[i].y;
            }
        }
        //Debug.LogError("Failed to find point at: " + positionXZ);
        return 0;
    }

    void Vertextest()
    {
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = Color.red;

        for (int i = 0; i < vertexes.Length; i++)
        {
            Vector3 pos = new Vector3(vertexes[i].x + 0.3f, HeightAt(new Vector2(vertexes[i].x + 0.3f, vertexes[i].z + 0.3f)), vertexes[i].z + 0.3f);
            if (pos != null)
            {
                GameObject test = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                test.transform.localScale = Vector3.one * 0.1f;
                test.transform.position = pos;

                test.GetComponent<Renderer>().material = mat;
            }
        }

        Material mat2 = new Material(Shader.Find("Standard"));
        mat2.color = Color.green;

        for (int i = 0; i < vertexes.Length; i++)
        {
            Vector3 pos = new Vector3(vertexes[i].x, HeightAt(new Vector2(vertexes[i].x, vertexes[i].z)), vertexes[i].z);
            if (pos != null)
            {
                GameObject test = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                test.transform.localScale = Vector3.one * 0.1f;
                test.transform.position = pos;

                test.GetComponent<Renderer>().material = mat2;
            }
        }
    }

    public void TextureTest()
    {
        //Sprite s = Sprite.Create(this.texture, new Rect(0, 0, this.texture.width, this.texture.height), Vector2.zero);
        //GameObject.FindGameObjectWithTag("Test").GetComponent<Image>().sprite = s;
        GameObject.FindGameObjectWithTag("Test").GetComponent<MeshRenderer>().material = gameObject.GetComponent<MeshRenderer>().material;
    }
}