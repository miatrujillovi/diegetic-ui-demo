using UnityEngine;

public class EnemyUI : MonoBehaviour
{
    public GameObject alertQuestion;
    public GameObject alertExclamation;

    public void ShowQuestion()
    {
        alertQuestion.SetActive(true);
        alertExclamation.SetActive(false);
    }

    public void ShowExclamation()
    {
        alertQuestion.SetActive(false);
        alertExclamation.SetActive(true);
    }

    public void HideAlerts()
    {
        alertQuestion.SetActive(false);
        alertExclamation.SetActive(false);
    }
}