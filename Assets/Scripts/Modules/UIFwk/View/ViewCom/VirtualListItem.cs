
using System;
using UnityEngine;

public enum EListItemState
{
    Sleep = 0,
    Show = 1,
    Hide = 2
}
[RequireComponent(typeof(CanvasGroup))]
public abstract class VirtualListItem : MView<object>
{
    public abstract Type DataType { get; }
    STComp<CanvasGroup> canvasGroup = new();
    [HideInInspector]
    public EListItemState itemState = EListItemState.Sleep;
    [HideInInspector]
    public object data = null;
    [HideInInspector]
    public int idx = 0;
    public virtual void OnLoadItem()
    {

    }
    public virtual void OnDestoryItem()
    {

    }
    public virtual void OnShow()
    {

    }
    public virtual void OnHide()
    {

    }
    public void ShowIdle()
    {
        canvasGroup.Get(this).alpha = 255;

    }
    public void HideIdle()
    {
        canvasGroup.Get(this).alpha = 0;

    }
    public abstract void OnData();
    public override void SetData(object data)
    {
        this.data = data;
        this.OnData();
    }
}