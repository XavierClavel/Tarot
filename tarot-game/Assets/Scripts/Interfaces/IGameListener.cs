
using System.Collections.Generic;

public interface IGameListener
{
    public void onHandReceived(List<int> cards);
    public void onCardPlayed(string username, int card);
    public void onCardPlayedByOther(string username, int card);
    public void onCardPlayedByMe(int card);
}
