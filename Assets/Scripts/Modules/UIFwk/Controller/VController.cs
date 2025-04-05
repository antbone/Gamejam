using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public enum ERelativeType
{
    Full,
    Center,
}
public class VParams
{
    public ERelativeType? relativeType = null;
    public ELayer? layer = null;
    public object data = null;
    public static Group Relative(ERelativeType type) => new Group() { paramInfo = new VParams() { relativeType = type } };
    public static Group Layer(ELayer layer) => new Group() { paramInfo = new VParams() { layer = layer } };
    public static Group Data(object data) => new Group() { paramInfo = new VParams() { data = data } };
    public class Group
    {
        public VParams paramInfo;
        public Group()
        {
            paramInfo = new VParams();
        }
        public Group Relative(ERelativeType type)
        {
            paramInfo.relativeType = type;
            return this;
        }
        public Group Layer(ELayer layer)
        {
            paramInfo.layer = layer;
            return this;
        }
        public Group Data(object data)
        {
            paramInfo.data = data;
            return this;
        }
    }
}
public class VController : Singleton<VController>
{
    const string UIPath = "Prefabs/UI/";
    public Dictionary<ELayer, VLayer> layers = new();
    private Dictionary<string, VPanel> uiDic = new();
    private Dictionary<string, Queue<VPanel>> toastDic = new();
    private Dictionary<int, object> panelData = new();
    private static Allocater panelIdAllocater = new();
    public VPanel GetSTUI(string key)
    {
        if (uiDic.ContainsKey(key))
            return uiDic[key];
        return null;
    }
    public void SendMessage(string key, object data = null, bool forToast = false)
    {
        if (forToast)
        {
            if (toastDic.ContainsKey(key))
                foreach (VPanel panel in toastDic[key])
                    panel.SendEvent(key, data);
        }
        else if (uiDic.ContainsKey(key))
            uiDic[key].SendEvent(key, data);
    }
    public List<VPanel> PanelList => uiDic.Values.ToList();
    public List<VPanel> GetToastUIs(string key)
    {
        if (toastDic.ContainsKey(key))
            return toastDic[key].ToList();
        return new List<VPanel>();
    }
    public void SetUIIndex(int id, int index)
    {
        GetUI(id)?.Root.SetSiblingIndex(index);
    }
    public object GetPanelData(int id)
    {
        return panelData.ContainsKey(id) ? panelData[id] : null;
    }
    public void UpdateUI()
    {
        foreach (VPanel panel in uiDic.Values)
            panel.Update();
    }
    public VPanel GetUI(int id)
    {
        VPanel stui = GetSTUI(id);
        if (stui != null)
            return stui;
        VPanel popui = GetPopUI(id);
        if (popui != null)
            return popui;
        return null;
    }
    public T NewUI<T>(string key, bool isFromPool, Transform parent) where T : VPanel
    {
        GameObject prefab = Resources.Load<GameObject>(UIPath + key);
        GameObject go;
        if (isFromPool)
            go = prefab.OPGet();
        else
            go = GameObject.Instantiate(prefab);
        go.transform.SetParent(parent);
        int id = panelIdAllocater.GetID();
        go.name = key + "_" + id;
        T panel = Activator.CreateInstance<T>();
        panel.Init(go.transform, key, id);
        return panel;
    }
    public int ShowST<T>(string key, VParams.Group paramsGroup = null) where T : VPanel
    {
        if (paramsGroup == null)
            paramsGroup = new VParams.Group();
        VParams paramsInfo = paramsGroup.paramInfo;
        ELayer layer = paramsInfo.layer == null ? ELayer.Panel : paramsInfo.layer.Value;
        ERelativeType relative = paramsInfo.relativeType == null ? ERelativeType.Full : paramsInfo.relativeType.Value;
        VLayer vlayer = layers.ContainsKey(layer) ? layers[layer] : layers[ELayer.Default];
        VPanel com;
        if (uiDic.ContainsKey(key))
        {
            com = uiDic[key];
            if (com.Root.parent != vlayer.transform)
                com.Root.SetParent(vlayer.transform);
        }
        else
        {
            com = NewUI<T>(key, false, vlayer.transform);
            com.Load();
            uiDic.Add(key, com);
            panelData.Add(com.ID, paramsInfo.data);
            com.vlayer.OnAddPanel();
        }
        com.Show();
        com.SetData(paramsInfo.data);
        com.Root.SetSiblingIndex(-1);
        RectTransform rt = com.Root.GetComponent<RectTransform>();
        switch (relative)
        {
            case ERelativeType.Full:
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = Vector2.zero;
                break;
            case ERelativeType.Center:
                rt.anchorMin = rt.anchorMax = Vector2.one * 0.5f;
                rt.anchoredPosition = Vector2.zero;
                break;
        }
        return com.ID;
    }
    public VPanel GetSTUI(int id)
    {
        return PanelList.Find(e => e.ID == id);
    }
    public VPanel GetPopUI(int id)
    {
        VPanel res = null;
        toastDic.Values.Any(e =>
        {
            VPanel ui = e.ToList().Find(f => f.ID == id);
            res = ui;
            return ui != null;
        });
        return res;
    }
    public int ShowST(string key, VParams.Group paramsGroup = null)
    {
        return ShowST<VPanel>(key, paramsGroup);
    }
    public void HideST(string key)
    {
        if (!uiDic.ContainsKey(key))
            return;
        uiDic[key].Hide();
    }
    public void HideST(int id)
    {
        VPanel panel = GetSTUI(id);
        if (panel != null)
            HideST(panel.Key);
    }
    public void DestoryST(string key)
    {
        if (!uiDic.ContainsKey(key))
            return;
        panelIdAllocater.PushID(uiDic[key].ID);
        panelData.Remove(uiDic[key].ID);
        uiDic[key].Destory();
        GameObject.Destroy(uiDic[key].Root);
        uiDic[key].vlayer.OnRemovePanel();
        uiDic.Remove(key);
    }
    public void DestoryST(int id)
    {
        VPanel panel = GetSTUI(id);
        if (panel != null)
            DestoryST(panel.Key);
    }
    public int ShowPop<T>(string key, VParams.Group paramsGroup = null) where T : VPanel
    {
        if (paramsGroup == null)
            paramsGroup = new VParams.Group();
        VParams paramsInfo = paramsGroup.paramInfo;
        ELayer layer = paramsInfo.layer == null ? ELayer.Pop : paramsInfo.layer.Value;
        VLayer vlayer = layers.ContainsKey(layer) ? layers[layer] : layers[ELayer.Default];
        T com = NewUI<T>(key, true, vlayer.transform);
        com.Show();
        com.SetData(paramsInfo.data);
        panelData.Add(com.ID, paramsInfo.data);
        if (!toastDic.ContainsKey(key))
            toastDic.Add(key, new Queue<VPanel>());
        toastDic[key].Enqueue(com);
        com.Root.SetSiblingIndex(-1);
        com.vlayer.OnAddPanel();
        RectTransform rt = com.Root.GetComponent<RectTransform>();
        ERelativeType relative = paramsInfo.relativeType == null ? ERelativeType.Center : paramsInfo.relativeType.Value;
        switch (relative)
        {
            case ERelativeType.Full:
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = Vector2.zero;
                break;
            case ERelativeType.Center:
                rt.anchorMin = rt.anchorMax = Vector2.one * 0.5f;
                rt.anchoredPosition = Vector2.zero;
                break;
        }
        return com.ID;
    }
    public int ShowPop(string key, VParams.Group paramsGroup = null)
    {
        return ShowPop<VPanel>(key, paramsGroup);
    }
    public void HidePop(string key)
    {
        if (!toastDic.ContainsKey(key))
            return;
        VPanel com = toastDic[key].Dequeue();
        com.Hide();
        panelIdAllocater.PushID(com.ID);
        panelData.Remove(com.ID);
        com.Destory();
        com.vlayer.OnRemovePanel();
        com.Root.gameObject.OPPush();
        if (toastDic[key].Count == 0)
            toastDic.Remove(key);
    }
}