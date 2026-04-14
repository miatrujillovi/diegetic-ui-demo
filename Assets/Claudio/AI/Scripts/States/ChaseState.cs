public class ChaseState : IState
{
    private EnemyBrain brain;

    public ChaseState(EnemyBrain brain)
    {
        this.brain = brain;
    }

    public void Enter()
    {
        var agent = brain.GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.speed = 15f; // más rápido

        brain.enemyUI.ShowExclamation();
    }

    public void Execute()
    {
        brain.GetComponent<MovementController>()
             .MoveTo(brain.player.position);

       

    }

    public void Exit() { }
}