using UnityEngine;

public class PatrolState : IState
{
    private EnemyBrain brain;
    private MovementController movement;
    private PatrolPath path;

    private int currentPoint = 0;

    private float waitTimer = 0f;
    private bool isWaiting = false;

    public PatrolState(EnemyBrain brain)
    {
        this.brain = brain;
    }

    public void Enter()
    {
        movement = brain.GetComponent<MovementController>();
        path = brain.patrolPath; // usar su propio path

        MoveToNextPoint();
    }

    public void Execute()
    {
        // 🔴 COMBATE
        if (brain.alert.currentLevel == AlertLevel.Combat)
        {
            brain.stateMachine.ChangeState(new ChaseState(brain));
            return;
        }

        // 🟡 INVESTIGAR
        if (brain.alert.currentLevel == AlertLevel.Suspicious)
        {
            brain.stateMachine.ChangeState(new InvestigateState(brain));
            return;
        }

        var agent = movement.GetComponent<UnityEngine.AI.NavMeshAgent>();

        // 🚶 SI LLEGÓ AL PUNTO
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            if (!isWaiting)
            {
                StartWaiting();
            }
        }

        // ⏱️ SI ESTÁ ESPERANDO
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0f)
            {
                isWaiting = false;
                MoveToNextPoint();
            }
        }
        brain.enemyUI.HideAlerts();
    }

    void StartWaiting()
    {
        isWaiting = true;

        PatrolPoint point = path.GetPoint(currentPoint);

        waitTimer = point.waitTime;

        // Detener movimiento
        movement.Stop();
    }

    void MoveToNextPoint()
    {
        if (path == null || path.Length == 0) return;

        PatrolPoint point = path.GetPoint(currentPoint);

        movement.MoveTo(point.transform.position);

        currentPoint = (currentPoint + 1) % path.Length;
    }

    public void Exit()
    {
        isWaiting = false;
    }
}