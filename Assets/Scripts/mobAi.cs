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
    private GameObject player;

    public float timeLeft = 7, attackTimeLeft;

    public bool isStaring = false;



    public bool isMoving = false; 
    
    public bool isSecondary = false;

    public enum Action { chasing, fleeing, moving, immobilized, other }
    public Action action = Action.moving;
    public float health;
    public enum Type { lookPhobic, movimentSensor, huntinPack, darknessCrawler, bloodLeecher}
    public Type type;

    private GameObject targetSaver; 
    public Vector3 target = new Vector3(0, 0, 0);
    private Animator animator;
    public int attackDamage, armor;
    public float attackSpeed;
    public bool onPlayerSight = false;




    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !(action == Action.immobilized))
        {
            if(other.GetComponent<Player>().health > 0)
            targetSaver = other.gameObject;
        }
        
    }

    void Start()
    {
        player = GameObject.Find("Player");
        if (type == Type.huntinPack)
           gameObject.GetComponent<packCheck>().enabled = true;
        animator = this.GetComponent<Animator>();
        
        agent = this.GetComponent<NavMeshAgent>();

        agent.speed = speed;

        waitTime = Random.Range(minWaitTime, maxWaitTime);
    }

    // Update is called once per frame
    void Update()
    {
       if(attackTimeLeft > 0)
       {
            attackTimeLeft -= Time.deltaTime;
       }


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
                        if (CheckPlayerMotion())
                        {
                            timeLeft = 7;
                            action = Action.chasing;
                            isMoving = false;
                            isStaring = true;
                        }
                        else
                        {
                            isMoving = false;
                            action = Action.immobilized;
                            //timer com reset - > creepyLook
                            timeLeft -= Time.deltaTime;
                            

                            isStaring = true;
                            if (timeLeft < 0)
                            {
                                action = Action.moving;
                                isStaring = false;
                            }
                           

                            
                        }
                        break;
                    case Type.lookPhobic:
                        if(!onPlayerSight)
                        {
                            action = Action.chasing;
                            isMoving = false;
                        }
                        else
                        {
                            action = Action.moving;
                        }
                        break;
                    case Type.huntinPack:
                        if(GetComponent<packCheck>().packFormed)
                        {
                            action = Action.chasing;
                            isMoving = false;
                        }
                        else
                        {
                            action = Action.moving;
                        }
                           
                        break;
                    case Type.darknessCrawler:
                        //check if lightsource in radius
                        break;
                }
            }
        }
        


        rngRest = Random.Range(0f, 10f);

        if(action == Action.immobilized)
        {
            animator.SetBool("isWalking", false);
            agent.isStopped = true;
        }
        else if (action == Action.chasing)
        {
            agent.isStopped = false;
            Chase(target);
            animator.SetBool("isWalking", true);

        }
        else if (action == Action.fleeing)
        {
            agent.isStopped = false;
            Flee(target);
            animator.SetBool("isWalking", true);
        }
        else if (action == Action.moving && isMoving == false)
        {
            agent.isStopped = false;
            center = transform.position;
            StartCoroutine(Move());
            agent.speed = speed;
        }

       
    }

    

    private bool CheckPlayerMotion()
    {
        if (player.GetComponent<Rigidbody>().velocity.magnitude < 2)
            return false;   
        else
            return true;
    }
    //private IEnumerator CheckifSelfMoving()
    //{
    //    Vector3 startPos = this.GetComponent<Transform>().position;
    //    yield return new WaitForSeconds(1f);
    //    Vector3 finalPos = this.GetComponent<Transform>().position;

    //    if (startPos.x != finalPos.x || startPos.y != finalPos.y
    //        || startPos.z != finalPos.z)
    //        selfIsOnTheMove = true;
    //    else
    //    {
    //        selfIsOnTheMove = false;
    //    }
    //}

    public void Chase(Vector3 target)
    {
        StopCoroutine(Move());
        //isMoving = false;
        float distance = Vector3.Distance(target, transform.position);

        agent.SetDestination(target);
        if (distance <= agent.stoppingDistance)
        {
            if (attackTimeLeft <= 0)
            {
                targetSaver.GetComponent<Player>().health -= (attackDamage - targetSaver.GetComponent<Player>().armor);
                attackTimeLeft = 1 / attackSpeed;
            }
            
        }
        FaceTarget(target);
        agent.speed = speed * 2f;

    }

    IEnumerator Move()
    {
        animator.SetBool("isWalking", true);
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
            animator.SetBool("isWalking", false);
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
        }
        isMoving = false;
        animator.SetBool("isWalking", true);

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
