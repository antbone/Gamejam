using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private Transform root;
    public void Init(Transform root)
    {
        this.root = root;

    }
    public void OnStart()
    {
        // VController.Ins.ShowST("HUD", VParams.Relative(ERelativeType.Full));
    }
    public void OnUpdate()
    {
        VController.Ins.UpdateUI();
    }
}
