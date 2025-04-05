using System.Collections.Generic;

namespace GameConfig
{
    public class TorchConfigItem
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
        /// 光强
        /// </summary>
        public float Intensity { private set; get; }
        /// <summary>
        /// 湿度抗性
        /// </summary>
        public float Resistance { private set; get; }

        public TorchConfigItem(int uniqueKey, int id, int itemId, float intensity, float resistance)
        {
            UniqueKey = uniqueKey;
            Id = id;
            ItemId = itemId;
            Intensity = intensity;
            Resistance = resistance;
        }
    }
}