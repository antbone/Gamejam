
using UnityEngine.UI;

public class MVText : MView<string>
{
    STComp<Text> text = new();
    public override void SetData(string data)
    {
        text.Get(this).text = data;
    }
}