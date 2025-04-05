using UnityEngine;
using UnityEngine.UI;

public class MVProgress : MView<float>
{
    STComp<Image> img = new();
    public override void SetData(float data)
    {
        img.Get(this).fillAmount = data;
    }
}