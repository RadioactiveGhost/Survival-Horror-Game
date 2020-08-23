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

    public int sizeXtile, sizeZtile, mapSizeX, mapSizeZ, textureDetailMultiplier;
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
        if (CustomGameManager.saveData == null)
        {
            GenerateEverything();
        }
        else
        {
            LoadEverything(CustomGameManager.saveData);
        }
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

    void GenerateEverything()
    {
        if (textureDetailMultiplier < 1)
        {
            Debug.LogError("TextureDetailMultiplier should be at least 1");
        }

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
        try
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().SetPlayerInitialPos(this);
        }
        catch
        {
            Debug.Log("Is player missing? Maybe untagged?");
        }
    }

    void LoadEverything(SaveData data)
    {
        if (textureDetailMultiplier < 1)
        {
            Debug.LogError("TextureDetailMultiplier should be at least 1");
        }

        //find tile size
        sizeXtile = data.sizeXtile;
        sizeZtile = data.sizeZtile;
        //Debug.Log("Tile size: " + sizeXtile + "," + sizeZtile);

        mapSizeX = data.mapSizeX;
        mapSizeZ = data.mapSizeZ;
        //Debug.Log("Map size: " + mapSizeX + "," + mapSizeZ);

        textureDetailMultiplier = data.textureDetailMultiplier;

        SetMap(data);

        spawnManager = new Spawns();
        for (int i = 0; i < map.Count; i++)
        {
            spawnManager.Populate(map[i].GetComponent<Tile>());
        }

        CreateDecoys();

        surface.BuildNavMesh();

        //set player on middle of map
        try
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().SetPlayerInitialPos(this);
        }
        catch
        {
            Debug.Log("Is player missing? Maybe untagged?");
        }
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
        if (mapSizeZ == 0 || mapSizeX == 0 || sizeXtile == 0 || sizeZtile == 0)
        {
            Debug.LogError("Map and tiles can't be with any size 0!");
            return;
        }
        if (first) //Create map
        {
            for (int z = 0; z < mapSizeZ; z++) //Y
            {
                for (int x = 0; x < mapSizeX; x++) //X
                {
                    //Choose Biome pure random, CHANGE
                    BiomeStats b = Biomes.biomes[UnityEngine.Random.Range(0, Biomes.biomes.Count)]; //max exclusive

                    //Create tile
                    GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    plane.name = (z * mapSizeZ + x).ToString() + " " + b.biome.ToString();

                    //set walking + hooking properties
                    plane.layer = LayerMask.NameToLayer("Hookable");
                    plane.tag = "Ground";
                    plane.transform.position = new Vector3(x * sizeXtile, 0, z * sizeZtile);

                    Tile t = plane.AddComponent<Tile>();
                    t.Initialize(b, sizeXtile, sizeZtile, textureDetailMultiplier, multiThreading);

                    map.Add(plane);

                    //connect edges to previous tile
                    ConnectInsideEdges(x, z, t);
                }
            }

            //Connect edges on edge tiles to other side
            ConectOuterEdges();

            //set colors by biome and height
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
            }

            //Change colors to transition from neighboors
            for (int i = 0; i < map.Count; i++)
            {
                //Debug.Log("Tile " + i);
                int[] numbers = new int[4];

                //top
                if (i < (mapSizeX - 1) * mapSizeZ) //isn't top row?
                {
                    numbers[0] = i + mapSizeX;
                }
                else
                {
                    numbers[0] = i - ((mapSizeX - 1) * mapSizeZ);
                }

                //bottom
                if (i >= mapSizeX) //isn't bottom row?
                {
                    numbers[1] = i - mapSizeX;
                }
                else
                {
                    numbers[1] = i + ((mapSizeX - 1) * mapSizeZ);
                }

                //left
                if (i % mapSizeX != 0) //isn't left collumn?
                {
                    numbers[2] = i - 1;
                }
                else
                {
                    numbers[2] = i + (mapSizeX - 1);
                }

                //right
                if (i % mapSizeX != mapSizeX - 1) //isn't right collumn?
                {
                    numbers[3] = i + 1;
                }
                else
                {
                    numbers[3] = i - (mapSizeX - 1);
                }

                map[i].GetComponent<Tile>().MixColors(map[numbers[0]].GetComponent<Tile>(), map[numbers[1]].GetComponent<Tile>(), map[numbers[2]].GetComponent<Tile>(), map[numbers[3]].GetComponent<Tile>());
            }

            //Update mesh to update all changes
            for (int i = 0; i < map.Count; i++)
            {
                map[i].GetComponent<Tile>().UpdateMesh();
            }
            first = false;
        }
        else //Just update map, ignores editor OLD, CHANGE
        {
            int x = 0, z = 0;
            for (int i = 0; i < map.Count; i++)
            {
                if (z == mapSizeZ)
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

    void SetMap(SaveData data)
    {
        for (int z = 0; z < mapSizeZ; z++) //Y
        {
            for (int x = 0; x < mapSizeX; x++) //X
            {
                //Create tile
                GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                plane.name = (z * mapSizeZ + x).ToString();

                //set walking + hooking properties
                plane.layer = LayerMask.NameToLayer("Hookable");
                plane.tag = "Ground";
                plane.transform.position = new Vector3(x * sizeXtile, 0, z * sizeZtile);

                Tile t = plane.AddComponent<Tile>();

                float[,] points = new float[data.points.GetLength(1), data.points.GetLength(2)];

                for (int i = 0; i < data.points.GetLength(1); i++)
                {
                    for (int j = 0; j < data.points.GetLength(2); j++)
                    {
                        points[i, j] = data.points[z * mapSizeX + x, i, j];
                    }
                }

                float[,] colors = new float[data.colors.GetLength(1), data.colors.GetLength(2)];

                for (int i = 0; i < data.colors.GetLength(1); i++)
                {
                    for (int j = 0; j < data.colors.GetLength(2); j++)
                    {
                        colors[i, j] = data.colors[z * mapSizeX + x, i, j];
                    }
                }

                t.Initialize(colors, points, sizeXtile, sizeZtile, data.textureDetailMultiplier);

                map.Add(plane);
            }
        }

        //Update mesh to update all changes
        for (int i = 0; i < map.Count; i++)
        {
            map[i].GetComponent<Tile>().SetTexture();
            map[i].GetComponent<Tile>().UpdateMesh();
        }
    }

    void CreateDecoys()
    {
        for (int i = 0; i < mapSizeX; i++) //top + bottom decoys
        {
            //top
            GameObject plane = Instantiate(map[i]); //create copy
            plane.name = "Top " + i + "decoy";
            plane.transform.position = new Vector3(i * sizeXtile, 0, mapSizeZ * sizeZtile); //1 outside
            DecoyBehaviour dB = plane.AddComponent<DecoyBehaviour>();
            dB.original = map[i];

            //bottom
            GameObject plane2 = Instantiate(map[(mapSizeZ - 1) * mapSizeZ + i]); //create copy
            plane2.name = "Bottom " + i + "decoy";
            plane2.transform.position = new Vector3(i * sizeXtile, 0, -sizeZtile); //-1 outside
            DecoyBehaviour dB2 = plane2.AddComponent<DecoyBehaviour>();
            dB2.original = map[(mapSizeZ - 1) * mapSizeZ + i];

            decoys.Add(plane);
            decoys.Add(plane2);
        }

        for (int i = 0; i < mapSizeZ; i++) //left + right decoys
        {
            //Left
            GameObject plane = Instantiate(map[i * mapSizeZ + mapSizeX - 1]); //create copy
            plane.name = "Left " + i + "decoy";
            plane.transform.position = new Vector3(-sizeXtile, 0, sizeZtile * i);
            DecoyBehaviour dB = plane.AddComponent<DecoyBehaviour>();
            dB.original = map[i * mapSizeZ + mapSizeX - 1];

            //Right
            GameObject plane2 = Instantiate(map[i * mapSizeZ + 0]); //create copy
            plane2.name = "Right " + i + "decoy";
            plane2.transform.position = new Vector3(sizeXtile * mapSizeZ, 0, sizeZtile * i);
            DecoyBehaviour dB2 = plane2.AddComponent<DecoyBehaviour>();
            dB2.original = map[i * mapSizeZ + 0];

            decoys.Add(plane);
            decoys.Add(plane2);
        }

        //Diagonal decoys
        //top left
        GameObject topLeft = Instantiate(map[mapSizeX - 1]);
        topLeft.name = "Top left decoy";
        topLeft.transform.position = new Vector3(-sizeXtile, 0, sizeZtile * mapSizeZ);
        DecoyBehaviour dB3 = topLeft.AddComponent<DecoyBehaviour>();
        dB3.original = map[mapSizeX - 1];

        //top right
        GameObject topRight = Instantiate(map[0]);
        topRight.name = "Top right decoy";
        topRight.transform.position = new Vector3(sizeXtile * mapSizeX, 0, sizeZtile * mapSizeZ);
        DecoyBehaviour dB4 = topRight.AddComponent<DecoyBehaviour>();
        dB4.original = map[0];

        //bottom left
        GameObject bottomLeft = Instantiate(map[map.Count - 1]);
        bottomLeft.name = "Bottom left decoy";
        bottomLeft.transform.position = new Vector3(-sizeXtile, 0, -sizeZtile);
        DecoyBehaviour dB5 = bottomLeft.AddComponent<DecoyBehaviour>();
        dB5.original = map[map.Count - 1];

        //bottom right
        GameObject bottomRight = Instantiate(map[(mapSizeZ - 1) * mapSizeX]);
        bottomRight.name = "Bottom right decoy";
        bottomRight.transform.position = new Vector3(sizeXtile * mapSizeX, 0, -sizeZtile);
        DecoyBehaviour dB6 = bottomRight.AddComponent<DecoyBehaviour>();
        dB6.original = map[(mapSizeZ - 1) * mapSizeX];

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
            Tile lastVerts = map[z * mapSizeZ + x - 1].GetComponent<Tile>();
            for (int k = 0; k < sizeZtile + 1; k++)//tiles are size+1, ignore corners
            {
                if ((k == 0 && z != 0) || (k == sizeZtile && z != mapSizeZ - 1))//corners with more tiles
                {
                    if (z != 0 && k == 0)
                    {
                        Tile bottomLeft = map[(z - 1) * mapSizeZ + x - 1].GetComponent<Tile>();
                        Tile bottom = map[(z - 1) * mapSizeZ + x].GetComponent<Tile>();

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
            Vector3[] lastVerts = map[(z - 1) * mapSizeZ + x].GetComponent<Tile>().vertexes;
            for (int l = 0; l < sizeXtile + 1; l++)//tiles are size+1, ignore corners
            {
                if ((l == 0 && x != 0) || (l == sizeZtile && x != mapSizeZ - 1))
                {
                    continue; //skip this loop
                }
                //bottom of current connects to top of bottom tile
                float height = mediaHeightOne(t.vertexes[l], lastVerts[(sizeXtile + 1) * (sizeZtile) + l]);

                t.vertexes[l].y = height;
                map[(z - 1) * mapSizeZ + x].GetComponent<Tile>().vertexes[(sizeXtile + 1) * (sizeZtile) + l].y = height;
            }
        }
    }

    void ConectOuterEdges()
    {
        //this all makes sure decoys are conected
        //conect bottom edge to top edge
        for (int i = 0; i < mapSizeX; i++)
        {
            Tile topTile = map[(mapSizeZ - 1) * mapSizeX + i].GetComponent<Tile>();
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
        for (int i = 0; i < mapSizeZ; i++)
        {
            Tile LeftTile = map[i * mapSizeX].GetComponent<Tile>();
            Tile rightTile = map[(mapSizeZ - 1) + i * mapSizeX].GetComponent<Tile>();

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