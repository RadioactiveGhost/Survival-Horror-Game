using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TerrainGenerator : MonoBehaviour
{
    public NavMeshSurface surface;

    List<GameObject> map;

    Material mat;
    public Texture2D texture;

    public int sizeXtile, sizeZtile, mapSizeX, mapSizeY;
    //public float noiseFrequency, noiseAmplitude;
    bool first;

    Spawns spawnManager;

    void Awake()
    {
        first = true;
        mat = new Material(Shader.Find("Standard"));
        mat.color = Color.green;

        map = new List<GameObject>();

        //GenMap();
        //test();
    }

    private void Start()
    {
        spawnManager = new Spawns();
        GenMap();
        surface.BuildNavMesh();
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().SetPlayerInitialPos();
        foreach(GameObject g in map)
        {
            spawnManager.Populate(g.GetComponent<Tile>());
           
        }
        
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GenMap();
        }
    }

    void test()
    {
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        Tile t = plane.AddComponent<Tile>();
        Debug.Log(t);
        //t.Initialize(mat, sizeXtile, sizeZtile, Vector3.zero, noiseFrequency, noiseAmplitude);
    }

    void GenMap()
    {
        if (mapSizeY == 0 || mapSizeX == 0 || sizeXtile == 0 || sizeZtile == 0)
        {
            Debug.LogError("Map and tiles can't be with any size 0!");
            return;
        }
        if (first) //Create map
        {
            for (int i = 0; i < mapSizeY; i++) //Y
            {
                for (int j = 0; j < mapSizeX; j++) //X
                {
                    //Choose Biome pure random, CHANGE
                    BiomeStats b = Biomes.biomes[Random.Range(0, Biomes.biomes.Count)]; //max exclusive

                    //Create tile
                    GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    plane.name = (i * mapSizeY + j).ToString() + " " + b.biome.ToString();
                    Tile t = plane.AddComponent<Tile>();
                    t.Initialize(b, sizeXtile, sizeZtile, new Vector3(j * sizeXtile, 0, i * sizeZtile));
                    map.Add(plane);

                    Connect(j, i, t);

                    //Testing Purposes
                    //t.TextureTest();
                }
            }
            foreach(GameObject g in map)
            {
                g.GetComponent<Tile>().UpdateMesh();
            }
            first = false;
        }
        else //Just update map, ignores editor
        {
            int j = 0, i = 0;
            foreach (GameObject g in map)
            {
                if (i == mapSizeY)
                {
                    Debug.LogError("Something wrong isn't right");
                }
                Tile t = g.GetComponent<Tile>();
                t.Initialize(GetRandomMat());
                t.UpdateMesh();
                Connect(j, i, t);
                j++;
                if (j == mapSizeX)
                {
                    j = 0;
                    i++;
                }
            }
        }
    }

    void Connect(int j, int i, Tile t)
    {
        //Connect and smooth tile to left
        if (j != 0)
        {
            //left tile
            Tile lastVerts = map[i * mapSizeY + j - 1].GetComponent<Tile>();
            for (int k = 0; k < sizeZtile + 1; k++)//tiles are size+1, ignore corners
            {
                if ((k == 0 && i != 0) || (k == sizeZtile && i != mapSizeY - 1))//corners with more tiles
                {
                    if (i != 0 && k == 0)
                    {
                        Tile bottomLeft = map[(i - 1) * mapSizeY + j - 1].GetComponent<Tile>();
                        Tile bottom = map[(i - 1) * mapSizeY + j].GetComponent<Tile>();

                        Vector3 current = t.vertexes[sizeXtile * k + k];
                        Vector3 left = lastVerts.vertexes[k * sizeXtile + k + sizeXtile];

                        Vector3 currentBottom = bottom.vertexes[bottom.vertexes.Length - 1 - sizeXtile];
                        Vector3 leftBottom = bottomLeft.vertexes[bottomLeft.vertexes.Length - 1];

                        Vector3 result = mediaHeightOne(current, left, currentBottom, leftBottom);

                        t.vertexes[sizeXtile * k + k] = result;
                        lastVerts.vertexes[k * sizeXtile + k + sizeXtile] = result;
                        bottom.vertexes[bottomLeft.vertexes.Length - 1 - sizeXtile] = result;
                        bottomLeft.vertexes[bottom.vertexes.Length - 1] = result;
                    }

                    continue;//skip loop (doesn't do code below)
                }

                //left side of current connects to right side of left tile
                Vector3 height = mediaHeightOne(t.vertexes[sizeXtile * k + k], lastVerts.vertexes[k * sizeXtile + k + sizeXtile]);

                t.vertexes[sizeXtile * k + k] = height;
                lastVerts.vertexes[k * sizeXtile + k + sizeXtile] = height;

                //TestPoint(t.vertexes[sizeXtile * k + k]);
            }
        }

        //Connect and smooth tile to bottom
        if (i != 0)
        {
            //bottom tile
            Vector3[] lastVerts = map[(i - 1) * mapSizeY + j].GetComponent<Tile>().vertexes;
            for (int l = 0; l < sizeXtile + 1; l++)//tiles are size+1, ignore corners
            {
                if ((l == 0 && j != 0) || (l == sizeZtile && j != mapSizeY - 1))
                {
                    continue; //skip this loop
                }
                //bottom of current connects to top of bottom tile
                Vector3 height = mediaHeightOne(t.vertexes[l], lastVerts[(sizeXtile + 1) * (sizeZtile) + l]);

                t.vertexes[l] = height;
                map[(i - 1) * mapSizeY + j].GetComponent<Tile>().vertexes[(sizeXtile + 1) * (sizeZtile) + l] = height;
            }
        }
    }

    public Vector3 mediaHeightOne(Vector3 numOne, Vector3 numTwo)
    {
        float media = (numOne.y + numTwo.y) / 2;
        return new Vector3(numOne.x, media, numOne.z);
    }

    public Vector3 mediaHeightOne(Vector3 numOne, Vector3 numTwo, Vector3 numThree, Vector3 numFour)
    {
        float media = (numOne.y + numTwo.y + numThree.y + numFour.y) / 4;
        return new Vector3(numOne.x, media, numOne.z);
    }

    public Material GetRandomMat()
    {
        Material rndColor = new Material(Shader.Find("Standard"));
        rndColor.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        return rndColor;
    }

    public void TestPoint(Vector3 point)
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        g.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        g.transform.position = point;
        g.GetComponent<Renderer>().material.color = Color.red;
    }

    public void TestPoint(Vector3 point, Color col, string name)
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        g.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        g.transform.position = point;
        g.name = name;
        g.GetComponent<Renderer>().material.color = col;
    }

    public float HeightAt(Vector2 pos)
    {
        for (int i = 0; i < map.Count; i++)
        {
            if (new Rect(new Vector2(map[i].GetComponent<Tile>().mainPos.x, map[i].GetComponent<Tile>().mainPos.z), new Vector2(sizeXtile, sizeZtile)).Contains(pos))
            {
                return map[i].GetComponent<Tile>().HeightAt(pos);
            }
        }
        return 0;
    }
}