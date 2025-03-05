using UnityEngine;

public static class ScriptableObjectManager
{
    public static Sprite getIcon(string key)
    {
        Debug.LogError($"Scriptable object with key -{key}- not found");
        return null;
    }
    
    public static void LoadScriptableObjects()
    {

    }
}
