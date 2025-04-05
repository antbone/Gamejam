using GameConfig;
using UnityEngine;
public class GlobalRef : SingletonComp<GlobalRef>
{
    public Sprite emptySprite;
    public static void CfgInit()
    {
        Config.Init("configs/Config");
    }
}

