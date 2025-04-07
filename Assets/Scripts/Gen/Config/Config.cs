using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace GameConfig
{
    public static class Config
    {
        public static BaseConfig<int, CardsConfigItem> CardsConfig { private set; get; }
        public static BaseConfig<int, CardEffectsConfigItem> CardEffectsConfig { private set; get; }
        public static bool isInited = false;
        public static void Init(string configPath)
        {
            if (isInited)
                return;
            isInited = true;
            var textAsset = Resources.Load(configPath) as TextAsset;
            var configText = ConfigUtility.DecodeBase64(textAsset.text);
            Parse(configText);
        }
        
        public static void Parse(string configText)
        {
            var sections = configText.Split("#"[0]);

            string section;
            string[] lines;

            // CardsConfig
            section = sections[0];
            lines = Regex.Split(section, "\r\n");
            Dictionary<int, CardsConfigItem> cardsConfigData = new Dictionary<int, CardsConfigItem>();
            for (int n = 0; n < lines.Length - 1; n += 22)
            {
                var item = new CardsConfigItem(ConfigUtility.ParseInt(lines[n]), ConfigUtility.ParseInt(lines[n + 1]), lines[n + 2], lines[n + 3], ConfigUtility.ParseInt(lines[n + 4]), ConfigUtility.ParseInt(lines[n + 5]), lines[n + 6], lines[n + 7], lines[n + 8], lines[n + 9], ConfigUtility.ParseInt(lines[n + 10]), ConfigUtility.ParseIntList(lines[n + 11]), ConfigUtility.ParseIntList(lines[n + 12]), ConfigUtility.ParseIntList(lines[n + 13]), ConfigUtility.ParseInt(lines[n + 14]), ConfigUtility.ParseIntList(lines[n + 15]), ConfigUtility.ParseIntList(lines[n + 16]), ConfigUtility.ParseIntList(lines[n + 17]), ConfigUtility.ParseInt(lines[n + 18]), ConfigUtility.ParseIntList(lines[n + 19]), ConfigUtility.ParseIntList(lines[n + 20]), ConfigUtility.ParseIntList(lines[n + 21]));
                cardsConfigData[item.UniqueKey] = item;
            }
            CardsConfig = new BaseConfig<int, CardsConfigItem>("CardsConfig", cardsConfigData);

            // CardEffectsConfig
            section = sections[1];
            lines = Regex.Split(section, "\r\n");
            Dictionary<int, CardEffectsConfigItem> cardEffectsConfigData = new Dictionary<int, CardEffectsConfigItem>();
            for (int n = 0; n < lines.Length - 1; n += 9)
            {
                var item = new CardEffectsConfigItem(ConfigUtility.ParseInt(lines[n]), ConfigUtility.ParseInt(lines[n + 1]), lines[n + 2], lines[n + 3], ConfigUtility.ParseInt(lines[n + 4]), ConfigUtility.ParseInt(lines[n + 5]), ConfigUtility.ParseInt(lines[n + 6]), ConfigUtility.ParseInt(lines[n + 7]), ConfigUtility.ParseInt(lines[n + 8]));
                cardEffectsConfigData[item.UniqueKey] = item;
            }
            CardEffectsConfig = new BaseConfig<int, CardEffectsConfigItem>("CardEffectsConfig", cardEffectsConfigData);
        }
    }
}