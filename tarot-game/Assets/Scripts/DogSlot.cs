
using System.Collections.Generic;
using UnityEngine.Events;

public class DogSlot: DraggableHolder
{
    
    public override void onAttachDraggable(Draggable draggable)
    {
        var card = (TarotCard)draggable;
        Hand.instance.removeCard(card.card.index);
    }

    public override void onBeginDrag(Draggable draggable)
    {
        HandZone.instance.gameObject.SetActive(true);
    }

    public override void onEndDrag(Draggable draggable)
    {
        HandZone.instance.gameObject.SetActive(false);
    }
}
