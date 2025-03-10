
using System;

public class DropZone: DraggableHolder
{
    public static DropZone instance;
    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    protected override Draggable getSelectedDraggable()
    {
        return TarotCard.draggedCard;
    }

    public override void onAttachDraggable(Draggable draggable)
    {
        TarotCard card = (TarotCard)draggable;
        Hand.playCard(card.card.index);
        destroyAttachedDraggable();
    }
}