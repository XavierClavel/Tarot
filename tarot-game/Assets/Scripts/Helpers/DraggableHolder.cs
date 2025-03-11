using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class DraggableHolder : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public Draggable itemHovering;
    [HideInInspector] public Draggable itemSelected;
    public RectTransform rectTransform;
    
    public void OnPointerEnter(PointerEventData e)
    {
        Draggable draggable = Draggable.draggedCard;
        if (draggable == null) return;
        itemHovering = draggable;
        itemHovering.slotHovered = this;
        onPointerEnter();
    }

    
    public void OnPointerExit(PointerEventData e)
    {
        if (itemHovering == null) return;
        onPointerExit();
        itemHovering.slotHovered = null;
        itemHovering = null;
    }

    protected virtual void onPointerEnter()
    {
        
    }

    protected virtual void onPointerExit()
    {
        
    }

    public virtual void onAttachDraggable(Draggable draggable)
    {
        
    }

    protected virtual void destroyAttachedDraggable()
    {
        if (itemSelected == null) return;
        Destroy(itemSelected.gameObject);
        itemSelected = null;
    }
    
    public void UseDraggable()
    {
        onUseDraggable();
        if (itemSelected == null) return;
        Destroy(itemSelected.gameObject);
        itemSelected = null;
    }

    public virtual void onUseDraggable()
    {
        
    }
    
    public bool isFree(Draggable draggable)
    {
        return itemSelected == null || itemSelected == draggable;
    }

    public virtual void onBeginDrag(Draggable draggable)
    {
        
    }

    public virtual void onEndDrag(Draggable draggable)
    {
        
    }

}
