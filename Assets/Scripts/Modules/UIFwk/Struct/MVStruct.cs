//model view struct


using System;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif
public interface IMVStruct
{
    public void OnEnter();
    public void OnExit();
    public void OnReceiveMsg(string msg);
    public void OnUpdate();
}
public class VPrivateStruct<T> : IMVStruct
{
    private readonly MVUI ui;
    private readonly MView<T> mvView;
    private readonly string eventKey = "";
    public VPrivateStruct(MVUI mvui, MView<T> mv, string key)
    {
        this.ui = mvui;
        this.mvView = mv;
        this.eventKey = key;
    }
    public void OnEnter()
    {
        ui.On(eventKey, OnData);
    }
    public void OnExit()
    {
        ui.Off(eventKey, OnData);
    }
    public void OnUpdate()
    {
        mvView.OnUpdate();
    }
    public void OnData(object data)
    {
        if (mvView.gameObject.activeInHierarchy && data.GetType() == typeof(T))
        {
            mvView.SetData((T)data);
        }
    }

    public void OnReceiveMsg(string msg)
    {
        mvView.OnReceiveMsg(msg);
    }
}
public class MVStruct<T> : IMVStruct
{
    private VMBase<T> vm = new();
    private readonly MView<T> com;
    public MVStruct(MView<T> com, VMBase<T> vm)
    {
        this.com = com;
        this.vm = vm;
    }
    public void OnData(T data)
    {
        if (com.gameObject.activeInHierarchy)
            com.SetData(data);
    }
    public void OnEnter()
    {
        vm?.On(OnData);
        vm?.InitRender(com.SetData);
    }
    public void OnUpdate()
    {
        com?.OnUpdate();
    }
    public void OnExit()
    {
        vm?.Off(OnData);
    }

    public void OnReceiveMsg(string msg)
    {
        com.OnReceiveMsg(msg);
    }
}
public class MVStructWithAdapter<VD, MD> : IMVStruct
{
    private VMBase<MD> vm;
    private readonly MView<VD> com;
    private VMAdapter<MD, VD> adapter;
    public MVStructWithAdapter(MView<VD> com, VMBase<MD> vm, VMAdapter<MD, VD> adapter)
    {
        this.com = com;
        this.vm = vm;
        this.adapter = adapter;
    }
    public void SetData(MD d)
    {
        if (com.gameObject.activeInHierarchy)
            com.SetData(adapter.Convert(d));
    }
    public void OnEnter()
    {
        vm?.On(SetData);
        vm?.InitRender(SetData);
        com?.OnBind();
    }
    public void OnExit()
    {
        vm?.Off(SetData);
        com?.OnUnbind();
    }
    public void OnUpdate()
    {
        com?.OnUpdate();
    }

    public void OnReceiveMsg(string msg)
    {
        com.OnReceiveMsg(msg);
    }
}