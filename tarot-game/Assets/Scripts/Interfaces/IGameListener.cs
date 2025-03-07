
using System.Collections.Generic;

public interface IGameListener
{
    public void onHandReceived(List<int> cards);
    public void onCardPlayedByOther(int card);
    public void onCardPlayedByMe(int card);
    public void onPlayerTurn(string username);
    public void onTurnWon(string username);
}
