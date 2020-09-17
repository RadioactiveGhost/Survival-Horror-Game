using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawnerCave : MonoBehaviour
{
    public GameObject generator;
    private Generation generation;

    private void Start()
    {
        generation = generator.GetComponent<Generation>();
    }

    public void SpawnCrawler(Vector3 position)
    {
        SpawnEnemy((GameObject)Resources.Load<GameObject>("Spooks/SpookDC"), position);
    }

    public void SpawnEnemy(GameObject mob, Vector3 position)
    {
        GameObject newMob = Instantiate(mob, new Vector3(position.x, position.y,
                 position.z), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
    }
}
