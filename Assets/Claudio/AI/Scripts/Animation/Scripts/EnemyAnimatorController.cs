using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    private Animator anim;
    private UnityEngine.AI.NavMeshAgent agent;

    void Awake()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    void Update()
    {
        float speed = agent.velocity.magnitude / agent.speed;
        anim.SetFloat("Speed", speed);
    }

    public void SetInvestigating(bool value)
    {
        anim.SetBool("IsInvestigating", value);
    }

    public void SetChasing(bool value)
    {
        anim.SetBool("IsChasing", value);
    }

    public void TriggerAlert()
    {
        anim.SetTrigger("Alert");
    }
}