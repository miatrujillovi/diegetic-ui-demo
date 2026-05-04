using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class TimelineEndHandler : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    [SerializeField] private bool goodEnding;
    [SerializeField] private bool badEnding;

    void OnEnable()
    {
        director.stopped += OnTimelineStopped;
    }

    void OnDisable()
    {
        director.stopped -= OnTimelineStopped;
    }

    void OnTimelineStopped(PlayableDirector pd)
    {
        Debug.Log("Timeline finished");

        if (badEnding)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        if (goodEnding)
        {
            SceneManager.LoadScene("CasaFinal");
        }
    }
}
