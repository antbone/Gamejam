using System.Linq;
using UnityEngine;

public class UIDemoListItemData : ListItemData
{
    public int id;
    public string txt => "背包" + id.ToString();
    public Sprite icon => Resources.LoadAll<Sprite>("Images/backpack").ToList().Find(x => x.name == "backpack_" + (id - 1).ToString());
}