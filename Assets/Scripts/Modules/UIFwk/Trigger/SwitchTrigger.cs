public class SwitchTrigger : MVTriggerBase<bool>
{
    protected override void OnTrigger()
    {
        base.OnTrigger();
        this.data = !this.data;
    }
}