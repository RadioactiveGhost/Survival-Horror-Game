using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



[RequireComponent(typeof(NavMeshAgent))]
public class MobBeaviour : MonoBehaviour
{
    public Vector3 targetPos;
    public bool isMoving = false; //public for debugging purposes
    public bool isChasing = false; //public for debugging purposes
    public bool isAgressive = false;
    public float maxRange;
    public float minRange;
    public float minWaitTime;
    public float maxWaitTime;
    public float lookradius;
    private Transform target;
    public float speed;
    private NavMeshAgent agent;
    private float waitTime;
    private Vector3 center;
    private Transform transform;
    private float rngRest;

    void Start()
    {
        target = PlayerManager.instance.player.transform;
        agent = this.GetComponent<NavMeshAgent>();
        transform = this.GetComponent<Transform>();
        agent.speed = speed;

        waitTime = Random.Range(minWaitTime, maxWaitTime);
    }

    void Update()
    {

        rngRest = Random.Range(0f, 10f);

        Debug.Log(rngRest);
        if (isAgressive) Chase(); //if mob stat- agressive

        if (isMoving == false && isChasing == false ) //wander
        {
            center = transform.position;
            StartCoroutine(Move());
            //Move2();
        }
        //else if(isResting == true && isChasing == false && isMoving == false)
        //{ 
        //    //StopCoroutine(Move());
        //    Rest();
        //    Debug.Log("resting");
        //}
       
        
    }

    //void Rest()
    //{
    //    for(float t = 0.0f; t < 200f; t += Time.deltaTime)
    //    {
    //        agent.isStopped = true;
    //    }
    //    agent.isStopped = false;
    //    isResting = false;
    //}

    //void Move2()
    //{
       
    //    isMoving = true;
    //    agent.destination = center + (Random.insideUnitSphere * range);
    //    for (float t = 0.0f; t < 10f; t += Time.deltaTime * 5)
    //    {
           
    //        if (agent.pathPending || agent.remainingDistance > 0.1f)
    //            break;

    //    }
    //    if (rngRest < 5)
    //    {
    //        isResting = true;
    //    }
    //    isMoving = false;
    //}


    IEnumerator Move()
    {
        agent.destination = center + (Random.insideUnitSphere * (Random.Range(minRange, maxRange)));
        isMoving = true;
        for (float t = 0.0f; t < 10f; t += Time.deltaTime )
        {
            if (agent.pathPending || agent.remainingDistance > 0.1f)
            {
                yield return null;
            }
            
        }
        Debug.Log("fim do yield");
       
        if (rngRest < 5)
        {
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
        }
        isMoving = false;

        //StopCoroutine(Move());
       

    }
    void Chase()
    {
        float distance = Vector3.Distance(target.position, transform.position);

        if (distance <= lookradius)
        {
            isChasing = true;
            agent.SetDestination(target.position);
            if (distance <= agent.stoppingDistance)
            {
                //attack!  ???
            }
            FaceTarget();
            agent.speed = speed * 2f;
        }
        else
        {
            isChasing = false;
            agent.speed = speed;
        }
       
    }

    private void OnDrawGizmosSelected() //debugging
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookradius);
    }

    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

}
