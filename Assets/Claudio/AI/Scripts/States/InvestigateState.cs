using UnityEngine;

public class InvestigateState : IState
{
    private EnemyBrain brain;
    private MovementController movement;

    private float timer = 5f;
    private float lookTimer;

    private Animator anim;

    public InvestigateState(EnemyBrain brain)
    {
        this.brain = brain;
    }

    public void Enter()
    {
        movement = brain.GetComponent<MovementController>();

        var agent = movement.GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.speed = 2f; //  lento (modo cauteloso)

        movement.MoveTo(brain.memory.LastKnownPosition);

        lookTimer = 0f;

        anim = brain.GetComponent<Animator>();

        anim.SetBool("IsInvestigating", true);

        brain.enemyUI.ShowQuestion();
    }

    public void Execute()
    {
        var agent = movement.GetComponent<UnityEngine.AI.NavMeshAgent>();

        // 🔴 PRIORIDAD ABSOLUTA → SI LO VE, CAMBIA YA
        var visionResult = brain.vision.CheckVision(brain.player);

        if (visionResult == VisionSensor.VisionResult.Clear)
        {
            StopLooking();
            brain.stateMachine.ChangeState(new ChaseState(brain));
            return;
        }

        // 🟡 también si ya subió alerta
        if (brain.alert.currentLevel == AlertLevel.Combat)
        {
            StopLooking();
            brain.stateMachine.ChangeState(new ChaseState(brain));
            return;
        }

        // ⏱️ llegó al punto
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            SearchAround();
        }

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            StopLooking();
            brain.memory.ClearMemory();
            brain.stateMachine.ChangeState(new PatrolState(brain));
        }
    }
    void SearchAround()
    {
        lookTimer -= Time.deltaTime;

        if (lookTimer <= 0f)
        {
            float randomAngle = Random.Range(-120f, 120f);
            brain.transform.Rotate(0, randomAngle, 0);

            lookTimer = 1.5f;
        }
    }
    void StopLooking()
    {
        if (anim != null)
            anim.SetBool("IsInvestigating", false);
    }

    public void Exit()
    {
        anim.SetBool("IsInvestigating", false);
    }
}
