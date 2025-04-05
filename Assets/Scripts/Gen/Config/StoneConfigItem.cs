using System.Collections.Generic;

namespace GameConfig
{
    public class StoneConfigItem
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
        /// 挖掘血量
        /// </summary>
        public float Hp { private set; get; }
        /// <summary>
        /// 掉落物id
        /// </summary>
        public int DropId { private set; get; }
        /// <summary>
        /// 掉落物数量
        /// </summary>
        public float DropNum { private set; get; }
        /// <summary>
        /// 所在图集名称
        /// </summary>
        public string StoneAltas { private set; get; }
        /// <summary>
        /// 图集中的精灵名称
        /// </summary>
        public string StoneSprite { private set; get; }

        public StoneConfigItem(int uniqueKey, int id, string name, float hp, int dropId, float dropNum, string stoneAltas, string stoneSprite)
        {
            UniqueKey = uniqueKey;
            Id = id;
            Name = name;
            Hp = hp;
            DropId = dropId;
            DropNum = dropNum;
            StoneAltas = stoneAltas;
            StoneSprite = stoneSprite;
        }
    }
}