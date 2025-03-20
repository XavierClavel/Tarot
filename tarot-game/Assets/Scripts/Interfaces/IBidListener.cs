
public interface IBidListener
{
    public void onAwaitBid(string username);
    public void onBidMade(string username, Bid bid);
    public void onBidWon(string username, Bid bid);
}
