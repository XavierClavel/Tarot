using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DataManager", menuName = Vault.other.scriptableObjectMenu + "DataManager", order = 0)]
public class DataManager : ScriptableObject
{
    [SerializeField] List<TextAsset> localizationData;
    
    public static Dictionary<string, LocalizedString> dictLocalization = new Dictionary<string, LocalizedString>();
    private static DataManager instance;
    private static bool initialized = false;

    
    public void LoadData()
    {
        if (instance != null) return;
        instance = this;
        
        ScriptableObjectManager.LoadScriptableObjects();
        
        LocalizedStringBuilder localizedStringBuilder = new LocalizedStringBuilder();

        foreach (TextAsset data in localizationData)
        {
            localizedStringBuilder.loadText(data, ref dictLocalization, $"Localization/{data.name}");
        }
        
        initialized = true;
    }

    public static bool isInitialized() => initialized;

}

