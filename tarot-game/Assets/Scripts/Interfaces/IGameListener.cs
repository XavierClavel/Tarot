
using System.Collections.Generic;

public interface IGameListener
{
    public void onHandReceived(List<int> cards);
    public void onCardPlayedByOther(string username, int card);
    public void onCardPlayedByMe(int card);
    public void onFirstTurn(string username);
}
