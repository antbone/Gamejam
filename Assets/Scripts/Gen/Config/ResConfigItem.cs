using System.Collections.Generic;

namespace GameConfig
{
    public class ResConfigItem
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
        /// 图集名称
        /// </summary>
        public string Altas { private set; get; }
        /// <summary>
        /// 图片名称
        /// </summary>
        public string Sprite { private set; get; }
        /// <summary>
        /// 对应物品id
        /// （-1为没有）
        /// </summary>
        public int ItemId { private set; get; }
        /// <summary>
        /// 是否共享储量
        /// </summary>
        public bool IsShare { private set; get; }
        /// <summary>
        /// 是否是道具
        /// </summary>
        public bool IsProp { private set; get; }

        public ResConfigItem(int uniqueKey, int id, string name, string altas, string sprite, int itemId, bool isShare, bool isProp)
        {
            UniqueKey = uniqueKey;
            Id = id;
            Name = name;
            Altas = altas;
            Sprite = sprite;
            ItemId = itemId;
            IsShare = isShare;
            IsProp = isProp;
        }
    }
}