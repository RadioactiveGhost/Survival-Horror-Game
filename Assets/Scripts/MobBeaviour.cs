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
    //public bool isAgressive = false;
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
    public enum Animal { bear, wolf, boar, deer, bunny, player};
    public Animal thisanimal;
    public List<Transform> targets;

    public bool WolfPack; //wolf

    public bool isMother; // boar

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
        //Specificbehaviours();
        Chase(); 

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
        StopCoroutine(Move());
        float distance = Vector3.Distance(target.position, transform.position);

        if (distance <= lookradius)
        {
            isChasing = true;
            agent.SetDestination(target.position);
            if (distance <= agent.stoppingDistance)
            {
                //attack!  ??? todo: atacar random inimigo q esta na area do lookradius e ta na lista de targets - also criar a lista primeiro..
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

    //void Specificbehaviours()
    //{
    //    if (thisanimal == Animal.bear)
    //    {
    //        this.isAgressive = true;
    //    }
    //    else if (thisanimal == Animal.wolf)
    //    {
    //        this.isAgressive = true;
    //    }
    //    else if (thisanimal == Animal.deer)
    //    {
    //        this.isAgressive = false;
    //    }
    //    else if (thisanimal == Animal.boar)
    //    {
    //        this.isAgressive = false; ;
    //    }
    //    else if (thisanimal == Animal.bunny)
    //    {
    //        this.isAgressive = false;
    //    }
    //}

    bool IsPrey(Animal targetAnimal)
    {
        if (thisanimal == Animal.bear)  // URSO
        {
            if (targetAnimal == Animal.wolf || targetAnimal == Animal.deer || targetAnimal == Animal.boar || targetAnimal == Animal.bunny || targetAnimal == Animal.player)
                return true;
            else return false;
        }
        else if (thisanimal == Animal.wolf) // LOBO
        {
            if (targetAnimal == Animal.boar || targetAnimal == Animal.bunny)
                return true;
            else if (targetAnimal == Animal.player || targetAnimal == Animal.bear || targetAnimal == Animal.deer)
            {
                if (WolfPack)
                    return true;
            }
            else return false;
        }
        else if (thisanimal == Animal.deer) // VIADO
        {
            return false;
        }
        else if (thisanimal == Animal.boar) // POSSSO N VALER NADA AGORA MAS JAVALI
        {
            if (targetAnimal == Animal.player || targetAnimal == Animal.wolf && isMother)
            {
                return true;
            }
            else return false;
        }
        else if (thisanimal == Animal.bunny) // COELHO
        {
            return false;
        }
        return false;
    }
}
