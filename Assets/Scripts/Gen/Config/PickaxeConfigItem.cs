using System.Collections.Generic;

namespace GameConfig
{
    public class PickaxeConfigItem
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
        /// 物品id
        /// </summary>
        public int ItemId { private set; get; }
        /// <summary>
        /// 镐力
        /// </summary>
        public int Damage { private set; get; }
        /// <summary>
        /// 镐速
        /// </summary>
        public float Speed { private set; get; }
        /// <summary>
        /// 挖掘等级
        /// </summary>
        public int Level { private set; get; }
        /// <summary>
        /// 制作材料id
        /// </summary>
        public IReadOnlyList<int> CraftMatIds { private set; get; }
        /// <summary>
        /// 制作材料数量
        /// </summary>
        public IReadOnlyList<int> CraftMatNums { private set; get; }
        /// <summary>
        /// 制作时长
        /// </summary>
        public float CraftTime { private set; get; }

        public PickaxeConfigItem(int uniqueKey, int id, int itemId, int damage, float speed, int level, IReadOnlyList<int> craftMatIds, IReadOnlyList<int> craftMatNums, float craftTime)
        {
            UniqueKey = uniqueKey;
            Id = id;
            ItemId = itemId;
            Damage = damage;
            Speed = speed;
            Level = level;
            CraftMatIds = craftMatIds;
            CraftMatNums = craftMatNums;
            CraftTime = craftTime;
        }
    }
}