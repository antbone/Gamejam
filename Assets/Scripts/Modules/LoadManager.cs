using System.Collections.Generic;
using GameConfig;
using UnityEngine;
public class LoadManager : Singleton<LoadManager>
{
    const string SpritePath = "Sprites/";
    STMap<string, Dictionary<string, Sprite>> spritesMap =
        new(altasName => Resources.LoadAll<Sprite>(SpritePath + altasName).ToMap(e => e.name));
    public Sprite LoadSprite(string altasName, string spriteName)
    {
        if (altasName == "" || spriteName == "")
            return null;
        var dic = spritesMap.Get(altasName);
        return dic != null ? dic[spriteName] : null;
    }
    public Sprite LoadResSprite(int resId)
    {
        ResConfigItem cfg = Config.ResConfig.Get(resId);
        if (cfg == null)
            return null;
        return LoadSprite(cfg.Altas, cfg.Sprite);
    }
    public Sprite LoadItemSprite(int itemId)
    {
        ItemConfigItem cfg = Config.ItemConfig.Get(itemId);
        if (cfg == null)
            return null;
        return LoadSprite(cfg.Altas, cfg.Sprite);
    }
}