using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Image))]
public class MVImage : MView<Sprite>
{
    STComp<Image> img = new();
    public override void SetData(Sprite data)
    {
        img.Get(this).sprite = data;
    }
}