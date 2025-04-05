using System;
using System.Data.Common;
using UnityEngine;
public interface IMVTrigger
{
    string Key { get; }
    GameObject gameObject { get; }
    Type type { get; }
    void Trigger();
    void OnReset();
}
public class MVTriggerBase<T> : MonoBehaviour, IMVTrigger
{
    protected T data;
    public T defaultData;
    public string key;
    public string Key => key;
    STField<MonoBehaviour, MVUI> _ui = new(e => e.transform.RecurFind(f => f.parent, f => f.GetComponent<MVUI>()));
    protected MVUI ui => _ui.Get(this);
    public Type type => GetType();
    public void ResetData(T resetData)
    {
        data = resetData;
    }
    public void Trigger()
    {
        OnTrigger();
        ui.Send(key, data);
    }
    public void Trigger(T data)
    {
        this.data = data;
        OnTrigger();
        ui.Send(key, data);
    }
    public void OnReset()
    {
        this.data = this.defaultData;
        this.Trigger();
    }
    protected virtual void OnTrigger()
    {

    }
}