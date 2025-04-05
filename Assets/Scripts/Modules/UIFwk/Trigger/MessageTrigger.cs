using UnityEngine;

public class MessageTrigger : MVTriggerBase<string>
{
    protected override void OnTrigger()
    {
        Debug.Log("MessageTrigger OnTrigger");
        ui.Send(data);
    }
}