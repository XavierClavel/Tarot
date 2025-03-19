
using System.Collections.Generic;

public interface IAppelListener
{
    public void onAppel(TarotColor color, string username);
    public void onAwaitAppel(string username);
}
