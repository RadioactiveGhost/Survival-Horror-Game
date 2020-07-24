using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mobSpawner : MonoBehaviour
{
    public bool lookPhobic, moveSensor, lifeLeach, packHunter, darkCrawler;
    public float minNumber, maxNumber;
    public float spawnRange, distForSpawn;
    public int numbOfMonsters = 20;

    
    public List<GameObject> mobList;
    
    public GameObject terrain;
    [HideInInspector]
    public GameObject player;
   
    TerrainGenerator terrainScript;
    
    private bool spawnFlag = false;
    

   

    void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(mobList.Count < numbOfMonsters)
        {
            spawnFlag = true;
        }

        if (spawnFlag == true)
        {
            SpawnMobs();
        }
    }

    void Spawn(GameObject mob, int number)
    {
        for (int i = 0; i < number; i++)
        {
            Vector3 position = new Vector3(player.GetComponent<Transform>().position.x + Random.Range(-spawnRange, spawnRange), 1.5f,
                player.GetComponent<Transform>().position.z + Random.Range(-spawnRange, spawnRange));


            while (position.x < 0 || position.x > terrainScript.sizeXtile * terrainScript.mapSizeX || position.z < 0
                || position.z > terrainScript.sizeZtile * terrainScript.mapSizeY || Vector3.Distance(player.GetComponent<Transform>().position, position) < distForSpawn)
            {

                position = new Vector3(player.GetComponent<Transform>().position.x + Random.Range(-spawnRange, spawnRange), 1.5f,
                player.GetComponent<Transform>().position.z + Random.Range(-spawnRange, spawnRange));
            }
            GameObject newMob = Instantiate(mob, new Vector3(position.x, 1.5f,
                  position.z), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
            mobList.Add(newMob);
        }
    }

    void SpawnMobs()
    {
        terrainScript = terrain.GetComponent<TerrainGenerator>();
        if (lookPhobic)
            Spawn((GameObject)Resources.Load<GameObject>("Spooks/SpookLP"), (int)Random.Range(minNumber, maxNumber));
        if (moveSensor)
            Spawn((GameObject)Resources.Load<GameObject>("Spooks/SpookMS"), (int)Random.Range(minNumber, maxNumber));
        if (lifeLeach)
            Spawn((GameObject)Resources.Load<GameObject>("Spooks/SpookBL"), (int)Random.Range(minNumber, maxNumber));
        //if (darkCrawler)
        //    Spawn((GameObject)Resources.Load<GameObject>("Spooks/SpookDC"), (int)Random.Range(minNumber, maxNumber));
        if (packHunter)
            Spawn((GameObject)Resources.Load<GameObject>("Spooks/SpookPH"), (int)Random.Range(minNumber, maxNumber));
        spawnFlag = false;
    }
}
