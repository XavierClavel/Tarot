
public static class EventManagers
{
    public static EventManager<IPlayerListener> player = new EventManager<IPlayerListener>();
    public static EventManager<IGameListener> game = new EventManager<IGameListener>();
    public static EventManager<IBidListener> bid = new EventManager<IBidListener>();
    public static EventManager<ITurnListener> turn = new EventManager<ITurnListener>();
    public static EventManager<IDogListener> dog = new EventManager<IDogListener>();
    public static EventManager<IAppelListener> appel = new EventManager<IAppelListener>();
}
