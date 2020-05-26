using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    public GameObject terrain;
    TerrainGenerator terrainScript;
    public bool wolves, boars, boarpacks, wolfpacks, deers, bears, bunnies;
    float random;
    public float minNumber, maxNumber;
    // public GameObject wolf, boar, wolfpack1, boarpack1, deer, bear, bunny;
    Vector3 rngposition;

    // Start is called before the first frame update
    void Awake()
    { //"GameObject o = GameObject.Instantiate((GameObject)Resources.Load("Caves/Default"));"

        terrainScript = terrain.GetComponent<TerrainGenerator>();
        if (wolves)
            Spawn((GameObject)Resources.Load("Animals/wolf") as GameObject, (int)Random.Range(minNumber, maxNumber));
        if (bears)
            Spawn((GameObject)Resources.Load("Animals/bear"), (int)Random.Range(minNumber, maxNumber));
        if (boars)
            Spawn((GameObject)Resources.Load("Animals/boar"), (int)Random.Range(minNumber, maxNumber));
        if (deers)
            Spawn((GameObject)Resources.Load("Animals/deer"), (int)Random.Range(minNumber, maxNumber));
        if (bunnies)
            Spawn((GameObject)Resources.Load("Animals/bunny"), (int)Random.Range(minNumber, maxNumber));
        if (boarpacks)
            Spawn((GameObject)Resources.Load("Animals/Boars/boarPack1"), (int)Random.Range(minNumber, maxNumber));
        if (wolfpacks)
            Spawn((GameObject)Resources.Load("Animals/Wolves/wolfpack1"), (int)Random.Range(minNumber, maxNumber));


    }

    public void Spawn(GameObject animal, int number)
    {
        for (int i = 0; i == number; i++)
        {
            Debug.Log("SPAWNED YEY");
            GameObject newAnimal = Instantiate(animal, new Vector3(Random.Range(0, terrainScript.sizeXtile * terrainScript.mapSizeX), 11f,
                  Random.Range(0, terrainScript.sizeZtile * terrainScript.mapSizeY)), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
