using UnityEngine;

public enum ELayer
{
    Default,
    Bottom,
    Panel,
    Pop,
    Top,
    Tips,
    Effect,
    Alert

}
public class VLayer : MonoBehaviour
{
    public bool isHideWithoutChildren = true;
    public ELayer layer = ELayer.Default;
    void Awake()
    {
        VController.Ins.layers.Fill(layer, this);
        RefreshActive();
    }
    public virtual void OnAddPanel()
    {
        RefreshActive();
    }
    public virtual void OnRemovePanel()
    {
        RefreshActive();
    }
    public void RefreshActive()
    {
        gameObject.SetActive(!isHideWithoutChildren || transform.childCount > 0);
    }
}