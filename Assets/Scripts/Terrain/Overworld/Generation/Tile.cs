using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public BiomeStats biome;

    //TEST
    public Vector2[] uvs;

    public void Initialize(BiomeStats biome, int sizeXtile, int sizeZtile, Vector3 mainPos)
    {
        spawns = new List<Spawn>();

        this.biome = biome;
        mesh = new Mesh();
        HeightColors = biome.color;
        this.mainPos = mainPos;
        this.sizeXtile = sizeXtile;
        this.sizeZtile = sizeZtile;

        vertexes = new Vector3[(sizeXtile + 1) * (sizeZtile + 1)];
        triangles = new int[sizeXtile * sizeZtile * 6];
        colors = new Color[(sizeXtile + 1) * (sizeZtile + 1)];
        uvs = new Vector2[(sizeXtile + 1) * (sizeZtile + 1)];
        noiseAmplitude = biome.Amp;
        noiseFrequency = biome.Freq;
        maxHeight = biome.MaxY;
        minHeight = biome.MinY;

        gameObject.GetComponent<MeshFilter>().mesh = mesh;
        gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;

        CreateShape(sizeZtile, sizeXtile);
        //Vertextest();
    }

    public void ApplyTexture(Texture2D texture)
    {
        //gameObject.GetComponent<MeshRenderer>().material.EnableKeyword("_NORMALMAP");
        //gameObject.GetComponent<MeshRenderer>().material.EnableKeyword("_METALLICGLOSSMAP");
        //texture.filterMode = FilterMode.Point;
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        gameObject.GetComponent<MeshRenderer>().material.mainTexture = texture;
        this.texture = texture;
    }

    public void Initialize(Material mat) //ONLY USE TO REDO TILE FROM PREMADE ONE
    {
        vertexes = new Vector3[(sizeXtile + 1) * (sizeZtile + 1)];
        triangles = new int[sizeXtile * sizeZtile * 6];
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

    public void CreateShape(int sizeZtile, int sizeXtile)
    {
        //creating vertexes

        float seed = Random.Range(0f, 1f);
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

                //CONFIRM
                uvs[i] = new Vector2((float)x / (float)sizeXtile, 1 - (float)z / (float)sizeZtile);

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
        ApplyTexture((Texture2D)gameObject.GetComponent<MeshRenderer>().material.mainTexture);
        mesh.uv = uvs;
    }

    public void CreateColors()
    {
        for (int i = 0, z = 0; z <= sizeZtile; z++) //vertices go to 1 more than size
        {
            for (int x = 0; x <= sizeXtile; x++)
            {
                float y = vertexes[i].y;
                //colors[i] = gradient.Evaluate(Mathf.InverseLerp(minY, maxY, y));
                for (int k = 0; k < HeightColors.Length; k++)
                {
                    if (y >= HeightColors[k].height)
                    {
                        colors[i] = HeightColors[k].color;
                    }
                }
                i++;
            }
        }
        ApplyTexture(CreateTextureFromCurrentColors());
    }

    public Texture2D CreateTextureFromCurrentColors()
    {
        Texture2D tex = new Texture2D(sizeXtile, sizeZtile);
        for (int i = 0; i < sizeZtile; i++)
        {
            for (int j = 0; j < sizeXtile; j++)
            {
                //tex.SetPixel(j, i, colors[j * sizeZtile + i]);
                tex.SetPixels(colors);
                tex.Apply();
                //Debug.Log(colors[j * sizeZtile + i]);
            }
        }
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
        Debug.LogError("Failed to find point at: " + positionXZ);
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