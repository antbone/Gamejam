using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Graphic))]
public class MVColor : MView<Color>
{
    STComp<Graphic> graphic = new();
    public override void SetData(Color data)
    {
        graphic.Get(this).color = data;
    }
}