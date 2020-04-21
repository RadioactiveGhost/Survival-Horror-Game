using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



[RequireComponent(typeof(NavMeshAgent))]
public class MobBeaviour : MonoBehaviour
{
    public Vector3 targetPos;
    public bool isMoving = false;
    public float maxRange;
    public float minRange;
    public float minWaitTime;
    public float maxWaitTime;
   

    public float speed = 1.5f;
    private NavMeshAgent agent;
    private float range;
    private float waitTime;
    private Vector3 center;
    private Transform transform;
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        transform = this.GetComponent<Transform>();
        agent.speed = speed;
        range = Random.Range(minRange, maxRange);
        waitTime = Random.Range(minWaitTime, maxWaitTime);
    }

    void Update()
    {
        if (isMoving == false)
        {
            //FindNewTargetPos();
            StartCoroutine(Move());
            center = transform.position;
        }
    }

    //private void FindNewTargetPos()
    //{
    //    Vector3 pos = transform.position;
    //    targetPos = new Vector3();
    //    targetPos.x = Random.Range(pos.x - maxRange, pos.x + maxRange);
    //    targetPos.y = pos.y;
    //    targetPos.z = Random.Range(pos.z - maxRange, pos.z + maxRange);

    //    transform.LookAt(targetPos);
    //    StartCoroutine(Move());
    //}

    IEnumerator Move()
    {
        isMoving = true;

        //for (float t = 0.0f; t < 1.0f; t += Time.deltaTime * speed)
        //{

        //    transform.position = Vector3.MoveTowards(transform.position, targetPos, t);
        //    yield return null;
        //}



        //for (float t = 0.0f; t < 1.0f; t += Time.deltaTime * speed)
        //{

        //    agent.SetDestination(targetPos);
        //    yield return null;
        //}


        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime * 5)
        {
                agent.destination = center + (Random.insideUnitSphere * range) ;
     

                if (agent.pathPending || agent.remainingDistance > 0.1f)
                {        
                    yield return null;
                }

        }
        yield return new WaitForSeconds(waitTime);
        isMoving = false;
        yield return null;





    }
}
