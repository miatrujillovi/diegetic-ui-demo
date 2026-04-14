using UnityEngine;

public class IdleState : IState
{
    private EnemyBrain brain;
    private MovementController movement;
    private Animator anim;

    public IdleState(EnemyBrain brain)
    {
        this.brain = brain;
    }

    public void Enter()
    {
        movement = brain.GetComponent<MovementController>();
        anim = brain.GetComponent<Animator>();

        // detener completamente
        movement.Stop();

        // animación idle (opcional)
        if (anim != null)
            anim.SetBool("IsIdle", true);

        brain.enemyUI.HideAlerts();
    }

    public void Execute()
    {
        // SI ESCUCHA  INVESTIGA
        if (brain.hearing.CanHearSomething())
        {
            brain.memory.SetLastKnownPosition(brain.hearing.LastHeardPosition);
            brain.stateMachine.ChangeState(new InvestigateState(brain));
            return;
        }

        //  OPCIONAL: si lo ve  combate directo
        var vision = brain.vision.CheckVision(brain.player);

        if (vision == VisionSensor.VisionResult.Clear)
        {
            brain.stateMachine.ChangeState(new ChaseState(brain));
        }
    }

    public void Exit()
    {
        if (anim != null)
            anim.SetBool("IsIdle", false);
    }
}