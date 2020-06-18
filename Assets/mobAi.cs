using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class mobAi : MonoBehaviour
{


    public float maxRange;
    public float minRange;
    public float minWaitTime;
    public float maxWaitTime;
    public float lookradius;
    public float speed;
    private NavMeshAgent agent;
    private float waitTime;
    private Vector3 center;
    public Transform follower;
    private float rngRest;
    public GameObject player;

    private bool playerLow = false;

    public bool isMoving = false; 
    
    public bool isSecondary = false;

    public enum Action { chasing, fleeing, moving, other }
    public Action action = Action.moving;
    public float health;
    public enum Type { lookPhobic, movimentSensor, huntinPack, darknessCrawler, bloodLeecher}
    public Type type;

    private GameObject targetSaver; 
    private Vector3 target = new Vector3(0, 0, 0);
    public bool bIsOnTheMove = false;

    private IEnumerator CheckMoving(GameObject entity)
    {
        Vector3 startPos = entity.GetComponent<Transform>().position;
        yield return new WaitForSeconds(1f);
        Vector3 finalPos = entity.GetComponent<Transform>().position;

        if (startPos.x != finalPos.x || startPos.y != finalPos.y
            || startPos.z != finalPos.z)
            bIsOnTheMove = true;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //targetSaver = other.gameObject.transform.position
            targetSaver = other.gameObject;
        }
        
    }


    // Start is called before the first frame update
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();

        agent.speed = speed;

        waitTime = Random.Range(minWaitTime, maxWaitTime);
    }

    // Update is called once per frame
    void Update()
    {
        




        if (!(targetSaver == null))
        {
           
            target = targetSaver.GetComponent<Transform>().position;
            float distance = Vector3.Distance(transform.position, target);

            if (distance > GetComponent<SphereCollider>().radius)
            {
                // isChasing = false;
                // isFleeing = false;
                action = Action.moving;
            }
            else if (distance <= GetComponent<SphereCollider>().radius)
            {
                switch (type)
                {

                    case Type.bloodLeecher:
                        if (CheckPlayerHP() < 40)
                        {
                            action = Action.chasing;
                            isMoving = false;
                        }
                        else
                        {
                            action = Action.moving;

                        }
                        break;
                    case Type.movimentSensor:
                        CheckMoving(player);
                        if (bIsOnTheMove)
                        {
                            action = Action.chasing;
                            isMoving = false;
                        }
                        break;
                    case Type.lookPhobic:
                        break;
                    case Type.huntinPack:
                        break;
                    case Type.darknessCrawler:
                        //check if lightsource in radius
                        break;



                }
            }



            


        }



        rngRest = Random.Range(0f, 10f);


        if (action == Action.chasing)
        {
            Chase(target);
        }
        else if (action == Action.fleeing)
        {
            Flee(target);
        }
        else if (action == Action.moving && isMoving == false)
        {
            center = transform.position;
            StartCoroutine(Move());
            agent.speed = speed;
        }

       
    }

    public void Chase(Vector3 target)
    {
        StopCoroutine(Move());
        //isMoving = false;
        float distance = Vector3.Distance(target, transform.position);

        agent.SetDestination(target);
        if (distance <= agent.stoppingDistance)
        {
            //attack!  ??? todo: atacar random inimigo q esta na area do lookradius e ta na lista de targets - also criar a lista primeiro..
        }
        FaceTarget(target);
        agent.speed = speed * 2f;

    }

    IEnumerator Move()
    {
        if (isSecondary) //ir atras da mae ou do leader de pack etc
        {
            agent.destination = follower.position;
        }
        else
            agent.destination = center + (Random.insideUnitSphere * (Random.Range(minRange, maxRange))); //andar randomly por aí

        isMoving = true;
        for (float t = 0.0f; t < 10f; t += Time.deltaTime)
        {
            if (agent.pathPending || agent.remainingDistance > 0.1f)
            {
                yield return null;
            }

        }
        if (rngRest < 5)
        {
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
        }
        isMoving = false;

        //StopCoroutine(Move());


    }

    public void Flee(Vector3 target)
    {
        Vector3 dirToTarget = transform.position - target;
        Vector3 newPosition = transform.position + dirToTarget;
        agent.SetDestination(newPosition);
        agent.speed = speed * 2f;

    }

    void FaceTarget(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    int CheckPlayerHP()
    {
        return player.GetComponent<Player>().health;
    }

}
