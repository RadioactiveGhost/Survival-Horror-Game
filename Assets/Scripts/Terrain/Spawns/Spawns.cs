using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawns
{
    //public GameObject terrain;
    //TerrainGenerator tScript;
    float maxX, maxZ;
    bool repeat;

    //private void Start()
    //{
    //    //tScript = terrain.GetComponent<TerrainGenerator>();
    //}

    void CreateSpawns(Tile t)
    {
        maxX = t.mainPos.x + t.sizeXtile;
        maxZ = t.mainPos.z + t.sizeZtile;

        for (int i = 0; i < (t.sizeXtile * t.sizeZtile) / 2; i++)
        {
            repeat = false;
            Vector3 place = new Vector3(Random.Range(t.mainPos.x, maxX), 0, Random.Range(t.mainPos.z, maxZ));
            place.y = t.HeightAt(new Vector2(place.x, place.z));
            for (int j = 0; j < t.spawns.Count; j++)//If place isn't repeated add that one to list
            {
                if (t.spawns[j].location == place)
                {
                    repeat = true;
                    break;
                }
            }
            if(!repeat)
            {
                Spawn s = new Spawn();
                s.location = place;
                s.thing = Things.None;
                t.spawns.Add(s);
            }
        }
    }

    void Spawnables(Tile t)
    {
        switch (t.biome.biome)
        {
            case Biome.Forest:
                for (int i = 0; i < t.spawns.Count / 2; i++)
                {
                    int place = Random.Range(0, t.spawns.Count);
                    Spawn s = new Spawn();
                    s.location = t.spawns[place].location;

                    //Choose thing
                    int choice = Random.Range(0, 100);
                    if (choice < 10)
                    {
                        s.thing = Things.Rock;
                    }
                    else
                    {
                        s.thing = Things.Tree;
                    }

                    t.spawns[place] = s;
                }
                break;

            case Biome.Mountain:

                for (int i = 0; i < t.spawns.Count / 4; i++)
                {
                    int place = Random.Range(0, t.spawns.Count);
                    Spawn s = new Spawn();
                    s.location = t.spawns[place].location;

                    //Choose thing
                    int choice = Random.Range(0, 100);
                    if (choice < 90)
                    {
                        s.thing = Things.Rock;
                    }
                    else
                    {
                        s.thing = Things.Tree;
                    }

                    t.spawns[place] = s;
                }

                break;

            case Biome.Plains:

                for (int i = 0; i < t.spawns.Count / 16; i++)
                {
                    int place = Random.Range(0, t.spawns.Count);
                    Spawn s = new Spawn();
                    s.location = t.spawns[place].location;

                    //Choose thing
                    int choice = Random.Range(0, 100);
                    if (choice < 50)
                    {
                        s.thing = Things.Rock;
                    }
                    else
                    {
                        s.thing = Things.Tree;
                    }

                    t.spawns[place] = s;
                }

                break;
        }
    }

    public void Populate(Tile t)
    {
        CreateSpawns(t);
        Spawnables(t);
        PutThingsInPlace(t);
    }

    void PutThingsInPlace(Tile t)
    {
        foreach (Spawn s in t.spawns)
        {
            if (s.thing != Things.None)
            {
                GameObject g;
                if (s.thing == Things.Tree)
                {
                    g = GameObject.Instantiate((GameObject)Resources.Load<GameObject>("Tree"));
                }
                else if (s.thing == Things.Rock)
                {
                    g = GameObject.Instantiate((GameObject)Resources.Load<GameObject>("Rock"));
                }
                else //something went wrong
                {
                    Debug.LogError("Tried to instantiate " + s.thing.ToString() + " wich corresponds to no specified enum");
                    continue;
                }
                g.transform.position = s.location;
                g.tag = "Thing";
                g.name = s.thing.ToString();
            }
        }
    }
}