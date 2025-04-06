using System.Collections.Generic;

namespace GameConfig
{
    public class CardsConfigItem
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
        /// 中文名
        /// </summary>
        public string ChineseName { private set; get; }
        /// <summary>
        /// 英文名
        /// </summary>
        public string EnglishName { private set; get; }
        /// <summary>
        /// 品质
        /// </summary>
        public int Quality { private set; get; }
        /// <summary>
        /// 基础弹珠数
        /// </summary>
        public int Reward { private set; get; }
        /// <summary>
        /// 图片素材
        /// </summary>
        public string Image { private set; get; }
        /// <summary>
        /// 中文描述
        /// </summary>
        public string ChineseDescription { private set; get; }
        /// <summary>
        /// 英文描述
        /// </summary>
        public string EnglishDescription { private set; get; }
        /// <summary>
        /// 效果描述
        /// </summary>
        public string EffectDescription { private set; get; }
        /// <summary>
        /// 效果0
        /// </summary>
        public int Effect0 { private set; get; }
        /// <summary>
        /// 参数0_0
        /// </summary>
        public IReadOnlyList<int> Arg00 { private set; get; }
        /// <summary>
        /// 参数0_1
        /// </summary>
        public IReadOnlyList<int> Arg01 { private set; get; }
        /// <summary>
        /// 参数0_2
        /// </summary>
        public IReadOnlyList<int> Arg02 { private set; get; }
        /// <summary>
        /// 效果1
        /// </summary>
        public int Effect1 { private set; get; }
        /// <summary>
        /// 参数1_0
        /// </summary>
        public IReadOnlyList<int> Arg10 { private set; get; }
        /// <summary>
        /// 参数1_1
        /// </summary>
        public IReadOnlyList<int> Arg11 { private set; get; }
        /// <summary>
        /// 参数1_2
        /// </summary>
        public IReadOnlyList<int> Arg12 { private set; get; }
        /// <summary>
        /// 效果2
        /// </summary>
        public int Effec2 { private set; get; }
        /// <summary>
        /// 参数2_0
        /// </summary>
        public IReadOnlyList<int> Arg20 { private set; get; }
        /// <summary>
        /// 参数2_1
        /// </summary>
        public IReadOnlyList<int> Arg21 { private set; get; }
        /// <summary>
        /// 参数2_2
        /// </summary>
        public IReadOnlyList<int> Arg22 { private set; get; }

        public CardsConfigItem(int uniqueKey, int id, string chineseName, string englishName, int quality, int reward, string image, string chineseDescription, string englishDescription, string effectDescription, int effect0, IReadOnlyList<int> arg00, IReadOnlyList<int> arg01, IReadOnlyList<int> arg02, int effect1, IReadOnlyList<int> arg10, IReadOnlyList<int> arg11, IReadOnlyList<int> arg12, int effec2, IReadOnlyList<int> arg20, IReadOnlyList<int> arg21, IReadOnlyList<int> arg22)
        {
            UniqueKey = uniqueKey;
            Id = id;
            ChineseName = chineseName;
            EnglishName = englishName;
            Quality = quality;
            Reward = reward;
            Image = image;
            ChineseDescription = chineseDescription;
            EnglishDescription = englishDescription;
            EffectDescription = effectDescription;
            Effect0 = effect0;
            Arg00 = arg00;
            Arg01 = arg01;
            Arg02 = arg02;
            Effect1 = effect1;
            Arg10 = arg10;
            Arg11 = arg11;
            Arg12 = arg12;
            Effec2 = effec2;
            Arg20 = arg20;
            Arg21 = arg21;
            Arg22 = arg22;
        }
    }
}