using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

public class Stacker<T>
{
    private Dictionary<T, int> dict = new Dictionary<T, int>();
    private List<UnityAction<T>> onStartStacking = new List<UnityAction<T>>();
    private List<UnityAction<T>> onStopStacking = new List<UnityAction<T>>();
    
    public Stacker<T> addOnStartStackingEvent(UnityAction<T> action)
    {
        onStartStacking.Add(action);
        return this;
    }

    public Stacker<T> addOnStopStackingEvent(UnityAction<T> action)
    {
        onStopStacking.Add(action);
        return this;
    }


    public void stack(T value)
    {
        if (dict.ContainsKey(value)) dict[value]++;
        else
        {
            dict[value] = 1;
            onStartStacking.ForEach(it => it.Invoke(value));
        }
    }

    public void unstack(T value)
    {
        if (!dict.ContainsKey(value)) return;
        dict[value]--;
        if (dict[value] == 0)
        {
            dict.Remove(value);
            onStopStacking.ForEach(it => it.Invoke(value));
        }
    }

    public HashSet<T> get()
    {
        return dict.Keys.ToHashSet();
    }

    public void remove(T value)
    {
        if (!dict.ContainsKey(value))
        {
            return;
        }

        dict.Remove(value);
    }
}

public class SingleStacker
{
    private int amount = 0;
    private List<UnityAction> onStartStacking = new List<UnityAction>();
    private List<UnityAction> onStopStacking = new List<UnityAction>();
    private Dictionary<int, UnityAction> onAboveThreshold = new Dictionary<int, UnityAction>();
    private Dictionary<int, UnityAction> onBelowThreshold = new Dictionary<int, UnityAction>();
    
    public SingleStacker addOnStartStackingEvent(UnityAction action)
    {
        onStartStacking.Add(action);
        return this;
    }

    public SingleStacker addOnStopStackingEvent(UnityAction action)
    {
        onStopStacking.Add(action);
        return this;
    }
    
    public SingleStacker addThresholdAction(int threshold, UnityAction aboveAction, UnityAction belowAction = null)
    {
        onAboveThreshold[threshold] = aboveAction;
        if (belowAction != null)
        {
            onBelowThreshold[threshold] = belowAction;
        }
        return this;
    }

    public void stack()
    {
        if (amount == 0)
        {
            onStartStacking.ForEach(it => it.Invoke());
        }

        amount++;

        foreach (var v in onAboveThreshold.Where(v => v.Key == amount))
        {
            v.Value.Invoke();
        }
    }

    public void unstack()
    {
        foreach (var v in onBelowThreshold.Where(v => v.Key == amount))
        {
            v.Value.Invoke();
        }
        
        amount--;
        if (amount == 0)
        {
            onStopStacking.ForEach(it => it.Invoke());
        }
    }

    public void reset() => amount = 0;
    public int get() => amount;

}

public class SingleStackerSingleThreshold
{
    private int amount = 0;
    private int threshold;
    private List<UnityAction> onThresholdReached = new List<UnityAction>();

    public SingleStackerSingleThreshold addAction(UnityAction action)
    {
        onThresholdReached.Add(action);
        return this;
    }

    public SingleStackerSingleThreshold setThreshold(int value)
    {
        amount = value;
        return this;
    }

    public void stack(int value = 1)
    {
        amount += value;
        if (amount < threshold)
        {
            return;
        }
        
        onThresholdReached.ForEach(it => it.Invoke());
        reset();
    }

    public void unstack(int value = 1)
    {
        amount -= value;
    }
    
    public void reset() => amount = 0;
    public int get() => amount;
}
