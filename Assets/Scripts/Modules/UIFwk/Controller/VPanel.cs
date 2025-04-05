using UnityEngine;
public static class VPanelMessage
{
    public const string
    OnData = "OnData";
}
public class VPanel
{
    private bool isShow = false;
    public bool IsShow => isShow;
    private Transform root;
    public Transform Root => root;
    private string key;
    public string Key => key;
    private int id;
    public int ID => id;
    private STComp<MVUI> _ui = new();
    public MVUI UI => _ui.Get(Root);
    public VLayer vlayer => Root.RecurFind(e => e.parent, e => e.GetComponent<VLayer>());
    public void Init(Transform root, string key, int id)
    {
        this.root = root;
        this.key = key;
        this.id = id;
        if (UI != null)
            UI.BindPanel(id);
    }
    public void SendEvent(string evt, object data)
    {
        if (UI != null)
            UI.Send(evt, data);
    }
    public void SendUIMsg(string msg)
    {
        if (UI != null)
            UI.SendVMsg(msg);
    }
    public void Reset()
    {
        if (UI != null)
            UI.OnReset();
    }
    public virtual void SetData(object o)
    {
        this.SendUIMsg(VPanelMessage.OnData);
    }
    public void Load()
    {
    }
    public void Destory()
    {
        if (UI)
            UI.Clear();
    }
    public void Show()
    {
        Root.gameObject.SetActive(true);
        if (!isShow)
        {
            this.OnShow();
        }
        isShow = true;
    }
    public void Update()
    {
        if (UI)
            UI.OnUpdate();
    }
    public void Hide()
    {

        Root.gameObject.SetActive(false);
        if (isShow)
        {
            OnHide();
        }
        isShow = false;
    }
    public virtual void OnShow()
    {
        if (UI)
            UI.SetEnter();
    }
    public virtual void OnHide()
    {
        if (UI)
            UI.SetExit();
    }
}