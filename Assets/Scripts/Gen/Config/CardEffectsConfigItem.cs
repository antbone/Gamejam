using System.Collections.Generic;

namespace GameConfig
{
    public class CardEffectsConfigItem
    {
        /// <summary>
        /// 唯一主键
        /// </summary>
        public int UniqueKey { private set; get; }
        public int Id { private set; get; }
        public string Name { private set; get; }
        /// <summary>
        /// 固定数值
        /// </summary>
        public string Effect { private set; get; }

        public CardEffectsConfigItem(int uniqueKey, int id, string name, string effect)
        {
            UniqueKey = uniqueKey;
            Id = id;
            Name = name;
            Effect = effect;
        }
    }
}