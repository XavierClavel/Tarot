
using System;

public class HandZone: DraggableHolder
{
    public static HandZone instance;
    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    public override void onAttachDraggable(Draggable draggable)
    {
        var card = (TarotCard)draggable;
        Hand.instance.addCard(card.card.index);
        Destroy(draggable.gameObject);
        itemSelected = null;
        itemHovering = null;
    }
}