using UnityEngine;

public class IndexTrigger : MVTriggerBase<int>
{
    public bool loop = true;
    public int min = 0;
    public int max = 2;
    protected override void OnTrigger()
    {
        if (loop)
            data = (data + 1) % max;
        else
            data = Mathf.Clamp(data, min, max);
    }
}