using UnityEngine;
using UnityEngine.EventSystems;

public class MenuNavigationNotifier : MonoBehaviour, IMoveHandler, ISubmitHandler
{
    public void OnMove(AxisEventData eventData)
    {
        switch (eventData.moveDir)
        {
            case MoveDirection.Up:
                MainMenuManager.onMoveUp?.Invoke();
            break;

            case MoveDirection.Down:
                MainMenuManager.onMoveDown?.Invoke();
            break;
        }
    }

    public void OnSubmit(BaseEventData eventData)
    {
        MainMenuManager.onSubmit?.Invoke();
    }
}
