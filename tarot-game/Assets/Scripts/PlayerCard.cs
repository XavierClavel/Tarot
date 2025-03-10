using TMPro;
using UnityEngine;

public class PlayerCard : MonoBehaviour
{
    [SerializeField] private TarotCard card;
    [SerializeField] private TextMeshProUGUI usernameDisplay;

    public void setup(int cardId, string username)
    {
        card.disableDrag();
        card.setValue(cardId);
        
        usernameDisplay.SetText(username);
    }
    


}
