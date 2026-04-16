using UnityEngine;

public class DebugManager : MonoBehaviour
{
    //THIS SCRIPT IS ONLY FOR DEBUGGING PURPOSES

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

    //Normal Debug.Log
    public void Log(string _text, bool _activate, string _script)
    {
        if (!_activate) return;

        Debug.Log(_script + " " + _text);
    }

    //Error Debug.LogError
    public void LogError(string _text, bool _activate, string _script)
    {
        if (!_activate) return;

        Debug.LogError(_script + " " + _text);
    }
}
