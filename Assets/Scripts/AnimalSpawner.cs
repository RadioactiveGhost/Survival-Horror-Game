﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    public List<GameObject> animalList;
    public GameObject terrain;
    public GameObject player;
    TerrainGenerator terrainScript;
    private bool animalsSpawnFlag = false;
    public bool wolves, boars, boarpacks, wolfpacks, deers, bears, bunnies;
    float random;
    public float minNumber, maxNumber;
    // public GameObject wolf, boar, wolfpack1, boarpack1, deer, bear, bunny;
    Vector3 rngposition;

    // Start is called before the first frame update
    void Start()
    { 

        
    }

    void Spawn(GameObject animal, int number)
    {
        for (int i = 0; i < number; i++)
        {
            Debug.Log("SPAWNED YEY");
            GameObject newAnimal = Instantiate(animal, new Vector3(Random.Range(0, terrainScript.sizeXtile * terrainScript.mapSizeX), 1.5f,
                  Random.Range(0, terrainScript.sizeZtile * terrainScript.mapSizeY)), Quaternion.identity);
            animalList.Add(newAnimal);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(animalList.Count);
        if(animalList.Count < 30)
        {
            animalsSpawnFlag = true;
        }
        DespawnFarMobs();

        if(animalsSpawnFlag == true)
        {
            SpawnMobs();
        }


    }
    void DespawnFarMobs()
    {
        foreach (GameObject animal in animalList)
        {
            float dist = Vector3.Distance(animal.transform.position, player.transform.position);
            if (dist > 40)
            {
                animalList.Remove(animal);
                Destroy(animal);
            }
        }
    }

    void SpawnMobs()
    {
        terrainScript = terrain.GetComponent<TerrainGenerator>();
        if (wolves)
            Spawn((GameObject)Resources.Load<GameObject>("Animals/wolf"), (int)Random.Range(minNumber, maxNumber));
        if (bears)
            Spawn((GameObject)Resources.Load<GameObject>("Animals/bear"), (int)Random.Range(minNumber, maxNumber));
        if (boars)
            Spawn((GameObject)Resources.Load<GameObject>("Animals/boar"), (int)Random.Range(minNumber, maxNumber));
        if (deers)
            Spawn((GameObject)Resources.Load<GameObject>("Animals/deer"), (int)Random.Range(minNumber, maxNumber));
        if (bunnies)
            Spawn((GameObject)Resources.Load<GameObject>("Animals/bunny"), (int)Random.Range(minNumber, maxNumber));
        if (boarpacks)
            Spawn((GameObject)Resources.Load<GameObject>("Animals/Boars/boarPack1"), (int)Random.Range(minNumber, maxNumber));
        if (wolfpacks)
            Spawn((GameObject)Resources.Load<GameObject>("Animals/Wolves/wolfpack1"), (int)Random.Range(minNumber, maxNumber));
        animalsSpawnFlag = false;
    }

}
