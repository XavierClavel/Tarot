
using TMPro;
using UnityEngine;

public class PlayerBid: MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI username;
    [SerializeField] private TextMeshProUGUI bid;

    public void setUsername(string username) => this.username.text = username;

    public void setBid(Bid bid)
    {
        this.bid.text = bid.ToString();
    } 
}
