using GameConfig;
using UnityEngine;

public class Launch : MonoBehaviour
{
    public GameObject root;

    private void Awake()
    {
        OP.Ins.root = transform;
        GlobalRef.CfgInit();
        GameManager.Ins.Init(root.transform);
    }
    void Start()
    {
        GameManager.Ins.OnStart();
    }
    private void Update()
    {
        TM.OnUpdate();
        CT.OnUpdate();
        GameManager.Ins.OnUpdate();
    }
}