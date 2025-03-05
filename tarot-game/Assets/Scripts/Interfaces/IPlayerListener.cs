
using System.Collections.Generic;

public interface IPlayerListener
{
    public void onPlayerJoin(List<string> users);
    
    public void onPlayerLeft(string username);
}
