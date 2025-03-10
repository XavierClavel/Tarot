using TreeEditor;
using UnityEngine;
using UnityEngine.UI;

public class TarotCard : Draggable
{
    public static TarotCard draggedCard;
    public Card card;

    public void setValue(int id)
    {
        card = new Card(id);
        image.sprite = Hand.getSprite(id);
        name = card.toString();
    }
    public void setup(RectTransform canvas, RectTransform slot)
    {
        this.canvas = canvas;
        this.slot = slot;
    }

    protected override void onBeginDrag()
    {
        draggedCard = this;
        DropZone.instance.gameObject.SetActive(true);
    }

    protected override void onEndDrag()
    {
        draggedCard = null;
        DropZone.instance.gameObject.SetActive(false);
    }

    protected override void onPlaced()
    {
        LobbyManager.sendWebSocketMessage(new CardPlayed(card.index));
    }
}
