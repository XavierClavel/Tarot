using UnityEngine;
using UnityEngine.UI;

public class TarotCard : Draggable
{
    public void setup(RectTransform canvas, RectTransform slot)
    {
        this.canvas = canvas;
        this.slot = slot;
    }
}
