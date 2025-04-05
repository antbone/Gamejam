//model view
using UnityEngine;
public abstract class MView<T> : MonoBehaviour
{
    private STField<MonoBehaviour, MVUI> _ui = new(e => MUtils.RecurFind(e.transform, f => f.transform.parent, f => f.GetComponent<MVUI>()));
    public MVUI ui => _ui.Get(this);
    public abstract void SetData(T data);
    public virtual void OnBind() { }
    public virtual void OnUnbind() { }
    public virtual void OnReceiveMsg(string msg) { }
    public virtual void OnUpdate() { }
}