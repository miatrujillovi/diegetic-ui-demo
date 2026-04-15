using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public static DebugManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Log(string _text, bool _activate, string _script)
    {
        if (!_activate) return;

        Debug.Log(_script + " " + _text);
    }

    public void LogError(string _text, bool _activate, string _script)
    {
        if (!_activate) return;

        Debug.LogError(_script + " " + _text);
    }
}
