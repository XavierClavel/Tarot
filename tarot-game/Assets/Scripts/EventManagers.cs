
public static class EventManagers
{
    public static EventManager<IPlayerListener> player = new();
    public static EventManager<IGameListener> game = new();
    public static EventManager<IBidListener> bid = new();
    public static EventManager<ITurnListener> turn = new();
    public static EventManager<IDogListener> dog = new();
    public static EventManager<IAppelListener> appel = new();
    public static EventManager<IFausseDonneListener> fausseDonne = new();
}
