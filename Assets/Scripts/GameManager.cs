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
    public Table table;

    public void OnStart()
    {
        // 启动游戏流程
        GameFlowManager.Instance.StartGame();
    }

    public void OnUpdate()
    {
        // 更新游戏流程
        GameFlowManager.Instance.Update();
    }
}
