using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using UnityEngine.Profiling;

public class TerrainGenerator : MonoBehaviour
{
    public NavMeshSurface surface;

    [HideInInspector]
    public List<GameObject> map;
    [HideInInspector]
    public List<GameObject> decoys;

    //Material mat;
    //public Texture2D texture;

    public int sizeXtile, sizeZtile, mapSizeX, mapSizeY, textureDetailMultiplier;
    //public float noiseFrequency, noiseAmplitude;
    bool first;

    Spawns spawnManager;

    //TEXTURE DEBUGGING STUFF
    //public int uvX, uvY;

    //DEBUGGING STUFF
    //private DateTime startTime;
    //TimeSpan elapsedTime;

    public bool multiThreading;

    void Awake()
    {
        first = true;
        //mat = new Material(Shader.Find("Standard"));
        //mat.color = Color.green;

        map = new List<GameObject>();
        decoys = new List<GameObject>();
    }

    private void Start()
    {
        //setting start timer
        //startTime = System.DateTime.Now;
        //Profiler.BeginSample("Terrain generation");

        //generate map
        GenMap();

        //Debugging time count...
        //Profiler.EndSample();
        //elapsedTime = System.DateTime.Now - startTime;
        //Debug.Log("Time spent generating terrain: " + elapsedTime);

        //spawn resources
        spawnManager = new Spawns();
        for (int i = 0; i < map.Count; i++)
        {
            spawnManager.Populate(map[i].GetComponent<Tile>());
        }

        CreateDecoys();

        //set player on middle of map
        surface.BuildNavMesh();
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().SetPlayerInitialPos(this);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.R)) //DEBUG
        //{
        //    GenMap();
        //}
        //if (Input.GetKeyDown(KeyCode.T)) //DEBUG
        //{
        //    changeUVs();
        //}
    }

    //void changeUVs()
    //{
    //    foreach(GameObject g in map)
    //    {
    //        g.GetComponent<Tile>().ChangeUVs(uvX, uvY);
    //    }
    //}

    void GenMap()
    {
        if (mapSizeY == 0 || mapSizeX == 0 || sizeXtile == 0 || sizeZtile == 0)
        {
            Debug.LogError("Map and tiles can't be with any size 0!");
            return;
        }
        if (first) //Create map
        {
            for (int z = 0; z < mapSizeY; z++) //Y
            {
                for (int x = 0; x < mapSizeX; x++) //X
                {
                    //Choose Biome pure random, CHANGE
                    BiomeStats b = Biomes.biomes[UnityEngine.Random.Range(0, Biomes.biomes.Count)]; //max exclusive

                    //Create tile
                    GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    plane.name = (z * mapSizeY + x).ToString() + " " + b.biome.ToString();

                    //set walking + hooking properties
                    plane.layer = LayerMask.NameToLayer("Hookable");
                    plane.tag = "Ground";
                    plane.transform.position = new Vector3(x * sizeXtile, 0, z * sizeZtile);

                    Tile t = plane.AddComponent<Tile>();
                    t.Initialize(b, sizeXtile, sizeZtile, textureDetailMultiplier, multiThreading);

                    map.Add(plane);

                    ConnectInsideEdges(x, z, t);
                }
            }
            ConectOuterEdges();
            for (int i = 0; i < map.Count; i++)
            {
                Tile t = map[i].GetComponent<Tile>();
                if (!multiThreading)
                {
                    t.CreateColors();
                }
                else
                {
                    t.MultiThreadingCreateColors();
                }

                map[i].GetComponent<Tile>().UpdateMesh();
            }
            first = false;
        }
        else //Just update map, ignores editor OLD, CHANGE
        {
            int x = 0, z = 0;
            for (int i = 0; i < map.Count; i++)
            {
                if (z == mapSizeY)
                {
                    Debug.LogError("Something wrong isn't right");
                }

                Tile t = map[i].GetComponent<Tile>();
                t.Initialize(); //creates shape

                ConnectInsideEdges(x, z, t); //connects tiles

                if (multiThreading) //creates texture
                {
                    t.MultiThreadingCreateColors();
                }
                else
                {
                    t.CreateColors();
                }

                x++;
                if (x == mapSizeX) //went down a row
                {
                    x = 0;
                    z++;
                }
            }
            for (int i = 0; i < map.Count; i++)
            {
                map[i].GetComponent<Tile>().UpdateMesh();
            }
        }
    }

    void CreateDecoys()
    {
        for (int i = 0; i < mapSizeX; i++) //top + bottom decoys
        {
            //top
            GameObject plane = Instantiate(map[i]); //create copy
            plane.name = "Top " + i + "decoy";
            plane.transform.position = new Vector3(i * sizeXtile, 0, mapSizeY * sizeZtile); //1 outside
            DecoyBehaviour dB = plane.AddComponent<DecoyBehaviour>();
            dB.original = map[i];

            //bottom
            GameObject plane2 = Instantiate(map[(mapSizeY - 1) * mapSizeY + i]); //create copy
            plane2.name = "Bottom " + i + "decoy";
            plane2.transform.position = new Vector3(i * sizeXtile, 0, -sizeZtile); //-1 outside
            DecoyBehaviour dB2 = plane2.AddComponent<DecoyBehaviour>();
            dB2.original = map[(mapSizeY - 1) * mapSizeY + i];

            decoys.Add(plane);
            decoys.Add(plane2);
        }

        for (int i = 0; i < mapSizeY; i++) //left + right decoys
        {
            //Left
            GameObject plane = Instantiate(map[i * mapSizeY + mapSizeX - 1]); //create copy
            plane.name = "Left " + i + "decoy";
            plane.transform.position = new Vector3(-sizeXtile, 0, sizeZtile * i);
            DecoyBehaviour dB = plane.AddComponent<DecoyBehaviour>();
            dB.original = map[i * mapSizeY + mapSizeX - 1];

            //Right
            GameObject plane2 = Instantiate(map[i * mapSizeY + 0]); //create copy
            plane2.name = "Right " + i + "decoy";
            plane2.transform.position = new Vector3(sizeXtile * mapSizeY, 0, sizeZtile * i);
            DecoyBehaviour dB2 = plane2.AddComponent<DecoyBehaviour>();
            dB2.original = map[i * mapSizeY + 0];

            decoys.Add(plane);
            decoys.Add(plane2);
        }

        //Diagonal decoys
        //top left
        GameObject topLeft = Instantiate(map[mapSizeX - 1]);
        topLeft.name = "Top left decoy";
        topLeft.transform.position = new Vector3(-sizeXtile, 0, sizeZtile * mapSizeY);
        DecoyBehaviour dB3 = topLeft.AddComponent<DecoyBehaviour>();
        dB3.original = map[mapSizeX - 1];

        //top right
        GameObject topRight = Instantiate(map[0]);
        topRight.name = "Top right decoy";
        topRight.transform.position = new Vector3(sizeXtile * mapSizeX, 0, sizeZtile * mapSizeY);
        DecoyBehaviour dB4 = topRight.AddComponent<DecoyBehaviour>();
        dB4.original = map[0];

        //bottom left
        GameObject bottomLeft = Instantiate(map[map.Count - 1]);
        bottomLeft.name = "Bottom left decoy";
        bottomLeft.transform.position = new Vector3(-sizeXtile, 0, -sizeZtile);
        DecoyBehaviour dB5 = bottomLeft.AddComponent<DecoyBehaviour>();
        dB5.original = map[map.Count - 1];

        //bottom right
        GameObject bottomRight = Instantiate(map[(mapSizeY - 1) * mapSizeX]);
        bottomRight.name = "Bottom right decoy";
        bottomRight.transform.position = new Vector3(sizeXtile * mapSizeX, 0, -sizeZtile);
        DecoyBehaviour dB6 = bottomRight.AddComponent<DecoyBehaviour>();
        dB6.original = map[(mapSizeY - 1) * mapSizeX];

        decoys.Add(topLeft);
        decoys.Add(topRight);
        decoys.Add(bottomLeft);
        decoys.Add(bottomRight);
    }

    void ConnectInsideEdges(int x, int z, Tile t)
    {
        //Connect and smooth tile to left
        if (x != 0)
        {
            //left tile
            Tile lastVerts = map[z * mapSizeY + x - 1].GetComponent<Tile>();
            for (int k = 0; k < sizeZtile + 1; k++)//tiles are size+1, ignore corners
            {
                if ((k == 0 && z != 0) || (k == sizeZtile && z != mapSizeY - 1))//corners with more tiles
                {
                    if (z != 0 && k == 0)
                    {
                        Tile bottomLeft = map[(z - 1) * mapSizeY + x - 1].GetComponent<Tile>();
                        Tile bottom = map[(z - 1) * mapSizeY + x].GetComponent<Tile>();

                        Vector3 current = t.vertexes[sizeXtile * k + k];
                        Vector3 left = lastVerts.vertexes[k * sizeXtile + k + sizeXtile];

                        Vector3 currentBottom = bottom.vertexes[bottom.vertexes.Length - 1 - sizeXtile];
                        Vector3 leftBottom = bottomLeft.vertexes[bottomLeft.vertexes.Length - 1];

                        float result = mediaHeightOne(current, left, currentBottom, leftBottom);

                        t.vertexes[sizeXtile * k + k].y = result;
                        lastVerts.vertexes[k * sizeXtile + k + sizeXtile].y = result;
                        bottom.vertexes[bottomLeft.vertexes.Length - 1 - sizeXtile].y = result;
                        bottomLeft.vertexes[bottom.vertexes.Length - 1].y = result;
                    }

                    continue;//skip loop (doesn't do code below)
                }

                //left side of current connects to right side of left tile
                float height = mediaHeightOne(t.vertexes[sizeXtile * k + k], lastVerts.vertexes[k * sizeXtile + k + sizeXtile]);

                t.vertexes[sizeXtile * k + k].y = height;
                lastVerts.vertexes[k * sizeXtile + k + sizeXtile].y = height;

                //TestPoint(t.vertexes[sizeXtile * k + k]);
            }
        }

        //Connect and smooth tile to bottom
        if (z != 0)
        {
            //bottom tile
            Vector3[] lastVerts = map[(z - 1) * mapSizeY + x].GetComponent<Tile>().vertexes;
            for (int l = 0; l < sizeXtile + 1; l++)//tiles are size+1, ignore corners
            {
                if ((l == 0 && x != 0) || (l == sizeZtile && x != mapSizeY - 1))
                {
                    continue; //skip this loop
                }
                //bottom of current connects to top of bottom tile
                float height = mediaHeightOne(t.vertexes[l], lastVerts[(sizeXtile + 1) * (sizeZtile) + l]);

                t.vertexes[l].y = height;
                map[(z - 1) * mapSizeY + x].GetComponent<Tile>().vertexes[(sizeXtile + 1) * (sizeZtile) + l].y = height;
            }
        }
    }

    void ConectOuterEdges()
    {
        //this all makes sure decoys are conected
        //conect bottom edge to top edge
        for (int i = 0; i < mapSizeX; i++)
        {
            Tile topTile = map[(mapSizeY - 1) * mapSizeX + i].GetComponent<Tile>();
            Tile bottomTile = map[i].GetComponent<Tile>();

            for (int j = 0; j < sizeXtile + 1; j++) //size is (sizeX + 1) * (sizeZ + 1)
            {
                float result = mediaHeightOne(topTile.vertexes[sizeZtile * (sizeXtile + 1) + j], bottomTile.vertexes[j]);
                //Debug.Log("Connecting " + ((mapSizeY - 1) * mapSizeX + i) + " to " + i + " point " + (sizeZtile * (sizeXtile + 1) + j) + " to " + j);
                topTile.vertexes[sizeZtile * (sizeXtile + 1) + j].y = result;
                bottomTile.vertexes[j].y = result;
            }
        }

        //connect left edge to right edge
        for (int i = 0; i < mapSizeY; i++)
        {
            Tile LeftTile = map[i * mapSizeX].GetComponent<Tile>();
            Tile rightTile = map[(mapSizeY - 1) + i * mapSizeX].GetComponent<Tile>();

            for (int j = 0; j < sizeZtile + 1; j++) //size is (sizeX + 1) * (sizeZ + 1)
            {
                float result = mediaHeightOne(LeftTile.vertexes[(sizeXtile + 1) * j], rightTile.vertexes[sizeZtile + j * (sizeXtile + 1)]);
                //Debug.Log("Connecting " + ((mapSizeY - 1) + i * mapSizeX) + " to " + (i * mapSizeX) + " point " + (sizeZtile + j * (sizeXtile + 1)) + " to " + ((sizeXtile + 1) * j));
                LeftTile.vertexes[(sizeXtile + 1) * j].y = result;
                rightTile.vertexes[sizeZtile + j * (sizeXtile + 1)].y = result;
            }
        }
    }

    public float mediaHeightOne(Vector3 numOne, Vector3 numTwo)
    {
        float media = (numOne.y + numTwo.y) / 2;
        return media;
    }

    public float mediaHeightOne(Vector3 numOne, Vector3 numTwo, Vector3 numThree, Vector3 numFour)
    {
        float media = (numOne.y + numTwo.y + numThree.y + numFour.y) / 4;
        return media;
    }

    public Material GetRandomMat()
    {
        Material rndColor = new Material(Shader.Find("Standard"));
        rndColor.color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1f);
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
            if (new Rect(new Vector2(map[i].transform.position.x, map[i].GetComponent<Tile>().transform.position.z), new Vector2(sizeXtile, sizeZtile)).Contains(pos))
            {
                return map[i].GetComponent<Tile>().HeightAt(pos);
            }
        }
        throw new Exception("Point " + pos + " out of bounds");
    }

    public Tile FindTileByPosition(Vector2 pos)
    {
        for (int i = 0; i < map.Count; i++)
        {
            if (new Rect(new Vector2(map[i].transform.position.x, map[i].GetComponent<Tile>().transform.position.z), new Vector2(sizeXtile, sizeZtile)).Contains(pos))
            {
                return map[i].GetComponent<Tile>();
            }
        }
        //throw new Exception("Point " + pos + " out of bounds");
        Debug.LogError("Point " + pos + " out of bounds");
        return null;
    }
}