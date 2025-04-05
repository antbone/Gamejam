using UnityEngine;

public class HideTrigger : MVTriggerBase<object>
{
    const string k_None = "#None";
    public bool isST = true;
    public new string key = k_None;
    protected override void OnTrigger()
    {
        if (key == k_None)
            return;
        if (isST)
            VController.Ins.HideST(key);
        else
            VController.Ins.HidePop(key);
    }
}