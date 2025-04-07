using System.Collections.Generic;

namespace GameConfig
{
    public class CardEffectsConfigItem
    {
        /// <summary>
        /// 唯一主键
        /// </summary>
        public int UniqueKey { private set; get; }
        /// <summary>
        /// 效果ID
        /// </summary>
        public int Id { private set; get; }
        /// <summary>
        /// 效果名称
        /// </summary>
        public string Name { private set; get; }
        /// <summary>
        /// 固定数值
        /// </summary>
        public string Effect { private set; get; }
        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority { private set; get; }
        /// <summary>
        /// 成长
        /// </summary>
        public int Grow { private set; get; }
        /// <summary>
        /// 额外收益
        /// </summary>
        public int Ext { private set; get; }
        /// <summary>
        /// 销毁
        /// </summary>
        public int Destory { private set; get; }
        /// <summary>
        /// 生成
        /// </summary>
        public int Gen { private set; get; }

        public CardEffectsConfigItem(int uniqueKey, int id, string name, string effect, int priority, int grow, int ext, int destory, int gen)
        {
            UniqueKey = uniqueKey;
            Id = id;
            Name = name;
            Effect = effect;
            Priority = priority;
            Grow = grow;
            Ext = ext;
            Destory = destory;
            Gen = gen;
        }
    }
}