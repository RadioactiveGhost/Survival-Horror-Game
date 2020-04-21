using UnityEngine;
using UnityEngine.AI;

// Walk to a random position and repeat
[RequireComponent(typeof(NavMeshAgent))]
public class RandomWalk : MonoBehaviour
{
    public float m_Range = 25.0f;
    NavMeshAgent m_Agent;
    public float speed;

    void Start()
    {
        
        m_Agent = GetComponent<NavMeshAgent>();
        m_Agent.speed = speed;
    }

    void Update()
    {
        if (m_Agent.pathPending || m_Agent.remainingDistance > 0.1f)
            return;
        
        m_Agent.destination = m_Range * Random.insideUnitCircle;
    }
}
