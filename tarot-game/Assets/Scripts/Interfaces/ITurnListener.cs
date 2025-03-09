
public interface ITurnListener
{
    public void onPlayerTurn(string username);
    public void onMyTurnStart();
    public void onMyTurnEnd();
    public void onTurnWon(string username);
}
