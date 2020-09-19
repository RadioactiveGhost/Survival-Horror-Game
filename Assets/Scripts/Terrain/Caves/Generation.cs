using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Block
{
    
    public List<Vector3Int> childBlocks;
    public Vector3Int parent;
    public bool top;
    public bool down;
    public bool north;
    public bool south;
    public bool east;
    public bool west;
    public bool used;
    public Vector3Int pos;

    public Block(Vector3Int pos)
    {
        this.pos = pos;
        used = false;
        top = false;
        down = false;
        north = false;
        south = false;
        east = false;
        west = false;
        childBlocks = new List<Vector3Int>();
    }
}

public enum BlockType
{
    //all 6
    top_down_north_south_east_west,

    //5
    top_down_north_south_east,
    top_down_north_south_west,
    top_down_north_east_west,
    top_down_south_east_west,
    top_north_south_east_west,
    down_north_south_east_west,

    //4
    top_down_north_south,
    down_north_south_east,
    top_down_north_east,
    top_down_south_east,
    top_north_south_east,
    top_down_north_west,
    top_down_south_west,
    top_north_south_west,
    down_north_south_west,
    top_down_east_west,
    top_north_east_west,
    down_north_east_west,
    top_south_east_west,
    down_south_east_west,
    north_south_east_west,

    //3
    top_down_north,
    top_down_south,
    top_north_south,
    down_north_south,
    top_down_east,
    top_north_east,
    down_north_east,
    top_south_east,
    down_south_east,
    north_south_east,
    top_down_west,
    top_north_west,
    down_north_west,
    top_south_west,
    down_south_west,
    north_south_west,
    top_east_west,
    down_east_west,
    north_east_west,
    south_east_west,

    //2
    top_down,
    top_north,
    down_north,
    top_south,
    down_south,
    north_south,
    top_east,
    down_east,
    north_east,
    south_east,
    top_west,
    down_west,
    north_west,
    south_west,
    east_west,

    //1
    top,
    down,
    north,
    south,
    east,
    west,

    Error
}

public class Generation : MonoBehaviour
{
    public NavMeshSurface surface;
    public int scale;
    public const int size = 5;

    Block[,,] map;
    Block startPoint;
    //LineRenderer lr;

    GameObject caveParent;
    GameObject resourceParent;

    [HideInInspector]
    public List<Vector3> cavePoints;

    void Start()
    {
        //Debug
        //lr = gameObject.AddComponent<LineRenderer>();
        //lr.startColor = Color.green;
        //lr.endColor = Color.red;
        //lr.startWidth = 0.2f;
        //lr.endWidth = 0.2f;
        caveParent = new GameObject();
        caveParent.name = "Cave Parent";
        resourceParent = new GameObject();
        resourceParent.name = "Resource Parent";
        cavePoints = new List<Vector3>();

        //Debug.Log("Initializing...");
        Initialize();
        //Debug.Log("Generating Labyrinth...");
        CreateLabyrinth();
        //Debug.Log("Making Labyrinth...");
        SetUpLabyrinth();
    }

    void Initialize()
    {
        map = new Block[size, size, size];

        //Initialize blocks
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    map[x, y, z] = new Block(new Vector3Int(x, y, z));
                }
            }
        }
    }

    BlockType BlockToEnum(Block block)
    {
        int count = 0;
        int connections = 0;
        bool[] bools = { false, false, false, false, false, false };
        string[] sides = { "top", "down", "north", "south", "east", "west" };

        if (block.top)
        {
            bools[0] = true;
            connections++;
        }
        if (block.down)
        {
            bools[1] = true;
            connections++;
        }
        if (block.north)
        {
            bools[2] = true;
            connections++;
        }
        if (block.south)
        {
            bools[3] = true;
            connections++;
        }
        if (block.east)
        {
            bools[4] = true;
            connections++;
        }
        if (block.west)
        {
            bools[5] = true;
            connections++;
        }

        string s = "";
        for (int i = 0; i < bools.Length; i++)
        {
            if (bools[i] && count != 0)
            {
                s += "_" + sides[i].ToString();
                count++;
            }
            else if (bools[i])
            {
                s += sides[i].ToString();
                count++;
            }
        }

        if(s == "")
        {
            return BlockType.Error;
        }

        BlockType bt = (BlockType)System.Enum.Parse(typeof(BlockType), s);

        //Debug.Log(bt.ToString());

        return bt;
    }

    Vector3Int ChooseStartPoint()
    {
        //middle of top
        int x = Random.Range(0, size);//exclusive
        int y = size - 1;
        int z = Random.Range(0, size);
        return new Vector3Int(x, y, z);
    }

    void CreateLabyrinth()
    {
        Vector3Int currentCoords = ChooseStartPoint();
        //Block currentBlock = map[currentCoords.x, currentCoords.y, currentCoords.z];

        map[currentCoords.x, currentCoords.y, currentCoords.z].used = true;
        map[currentCoords.x, currentCoords.y, currentCoords.z].top = true;
        startPoint = map[currentCoords.x, currentCoords.y, currentCoords.z];

        do
        {
            //Debug.Log("On " + currentCoords);
            //CHANGE
            Vector3Int newCoords = ChooseAndMovePath(currentCoords);
            cavePoints.Add(currentCoords);

            if (newCoords != new Vector3Int(-1, -1, -1))
            {
                currentCoords = newCoords;
                //Debug.Log("Moved to " + currentCoords);
            }
            else
            {
                //Backtrack
                //Debug.Log("Moving back!");
                currentCoords = map[currentCoords.x, currentCoords.y, currentCoords.z].parent;
            }

        } while (!(currentCoords == startPoint.pos && !AvailablePath(currentCoords)));
    }

    bool AvailablePath(Vector3Int coords)
    {
        int index = 0;
        List<int> indexes = new List<int>();

        //top_down_north_south_east_west
        List<Vector3Int> list = new List<Vector3Int>();
        //top
        list.Add(PathRelativeTest(coords, new Vector3Int(0, 0, 1)));
        //down
        list.Add(PathRelativeTest(coords, new Vector3Int(0, 0, -1)));
        //north
        list.Add(PathRelativeTest(coords, new Vector3Int(1, 0, 0)));
        //south
        list.Add(PathRelativeTest(coords, new Vector3Int(-1, 0, 0)));
        //east
        list.Add(PathRelativeTest(coords, new Vector3Int(0, 1, 0)));
        //west
        list.Add(PathRelativeTest(coords, new Vector3Int(0, -1, 0)));

        foreach (Vector3Int v in list)
        {
            if (v != new Vector3Int(-1, -1, -1) && !map[v.x, v.y, v.z].used) //not invalid and not used
            {
                indexes.Add(index);
                //Debug.Log(coords + " to " + v + " which = " + list[index]);
            }
            index++;
        }

        //Debug.Log("Possible paths: " + indexes.Count);

        if (indexes.Count == 0) //No paths available
        {
            return false;
        }

        return true;
    }

    Vector3Int ChooseAndMovePath(Vector3Int coords)
    {
        //Block b = map[coords.x, coords.y, coords.z];

        int index = 0;
        List<int> indexes = new List<int>();

        //top_down_north_south_east_west
        List<Vector3Int> list = new List<Vector3Int>();
        //top
        //list.Add(PathRelativeTest(coords, new Vector3Int(0, 0, 1)));
        list.Add(PathRelativeTest(coords, new Vector3Int(0, 1, 0)));
        //down
        //list.Add(PathRelativeTest(coords, new Vector3Int(0, 0, -1)));
        list.Add(PathRelativeTest(coords, new Vector3Int(0, -1, 0)));
        //north
        list.Add(PathRelativeTest(coords, new Vector3Int(1, 0, 0)));
        //south
        list.Add(PathRelativeTest(coords, new Vector3Int(-1, 0, 0)));
        //east
        //list.Add(PathRelativeTest(coords, new Vector3Int(0, 1, 0)));
        list.Add(PathRelativeTest(coords, new Vector3Int(0, 0, 1)));
        //west
        //list.Add(PathRelativeTest(coords, new Vector3Int(0, -1, 0)));
        list.Add(PathRelativeTest(coords, new Vector3Int(0, 0, -1)));

        foreach (Vector3Int v in list)
        {
            if (v != new Vector3Int(-1, -1, -1) && !map[v.x, v.y, v.z].used) //not invalid and not used
            {
                indexes.Add(index);
                //Debug.Log(coords + " to " + v + " which = " + list[index]);
            }
            index++;
        }

        //Debug.Log("Possible paths: " + indexes.Count);

        if (indexes.Count == 0) //No paths available
        {
            return new Vector3Int(-1, -1, -1);
        }
        else
        {
            int choice = Random.Range(0, indexes.Count); //max exclusive

            //save new block on original for pathing (previous line should be doing it)
            switch (indexes[choice])
            {
                case 0:
                    //top
                    map[coords.x, coords.y, coords.z].top = true; //open path
                    map[list[indexes[choice]].x, list[indexes[choice]].y, list[indexes[choice]].z].down = true; //open path
                    break;
                case 1:
                    //down
                    map[coords.x, coords.y, coords.z].down = true; //open path
                    map[list[indexes[choice]].x, list[indexes[choice]].y, list[indexes[choice]].z].top = true; //open path
                    break;
                case 2:
                    //north
                    map[coords.x, coords.y, coords.z].north = true; //open path
                    map[list[indexes[choice]].x, list[indexes[choice]].y, list[indexes[choice]].z].south = true; //open path
                    break;
                case 3:
                    //south
                    map[coords.x, coords.y, coords.z].south = true; //open path
                    map[list[indexes[choice]].x, list[indexes[choice]].y, list[indexes[choice]].z].north = true; //open path
                    break;
                case 4:
                    //east
                    map[coords.x, coords.y, coords.z].east = true; //open path
                    map[list[indexes[choice]].x, list[indexes[choice]].y, list[indexes[choice]].z].west = true; //open path
                    break;
                case 5:
                    //west
                    map[coords.x, coords.y, coords.z].west = true; //open path
                    map[list[indexes[choice]].x, list[indexes[choice]].y, list[indexes[choice]].z].east = true; //open path
                    break;
            }
            map[list[indexes[choice]].x, list[indexes[choice]].y, list[indexes[choice]].z].used = true; //pathed
            map[list[indexes[choice]].x, list[indexes[choice]].y, list[indexes[choice]].z].parent = coords;
            map[coords.x, coords.y, coords.z].childBlocks.Add(list[indexes[choice]]);

            return list[indexes[choice]];
        }
    }

    Vector3Int PathRelativeTest(Vector3Int pos, Vector3Int relative)
    {
        if(relative.x > 1 || relative.y > 1 ||relative.z > 1 || relative.x < -1 || relative.y < -1 || relative.z < -1)
        {
            //Debug.LogError("Relative vector is invalid!");
            //return new Vector3Int(-1, -1, -1);
            throw new System.Exception("Relative vector is invalid!");
        }

        Vector3Int possible = pos + relative;

        if (possible.x >= size || possible.x < 0 || possible.y >= size || possible.y < 0 || possible.z >= size || possible.z < 0)
        {
            return new Vector3Int(-1, -1, -1); //Not possible
        }

        return possible;
    }

    void SetUpLabyrinth()
    {
        surface.BuildNavMesh();
        GameObject p = GameObject.Instantiate((GameObject)Resources.Load("Caves/" + BlockType.top_down.ToString()));
        p.name = "Entrance";
        p.transform.position = new Vector3(startPoint.pos.x, startPoint.pos.y + 1, startPoint.pos.z) * scale;
        SphereCollider sc = p.AddComponent<SphereCollider>();
        sc.isTrigger = true;
        sc.radius = 0.2f;
        sc.center = new Vector3(sc.center.x, sc.center.y, 0.3f);
        p.AddComponent<Exit>();

        GameObject.FindGameObjectWithTag("Player").transform.position = p.transform.position;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    Vector3 pos = new Vector3(x, y, z) * scale;
                    BlockType type = BlockToEnum(map[x, y, z]);
                    try
                    {
                        GameObject o = GameObject.Instantiate((GameObject)Resources.Load("Caves/" + type.ToString()));
                        o.name = type.ToString();
                        o.transform.position = pos;
                        o.transform.parent = caveParent.transform;
                        try
                        {
                            StartCoroutine(SetResource(pos));
                        }
                        catch
                        {
                            Debug.Log("Set resource failed");
                            continue;
                        }
                    }
                    catch
                    {
                        GameObject o = GameObject.Instantiate((GameObject)Resources.Load("Caves/Default"));
                        o.name = type.ToString();
                        o.transform.position = pos;
                        Debug.LogError("Failed to load " + type.ToString() + " is it missing?");
                    }
                }
            }
        }
    }

    IEnumerator SetResource(Vector3 point)
    {
        yield return 0; //wait 1 frame

        Things thing = Things.None;
        int choice = Random.Range(0, 100);
        if(choice < 10)
        {
            thing = Things.Crystal;
        }
        else if(choice < 20)
        {
            thing = Things.MetalNode;
        }

        if (thing != Things.None)
        {
            Vector3 dir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            dir.Normalize();
            RaycastHit hit;
            if(Physics.Raycast(point, dir, out hit, 20.0f))
            {
                if(hit.transform.tag == "Ground")
                {
                    SpawnResource(hit.point, thing);
                }
                else
                {
                    Debug.Log(hit.transform.tag);
                }
            }
            else
            {
                //lr.SetPosition(0, point);
                //lr.SetPosition(1, point + dir * 20);
                //Debug.Log("Raycast failed " + hit.collider.gameObject.name);
            }
        }
    }

    void SpawnResource(Vector3 place, Things t)
    {
        //Debug.Log("Spawning at " + place);
        if(t == Things.Crystal)
        {
            GameObject g = (GameObject)GameObject.Instantiate(Resources.Load("Crystal"));
            g.transform.position = place;
            g.transform.parent = resourceParent.transform;
        }
        else if (t == Things.MetalNode)
        {
            GameObject g = (GameObject)GameObject.Instantiate(Resources.Load("Metal Node"));
            g.transform.position = place;
            g.transform.parent = resourceParent.transform;
        }
        else
        {
            Debug.LogError("None");
        }
    }

}