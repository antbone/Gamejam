using System.Collections.Generic;

namespace GameConfig
{
    public class BackpackConfigItem
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
        /// 容量
        /// </summary>
        public int Compacity { private set; get; }
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

        public BackpackConfigItem(int uniqueKey, int id, int itemId, int compacity, IReadOnlyList<int> craftMatIds, IReadOnlyList<int> craftMatNums, float craftTime)
        {
            UniqueKey = uniqueKey;
            Id = id;
            ItemId = itemId;
            Compacity = compacity;
            CraftMatIds = craftMatIds;
            CraftMatNums = craftMatNums;
            CraftTime = craftTime;
        }
    }
}