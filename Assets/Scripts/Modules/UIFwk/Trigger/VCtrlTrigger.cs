using UnityEngine;
public enum VCtrlTriggerType
{
    Show,
    Hide,
    Switch,
}
public class VCtrlTrigger : MVTriggerBase<object>
{
    const string k_None = "#None";
    public bool isSTPanel = true;
    public string panelKey = k_None;
    public VCtrlTriggerType ctrlType = VCtrlTriggerType.Switch;
    protected override void OnTrigger()
    {
        if (panelKey == k_None)
            return;
        if (isSTPanel)
        {
            VPanel ui = VController.Ins.GetSTUI(panelKey);
            bool isShow = ui == null ? false : ui.IsShow;
            if (ctrlType == VCtrlTriggerType.Show || ctrlType == VCtrlTriggerType.Switch && !isShow)
                VController.Ins.ShowST(panelKey, VParams.Data(data));
            else if (ctrlType == VCtrlTriggerType.Hide || ctrlType == VCtrlTriggerType.Switch && isShow)
                VController.Ins.HideST(panelKey);
        }
        else
        {
            if (ctrlType == VCtrlTriggerType.Show)
                VController.Ins.ShowST(panelKey, VParams.Data(data));
            else if (ctrlType == VCtrlTriggerType.Hide)
                VController.Ins.HideST(panelKey);
        }
    }
}