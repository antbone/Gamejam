using System.Collections.Generic;

namespace GameConfig
{
    public class TerminalConfigItem
    {
        /// <summary>
        /// 唯一主键
        /// </summary>
        public int UniqueKey { private set; get; }
        /// <summary>
        /// 等级ID
        /// </summary>
        public int Id { private set; get; }
        /// <summary>
        /// 效率继承
        /// </summary>
        public float Extend { private set; get; }
        /// <summary>
        /// 原料id列表
        /// </summary>
        public IReadOnlyList<int> MatIds { private set; get; }
        /// <summary>
        /// 原料数量列表
        /// </summary>
        public IReadOnlyList<int> MatNums { private set; get; }

        public TerminalConfigItem(int uniqueKey, int id, float extend, IReadOnlyList<int> matIds, IReadOnlyList<int> matNums)
        {
            UniqueKey = uniqueKey;
            Id = id;
            Extend = extend;
            MatIds = matIds;
            MatNums = matNums;
        }
    }
}