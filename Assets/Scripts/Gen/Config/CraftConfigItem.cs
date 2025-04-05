using System.Collections.Generic;

namespace GameConfig
{
    public class CraftConfigItem
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
        /// 原料id列表
        /// </summary>
        public IReadOnlyList<int> MatIds { private set; get; }
        /// <summary>
        /// 原料数量列表
        /// </summary>
        public IReadOnlyList<int> MatNums { private set; get; }
        /// <summary>
        /// 目标id
        /// </summary>
        public int TargetId { private set; get; }
        /// <summary>
        /// 制作时长
        /// </summary>
        public float Time { private set; get; }
        /// <summary>
        /// 生产上限
        /// </summary>
        public int CraftLimit { private set; get; }
        /// <summary>
        /// 存储上限
        /// </summary>
        public int StoreLimit { private set; get; }
        /// <summary>
        /// 是否是高级生产
        /// </summary>
        public bool IsAdvance { private set; get; }

        public CraftConfigItem(int uniqueKey, int id, IReadOnlyList<int> matIds, IReadOnlyList<int> matNums, int targetId, float time, int craftLimit, int storeLimit, bool isAdvance)
        {
            UniqueKey = uniqueKey;
            Id = id;
            MatIds = matIds;
            MatNums = matNums;
            TargetId = targetId;
            Time = time;
            CraftLimit = craftLimit;
            StoreLimit = storeLimit;
            IsAdvance = isAdvance;
        }
    }
}