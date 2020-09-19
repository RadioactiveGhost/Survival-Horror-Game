using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



[RequireComponent(typeof(NavMeshAgent))]
public class MobBeaviour : MonoBehaviour
{
   public Vector3 targetPos;
   public bool isMoving = false; //public for debugging purposes
   // public bool isChasing = false; //public for debugging purposes
    public bool isSecondary = false; //wolf pack member or boar child etc
  //  public bool isFleeing = false;
    //public bool isAgressive = false;
    public float maxRange;
    public float minRange;
    public float minWaitTime;
    public float maxWaitTime;
    public float lookradius;
    public float speed;
    private NavMeshAgent agent;
    private float waitTime;
    private Vector3 center;
    //private Transform transform;
    private float rngRest;
    public enum Animal { bear, wolf, boar, deer, bunny, player};
    public Animal thisanimal;
    public enum HunterPrey { hunt, flee, nothing};
    public List<Transform> targets;
    public Transform follower;
    public SphereCollider sphereCollider;
    private Vector3 target = new Vector3(0, 0, 0);
    public bool WolfPack; //wolf
    private GameObject targetSaver = null;
    public enum Action { chasing, fleeing, moving, dead}
    public Action action = Action.moving;
    public float health;
    public float timeLeft = 10;
    public int attackDamage, armor;
    private bool deathRotFlag = false;
    public Animator animator;

    public bool isMother; // boar

    void Start()
    {
        animator = GetComponent<Animator>();

        sphereCollider.radius = this.GetComponent<MobBeaviour>().lookradius * 2;
       
        agent = this.GetComponent<NavMeshAgent>();
        
        agent.speed = speed;
        
        waitTime = Random.Range(minWaitTime, maxWaitTime);

    }

    void Update()
    {
        if(health <= 0)
        {
            action = Action.dead;
            agent.isStopped = true;
            if(deathRotFlag == false)
            {
                transform.Rotate(new Vector3(0, 0, 90));
                deathRotFlag = true;
            }
            //enable looting script!
        }
        if(!(targetSaver == null))
        {
            target = targetSaver.GetComponent<Transform>().position;
            float distance = Vector3.Distance(transform.position, target);

            if (distance > GetComponent<SphereCollider>().radius)
            {
              
                action = Action.moving;
            }
        }
     

        rngRest = Random.Range(0f, 10f);



        switch (action)
        {
            case Action.dead:
                {
                    //laydead
                    if(thisanimal == Animal.bear || thisanimal == Animal.deer || thisanimal == Animal.wolf)
                    animator.SetBool("isWalking", false);
                }
                break;
            case Action.chasing:
                {
                    if (thisanimal == Animal.bear || thisanimal == Animal.deer || thisanimal == Animal.wolf)
                        animator.SetBool("isWalking", true);
                    Chase(target);
                }
                
                break;
            case Action.fleeing:
                {
                    if (thisanimal == Animal.bear || thisanimal == Animal.deer || thisanimal == Animal.wolf)
                        animator.SetBool("isWalking", true);
                    Flee(target);
                }
                
                break;
            case Action.moving:
                if (isMoving == false)
                {
                    
                    center = transform.position;
                    StartCoroutine(Move());
                    agent.speed = speed;
                }
                break;
           
        }


    }

    private void OnTriggerEnter(Collider other)
    {
       if(!(action == Action.dead))
        {
            if (other.CompareTag("Player"))
            {
                if(other.GetComponent<Player>().health > 0)
                {
                    if (HunterAndPrey(Animal.player) == HunterPrey.hunt)
                    {
                        // targetSaver = other.gameObject.transform.position;
                        targetSaver = other.gameObject;
                        action = Action.chasing;
                        isMoving = false;
                    }
                    else if (HunterAndPrey(Animal.player) == HunterPrey.flee)
                    {
                        targetSaver = other.gameObject;
                        action = Action.fleeing;
                        isMoving = false;
                    }
                    else if (HunterAndPrey(Animal.player) == HunterPrey.nothing)
                    {
                        //fazer nada
                    }
                }
                

            }
            else if (other.CompareTag("Animal"))
            {
                if(other.GetComponent<MobBeaviour>().health > 0)
                {
                    if (HunterAndPrey(other.gameObject.GetComponent<MobBeaviour>().thisanimal) == HunterPrey.hunt)
                    {
                        // targetSaver = other.GetComponent<GameObject>().transform.position;

                        targetSaver = other.gameObject;

                        action = Action.chasing;
                        isMoving = false;


                    }
                    else if (HunterAndPrey(other.gameObject.GetComponent<MobBeaviour>().thisanimal) == HunterPrey.flee)
                    {
                        targetSaver = other.gameObject;

                        isMoving = false;
                        action = Action.fleeing;


                    }
                    else if (HunterAndPrey(other.gameObject.GetComponent<MobBeaviour>().thisanimal) == HunterPrey.nothing)
                    {
                        //do nothing
                    }
                }
                
            }
        }
       
        
           
    }


    IEnumerator Move()
    {
        if (thisanimal == Animal.bear || thisanimal == Animal.deer || thisanimal == Animal.wolf)
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
            if (thisanimal == Animal.bear || thisanimal == Animal.deer || thisanimal == Animal.wolf)
                animator.SetBool("isWalking", false);
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
        }
        isMoving = false;
        if (thisanimal == Animal.bear || thisanimal == Animal.deer || thisanimal == Animal.wolf)
            animator.SetBool("isWalking", true);

        //StopCoroutine(Move());


    }
    public void Chase(Vector3 target)
    {
       
        StopCoroutine(Move());
        //isMoving = false;
        float distance = Vector3.Distance(target, transform.position);

            agent.SetDestination(target);
            if (distance <= agent.stoppingDistance)
            {
              timeLeft -= Time.deltaTime;
              if (timeLeft < 0)
              {
                 
                if (targetSaver.CompareTag("Animal"))
                    targetSaver.GetComponent<MobBeaviour>().health -= (attackDamage - targetSaver.GetComponent<MobBeaviour>().armor);
                else if (targetSaver.CompareTag("Player"))
                {
                    targetSaver.GetComponent<Player>().health -= (attackDamage - targetSaver.GetComponent<Player>().armor);
                }
                timeLeft = 1;
            }
           
            }
            FaceTarget(target);
            agent.speed = speed * 2f;

    }
    public void Flee(Vector3 target)
    {
        Vector3 dirToTarget = transform.position - target;
        Vector3 newPosition = transform.position + dirToTarget;
        agent.SetDestination(newPosition);
        agent.speed = speed * 2f;

    }

    private void OnDrawGizmosSelected() //debugging
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookradius);
    }

    void FaceTarget(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

   

    public HunterPrey HunterAndPrey(Animal targetAnimal)
    {
        if (thisanimal == Animal.bear)  // URSO---------------
        {
            if (targetAnimal == Animal.wolf || targetAnimal == Animal.deer || targetAnimal == Animal.boar || targetAnimal == Animal.bunny || targetAnimal == Animal.player)
                return HunterPrey.hunt;
            else return HunterPrey.nothing;
        }
        else if (thisanimal == Animal.wolf) // LOBO----------------
        {
            if (targetAnimal == Animal.boar || targetAnimal == Animal.bunny)
                return HunterPrey.hunt;
            else if (targetAnimal == Animal.player || targetAnimal == Animal.bear || targetAnimal == Animal.deer)
            {
                if (WolfPack) //matilhaaa
                    return HunterPrey.hunt;
                else if (targetAnimal == Animal.player || targetAnimal == Animal.bear)
                    return HunterPrey.flee;
                else return HunterPrey.nothing;
            }
            else return HunterPrey.nothing;
        }
        else if (thisanimal == Animal.deer) // VIADO------------
        {
            if (targetAnimal == Animal.bear || targetAnimal == Animal.wolf)
                return HunterPrey.flee;
            else return HunterPrey.nothing;
        }
        else if (thisanimal == Animal.boar) // POSSSO N VALER NADA AGORA MAS JAVALI----------
        {
            if (targetAnimal == Animal.bear)
                return HunterPrey.flee;
            else if (targetAnimal == Animal.player || targetAnimal == Animal.wolf)
            {
                if (isMother) //attacc to protecc
                    return HunterPrey.hunt;
                else
                    return HunterPrey.flee;
            }
            else return HunterPrey.nothing;
        }
        else if (thisanimal == Animal.bunny) // COELHO-----------------
        {
            if (targetAnimal == Animal.bunny || targetAnimal == Animal.boar || targetAnimal == Animal.deer)
                return HunterPrey.nothing;
        }
        return HunterPrey.nothing;
    }
}
