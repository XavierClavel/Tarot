
using System;

public class DropZone: DraggableHolder
{
    public static DropZone instance;
    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    public override void onAttachDraggable(Draggable draggable)
    {
        TarotCard card = (TarotCard)draggable;
        Hand.playCard(card.card.index);
        destroyAttachedDraggable();
    }
}