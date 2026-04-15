using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private IState current;

    public void ChangeState(IState next)
    {
        current?.Exit();
        current = next;
        current?.Enter();
    }

    void Update()
    {
        current?.Execute();
    }
}