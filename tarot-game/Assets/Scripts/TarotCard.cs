using TreeEditor;
using UnityEngine;
using UnityEngine.UI;

public class TarotCard : Draggable
{
    public Card card;

    public void setValue(int id)
    {
        card = new Card(id);
        image.sprite = Hand.getSprite(id);
        name = card.toString();
    }
    public void setup(Transform canvas, DraggableHolder slot)
    {
        this.canvas = canvas;
        this.slot = slot.transform;
        slotSelected = slot;
        slotSelected.itemSelected = this;
    }

    protected override void onBeginDrag()
    {
        draggedCard = this;
        if (!DogManager.dogPhase)
        {
            DropZone.instance.gameObject.SetActive(true);   
        }
    }

    protected override void onEndDrag()
    {
        draggedCard = null;
        DropZone.instance.gameObject.SetActive(false);
    }

    protected override void onPlaced()
    {
        
    }
}
