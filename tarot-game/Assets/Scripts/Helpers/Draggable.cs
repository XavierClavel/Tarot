using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public abstract class Draggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Image image;
    public RectTransform rectTransform;
    [SerializeField] protected RectTransform slot;
    [HideInInspector] public DraggableHolder hoverDraggableHolder = null;
    [HideInInspector] public DraggableHolder selectedDraggableHolder = null;
    protected bool canBeDragged = true;
    protected RectTransform canvas;

    public void disableDrag() => canBeDragged = false;
    public void enableDrag() => canBeDragged = true;

    public void darkenImage() => image.color = Color.grey;
    public void whitenImage() => image.color = Color.white;
    
    public void OnBeginDrag(PointerEventData data)
    {
        if (!canBeDragged) return;
        image.raycastTarget = false;
        transform.SetParent(canvas);
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
        if (!canBeDragged) return;
        image.raycastTarget = true;
        
        if (hoverDraggableHolder == null)
        {
            AttachToSlot();
            return;
        } else if (!hoverDraggableHolder.isFree(this))
        {
            hoverDraggableHolder.hoverDraggable = null;
            hoverDraggableHolder = null;
            AttachToSlot();
            return;
        }
        else
        {
            if (selectedDraggableHolder != null && selectedDraggableHolder != hoverDraggableHolder)
            {
                selectedDraggableHolder.selectedDraggable = null;
                selectedDraggableHolder = null;
            }
            AttachToDraggableHolder(hoverDraggableHolder);
            selectedDraggableHolder = hoverDraggableHolder;
            selectedDraggableHolder.selectedDraggable = this;
            selectedDraggableHolder.onAttachDraggable(this);
            hoverDraggableHolder.hoverDraggable = null;
            hoverDraggableHolder = null;
            onPlaced();
            onEndDrag();
        }
        
    }

    protected virtual void onPlaced()
    {
        
    }

    protected virtual void onEndDrag()
    {
    }
    
    private void AttachToDraggableHolder(DraggableHolder draggableHolder)
    {
        rectTransform.SetParent(draggableHolder.rectTransform);
        rectTransform.anchorMin = 0.5f * Vector2.one;
        rectTransform.anchorMax = 0.5f * Vector2.one;
        rectTransform.anchoredPosition = Vector2.zero;
    }
    
    private void AttachToSlot()
    {
        if (!onDrop()) return;
        
        rectTransform.DOMove(slot.position, 0.4f).SetEase(Ease.InOutQuad).OnComplete(delegate
        {
            if (selectedDraggableHolder != null)
            {
                selectedDraggableHolder.selectedDraggable = null;
                selectedDraggableHolder = null;
            }
            
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
}
