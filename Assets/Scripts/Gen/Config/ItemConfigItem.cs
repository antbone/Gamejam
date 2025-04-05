using System.Collections.Generic;

namespace GameConfig
{
    public class ItemConfigItem
    {
        /// <summary>
        /// 唯一主键
        /// </summary>
        public int UniqueKey { private set; get; }
        /// <summary>
        /// ID
        /// </summary>
        public int Id { private set; get; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { private set; get; }
        /// <summary>
        /// 负重
        /// </summary>
        public int Weight { private set; get; }
        /// <summary>
        /// 所在图集名称
        /// </summary>
        public string Altas { private set; get; }
        /// <summary>
        /// 图集中的精灵名称
        /// </summary>
        public string Sprite { private set; get; }
        /// <summary>
        /// 类型
        /// </summary>
        public ItemType Type { private set; get; }
        /// <summary>
        /// 是否自动回收
        /// </summary>
        public bool AutoRecycle { private set; get; }

        public ItemConfigItem(int uniqueKey, int id, string name, int weight, string altas, string sprite, ItemType type, bool autoRecycle)
        {
            UniqueKey = uniqueKey;
            Id = id;
            Name = name;
            Weight = weight;
            Altas = altas;
            Sprite = sprite;
            Type = type;
            AutoRecycle = autoRecycle;
        }
    }
}