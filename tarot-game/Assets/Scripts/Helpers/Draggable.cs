using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public abstract class Draggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Image image;
    public RectTransform rectTransform;
    [SerializeField] protected Transform slot;
    [HideInInspector] public DraggableHolder slotHovered = null;
    [HideInInspector] public DraggableHolder slotSelected = null;
    protected bool canBeDragged = true;
    protected Transform canvas;
    public static Draggable draggedCard;

    public void disableDrag() => canBeDragged = false;
    public void enableDrag() => canBeDragged = true;

    public void darkenImage() => image.color = Color.grey;
    public void whitenImage() => image.color = Color.white;
    
    public void OnBeginDrag(PointerEventData data)
    {
        if (!canBeDragged) return;
        image.raycastTarget = false;
        transform.SetParent(canvas);
        slotSelected?.onBeginDrag(this);
        onBeginDrag();
    }

    protected virtual void onBeginDrag()
    {
        
    }
    
    
    public void OnDrag(PointerEventData data)
    {
        if (!canBeDragged) return;
        transform.position = Input.mousePosition + Vector3.forward;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        slotSelected?.onEndDrag(this);
        if (!canBeDragged) return;
        image.raycastTarget = true;
        Debug.Log($"Slot hovered: {slotHovered?.name}");
        Debug.Log($"Slot selected: {slotSelected?.name}");
        Debug.Log($"Slot hovered item hovering: {slotHovered?.itemHovering?.name}");
        Debug.Log($"Slot hovered item selected: {slotHovered?.itemSelected?.name}");
        Debug.Log($"Slot selected item hovering: {slotSelected?.itemHovering?.name}");
        Debug.Log($"Slot selected item selected: {slotSelected?.itemSelected?.name}");
        
        if (slotHovered == null)
        {
            backToCurrentSlot();
            return;
        }
        if (!slotHovered.isFree(this))
        {
            slotHovered.itemHovering = null;
            slotHovered = null;
            backToCurrentSlot();
            return;
        }
        if (slotSelected != null && slotSelected != slotHovered)
        {
            slotSelected.itemSelected = null;
            slotSelected = null;
        }
        attachToNewSlot(slotHovered);
        slotSelected = slotHovered;
        slotSelected.itemSelected = this;
        slotSelected.onAttachDraggable(this);
        slotHovered.itemHovering = null;
        slotHovered = null;
        onPlaced();
        onEndDrag();
        
    }

    protected virtual void onPlaced()
    {
        
    }

    protected virtual void onEndDrag()
    {
    }
    
    private void attachToNewSlot(DraggableHolder draggableHolder)
    {
        rectTransform.SetParent(draggableHolder.rectTransform);
        rectTransform.anchorMin = 0.5f * Vector2.one;
        rectTransform.anchorMax = 0.5f * Vector2.one;
        rectTransform.anchoredPosition = Vector2.zero;
    }
    
    private void backToCurrentSlot()
    {
        if (!onDrop()) return;
        
        rectTransform.DOMove(slot.position, 0.4f).SetEase(Ease.InOutQuad).OnComplete(delegate
        {
            rectTransform.SetParent(slot, true);
            rectTransform.anchorMin = 0.5f * Vector2.one;
            rectTransform.anchorMax = 0.5f * Vector2.one;
            onEndDrag();
        });
    }

    protected virtual bool onDrop()
    {
        return true;
    }

    private void OnDestroy()
    {
        if (slotSelected != null)
        {
            slotSelected.itemSelected = null;
        }
    }
}
