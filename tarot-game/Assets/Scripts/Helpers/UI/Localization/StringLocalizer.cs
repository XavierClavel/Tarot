using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class StringLocalizer : MonoBehaviour
{
    [SerializeField] string key = null;
    private TextMeshProUGUI textDisplay = null;
    private LocalizedString localizedString;

    public void setKey(string key)
    {
        this.key = key;
        Setup();
    }

    private void OnEnable()
    {
        if (!DataManager.isInitialized()) return;
        Setup();
    }

    private void Start()
    {
        if (!DataManager.isInitialized()) return;
        Setup();
    }

    public void Setup()
    {
        if (textDisplay == null)
        {
            textDisplay = GetComponent<TextMeshProUGUI>();
            if (textDisplay == null)
            {
                Debug.LogError($"GameObject {gameObject.name} does not have TextMeshProUGUI component");
                return;
            }
            LocalizationManager.registerStringLocalizer(this);
        }
        if (key != null && key.Length > 0) UpdateKey();
    }

    private void UpdateKey()
    {
        if (!DataManager.dictLocalization.ContainsKey(key))
        {
            Debug.LogError($"{gameObject.name} is trying to call the \"{key}\" key which does not exist.");
            return;
        }
        localizedString = DataManager.dictLocalization[key];
        textDisplay.SetText(localizedString.getText());
    }

    private void OnDestroy()
    {
        LocalizationManager.unregisterStringLocalizer(this);
    }
}
