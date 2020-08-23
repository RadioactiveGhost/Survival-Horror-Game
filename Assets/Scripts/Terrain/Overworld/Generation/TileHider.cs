using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHider : MonoBehaviour
{
    public float timer;
    float timeHelper;
    TerrainGenerator tG;
    Transform player;

    List<GameObject> temp;

    // Start is called before the first frame update
    void Start()
    {
        tG = GetComponent<TerrainGenerator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        temp = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timeHelper >= timer)
        {
            tileCheck();
            timeHelper = 0;
        }
        else
        {
            timeHelper += Time.deltaTime;
        }

        for(int i = 0;i<tG.decoys.Count;i++)
        {
            tG.decoys[i].GetComponent<DecoyBehaviour>().CustomUpdate();
        }
    }

    void tileCheck()
    {
        int index = tG.map.IndexOf(tG.FindTileByPosition(new Vector2(player.transform.position.x, player.transform.position.z)).gameObject);
        int helper, indexCheck;
        bool active;

        for (int k = -1; k < 2; k++)//check 3 rows
        {
            helper = index + k * tG.mapSizeX;

            if (index < tG.mapSizeX && k == -1) //bottom
            {
                helper = tG.map.Count - (tG.mapSizeX - index);
            }
            else if(index >= tG.mapSizeX * (tG.mapSizeZ - 1) && k == 1) //top
            {
                helper = index % tG.mapSizeX; //FIX
            }


            for (int j = -1; j < 2; j++)//check 3 collumns
            {
                indexCheck = helper + j;

                if (helper % tG.mapSizeX == 0 && j == -1) //on left edge
                {
                    indexCheck = helper + (tG.mapSizeX - 1);
                }
                else if (helper % tG.mapSizeX == tG.mapSizeX - 1 && j == 1) //on right edge
                {
                    indexCheck = helper - (tG.mapSizeX - 1);
                }

                //out of index bounds check
                if (indexCheck < 0)
                {
                    Debug.Log("Inverted index " + indexCheck);
                    indexCheck = tG.map.Count - 1 + indexCheck; //invert
                    Debug.Log("To " + indexCheck);
                }
                else if (indexCheck >= tG.map.Count)
                {
                    Debug.Log("Inverted index " + indexCheck);
                    indexCheck = indexCheck - tG.map.Count - 1; //invert
                    Debug.Log("To " + indexCheck);
                }

                temp.Add(tG.map[indexCheck]);
            }
        }

        for (int i = 0; i < tG.map.Count; i++)
        {
            active = false;
            for (int j = 0; j < temp.Count; j++)
            {
                if (temp[j] == tG.map[i])
                {
                    active = true;
                    break;
                }
            }
            tG.map[i].SetActive(active);
        }

        temp.Clear();
    }
}