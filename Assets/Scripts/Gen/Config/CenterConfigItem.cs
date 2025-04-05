using System.Collections.Generic;

namespace GameConfig
{
    public class CenterConfigItem
    {
        /// <summary>
        /// 唯一主键
        /// </summary>
        public int UniqueKey { private set; get; }
        /// <summary>
        /// 等级
        /// </summary>
        public int Lv { private set; get; }
        /// <summary>
        /// 日曜区光强
        /// </summary>
        public float Light1 { private set; get; }
        /// <summary>
        /// 日曦区光强
        /// </summary>
        public float Light2 { private set; get; }
        /// <summary>
        /// 日晖区光强
        /// </summary>
        public float Light3 { private set; get; }
        /// <summary>
        /// 每3秒产出日芒
        /// </summary>
        public int Generate { private set; get; }
        /// <summary>
        /// 升级所需日芒
        /// </summary>
        public int UpgradeCost { private set; get; }
        /// <summary>
        /// 最大生命值
        /// </summary>
        public int MaxLife { private set; get; }

        public CenterConfigItem(int uniqueKey, int lv, float light1, float light2, float light3, int generate, int upgradeCost, int maxLife)
        {
            UniqueKey = uniqueKey;
            Lv = lv;
            Light1 = light1;
            Light2 = light2;
            Light3 = light3;
            Generate = generate;
            UpgradeCost = upgradeCost;
            MaxLife = maxLife;
        }
    }
}