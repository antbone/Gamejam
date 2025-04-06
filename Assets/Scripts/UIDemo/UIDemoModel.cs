
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIDemoModel : VMInventory<UIDemoModel>
{
    public VMList<int> backpacks = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 };//背包id数组
    public VMAdapter<List<int>, List<object>> backpackListAdapter = new VMAdapter<List<int>, List<object>>(
        (list) =>
        {
            List<UIDemoListItemData> data = new List<UIDemoListItemData>();
            foreach (var item in list)
            {
                data.Add(new UIDemoListItemData() { id = item });
            }
            return data.Cast<object>().ToList();
        }
    );
    public VM<int> backpack1 = 2;
    public VM<int> backpack2 = 5;
    public VM<int> backpack3 = 7;
    public VM<float> progressFloat = 1;
    public VM<string> dynamicText = "动态文本";
    public VM<int> dynamicId = 1;
    public VMAdapter<int, Sprite> dynamicIcon = new VMAdapter<int, Sprite>(
        (id) =>
        {
            return Resources.LoadAll<Sprite>("Images/backpack").ToList().Find(x => x.name == "backpack_" + (id - 1).ToString());
        }
    );
    public VMAdapter<float, string> dynamicProgressTxt = new VMAdapter<float, string>(
        (progress) =>
        {
            return progress.ToString("0.00") + "%";
        }
    );
}
