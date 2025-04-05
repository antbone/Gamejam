using System.Collections.Generic;
using UnityEngine;

public class MVActive : MView<int>
{
    public List<GameObject> go = new();
    public override void SetData(int state)
    {
        for (int i = 0; i < go.Count; i++)
        {
            if (go[i])
            {
                go[i].SetActive(i == state);
            }
        }
    }
}