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
    public bool isSecondary = false; //wolf pack member or boar child etc
    public bool isFleeing = false;
    //public bool isAgressive = false;
    public float maxRange;
    public float minRange;
    public float minWaitTime;
    public float maxWaitTime;
    public float lookradius;
    public Transform target;
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

    public bool WolfPack; //wolf

    public bool isMother; // boar

    void Start()
    {
        sphereCollider.radius = this.GetComponent<MobBeaviour>().lookradius * 2.5f;
        target = PlayerManager.instance.player.transform;
        agent = this.GetComponent<NavMeshAgent>();
        
        agent.speed = speed;
        
        waitTime = Random.Range(minWaitTime, maxWaitTime);

        thisanimal = Animal.boar;
    }

    void Update()
    {

        rngRest = Random.Range(0f, 10f);

        Debug.Log(rngRest);
       
        //Chase();

        //if (isFleeing)
        //    agent.speed = speed * 2f;
        //else
        //    agent.speed = speed;


        if (isMoving == false && isChasing == false && isFleeing == false) //wander
        {
            center = transform.position;
            StartCoroutine(Move());
           
        }
       
        
    }

    private void OnTriggerEnter(Collider other)
    {
        target.position = other.gameObject.transform.position;
        if (HunterAndPrey(other.gameObject.GetComponent<MobBeaviour>().thisanimal) == HunterPrey.hunt)
        {
            Chase();
            isChasing = true;
        }
        else if(HunterAndPrey(other.gameObject.GetComponent<MobBeaviour>().thisanimal) == HunterPrey.flee)
        {
            Flee();
            isFleeing = true;
        }
        else if(HunterAndPrey(other.gameObject.GetComponent<MobBeaviour>().thisanimal) == HunterPrey.nothing)
        {
            //do nothing
        }
        else
        {
            isChasing = false;
            isFleeing = false;
            agent.speed = speed;
        }
           
    }

    IEnumerator Move()
    {
        if(isSecondary) //ir atras da mae ou do leader de pack etc
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
    void Chase()
    {
        StopCoroutine(Move());
        float distance = Vector3.Distance(target.position, transform.position);

            agent.SetDestination(target.position);
            if (distance <= agent.stoppingDistance)
            {
                //attack!  ??? todo: atacar random inimigo q esta na area do lookradius e ta na lista de targets - also criar a lista primeiro..
            }
            FaceTarget();
            agent.speed = speed * 2f;

    }
    public void Flee()
    {
        StopCoroutine(Move());
        float distance = Vector3.Distance(transform.position, target.position);

        Vector3 dirToTarget = transform.position - target.position;
        Vector3 newPosition = transform.position + dirToTarget;
        agent.SetDestination(newPosition);
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

   

    public HunterPrey HunterAndPrey(Animal targetAnimal)
    {
        if (thisanimal == Animal.bear)  // URSO
        {
            if (targetAnimal == Animal.wolf || targetAnimal == Animal.deer || targetAnimal == Animal.boar || targetAnimal == Animal.bunny || targetAnimal == Animal.player)
                return HunterPrey.hunt;
            else return HunterPrey.nothing;
        }
        else if (thisanimal == Animal.wolf) // LOBO
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
        else if (thisanimal == Animal.deer) // VIADO
        {
            if (targetAnimal == Animal.bear || targetAnimal == Animal.wolf)
                return HunterPrey.flee;
            else return HunterPrey.nothing;
        }
        else if (thisanimal == Animal.boar) // POSSSO N VALER NADA AGORA MAS JAVALI
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
        else if (thisanimal == Animal.bunny) // COELHO
        {
            if (targetAnimal == Animal.bunny || targetAnimal == Animal.boar || targetAnimal == Animal.deer)
                return HunterPrey.nothing;
        }
        return HunterPrey.nothing;
    }
}
