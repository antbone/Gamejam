using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace GameConfig
{
    public static class Config
    {
        public static BaseConfig<int, CenterConfigItem> CenterConfig { private set; get; }
        public static BaseConfig<int, StoneConfigItem> StoneConfig { private set; get; }
        public static BaseConfig<int, ResConfigItem> ResConfig { private set; get; }
        public static BaseConfig<int, PickaxeConfigItem> PickaxeConfig { private set; get; }
        public static BaseConfig<int, BackpackConfigItem> BackpackConfig { private set; get; }
        public static BaseConfig<int, CraftConfigItem> CraftConfig { private set; get; }
        public static BaseConfig<int, TerminalConfigItem> TerminalConfig { private set; get; }
        public static BaseConfig<int, TorchConfigItem> TorchConfig { private set; get; }
        public static BaseConfig<int, ItemConfigItem> ItemConfig { private set; get; }
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

            // CenterConfig
            section = sections[0];
            lines = Regex.Split(section, "\r\n");
            Dictionary<int, CenterConfigItem> centerConfigData = new Dictionary<int, CenterConfigItem>();
            for (int n = 0; n < lines.Length - 1; n += 8)
            {
                var item = new CenterConfigItem(ConfigUtility.ParseInt(lines[n]), ConfigUtility.ParseInt(lines[n + 1]), ConfigUtility.ParseFloat(lines[n + 2]), ConfigUtility.ParseFloat(lines[n + 3]), ConfigUtility.ParseFloat(lines[n + 4]), ConfigUtility.ParseInt(lines[n + 5]), ConfigUtility.ParseInt(lines[n + 6]), ConfigUtility.ParseInt(lines[n + 7]));
                centerConfigData[item.UniqueKey] = item;
            }
            CenterConfig = new BaseConfig<int, CenterConfigItem>("CenterConfig", centerConfigData);

            // StoneConfig
            section = sections[1];
            lines = Regex.Split(section, "\r\n");
            Dictionary<int, StoneConfigItem> stoneConfigData = new Dictionary<int, StoneConfigItem>();
            for (int n = 0; n < lines.Length - 1; n += 8)
            {
                var item = new StoneConfigItem(ConfigUtility.ParseInt(lines[n]), ConfigUtility.ParseInt(lines[n + 1]), lines[n + 2], ConfigUtility.ParseFloat(lines[n + 3]), ConfigUtility.ParseInt(lines[n + 4]), ConfigUtility.ParseFloat(lines[n + 5]), lines[n + 6], lines[n + 7]);
                stoneConfigData[item.UniqueKey] = item;
            }
            StoneConfig = new BaseConfig<int, StoneConfigItem>("StoneConfig", stoneConfigData);

            // ResConfig
            section = sections[2];
            lines = Regex.Split(section, "\r\n");
            Dictionary<int, ResConfigItem> resConfigData = new Dictionary<int, ResConfigItem>();
            for (int n = 0; n < lines.Length - 1; n += 8)
            {
                var item = new ResConfigItem(ConfigUtility.ParseInt(lines[n]), ConfigUtility.ParseInt(lines[n + 1]), lines[n + 2], lines[n + 3], lines[n + 4], ConfigUtility.ParseInt(lines[n + 5]), ConfigUtility.ParseBool(lines[n + 6]), ConfigUtility.ParseBool(lines[n + 7]));
                resConfigData[item.UniqueKey] = item;
            }
            ResConfig = new BaseConfig<int, ResConfigItem>("ResConfig", resConfigData);

            // PickaxeConfig
            section = sections[3];
            lines = Regex.Split(section, "\r\n");
            Dictionary<int, PickaxeConfigItem> pickaxeConfigData = new Dictionary<int, PickaxeConfigItem>();
            for (int n = 0; n < lines.Length - 1; n += 9)
            {
                var item = new PickaxeConfigItem(ConfigUtility.ParseInt(lines[n]), ConfigUtility.ParseInt(lines[n + 1]), ConfigUtility.ParseInt(lines[n + 2]), ConfigUtility.ParseInt(lines[n + 3]), ConfigUtility.ParseFloat(lines[n + 4]), ConfigUtility.ParseInt(lines[n + 5]), ConfigUtility.ParseIntList(lines[n + 6]), ConfigUtility.ParseIntList(lines[n + 7]), ConfigUtility.ParseFloat(lines[n + 8]));
                pickaxeConfigData[item.UniqueKey] = item;
            }
            PickaxeConfig = new BaseConfig<int, PickaxeConfigItem>("PickaxeConfig", pickaxeConfigData);

            // BackpackConfig
            section = sections[4];
            lines = Regex.Split(section, "\r\n");
            Dictionary<int, BackpackConfigItem> backpackConfigData = new Dictionary<int, BackpackConfigItem>();
            for (int n = 0; n < lines.Length - 1; n += 7)
            {
                var item = new BackpackConfigItem(ConfigUtility.ParseInt(lines[n]), ConfigUtility.ParseInt(lines[n + 1]), ConfigUtility.ParseInt(lines[n + 2]), ConfigUtility.ParseInt(lines[n + 3]), ConfigUtility.ParseIntList(lines[n + 4]), ConfigUtility.ParseIntList(lines[n + 5]), ConfigUtility.ParseFloat(lines[n + 6]));
                backpackConfigData[item.UniqueKey] = item;
            }
            BackpackConfig = new BaseConfig<int, BackpackConfigItem>("BackpackConfig", backpackConfigData);

            // CraftConfig
            section = sections[5];
            lines = Regex.Split(section, "\r\n");
            Dictionary<int, CraftConfigItem> craftConfigData = new Dictionary<int, CraftConfigItem>();
            for (int n = 0; n < lines.Length - 1; n += 9)
            {
                var item = new CraftConfigItem(ConfigUtility.ParseInt(lines[n]), ConfigUtility.ParseInt(lines[n + 1]), ConfigUtility.ParseIntList(lines[n + 2]), ConfigUtility.ParseIntList(lines[n + 3]), ConfigUtility.ParseInt(lines[n + 4]), ConfigUtility.ParseFloat(lines[n + 5]), ConfigUtility.ParseInt(lines[n + 6]), ConfigUtility.ParseInt(lines[n + 7]), ConfigUtility.ParseBool(lines[n + 8]));
                craftConfigData[item.UniqueKey] = item;
            }
            CraftConfig = new BaseConfig<int, CraftConfigItem>("CraftConfig", craftConfigData);

            // TerminalConfig
            section = sections[6];
            lines = Regex.Split(section, "\r\n");
            Dictionary<int, TerminalConfigItem> terminalConfigData = new Dictionary<int, TerminalConfigItem>();
            for (int n = 0; n < lines.Length - 1; n += 5)
            {
                var item = new TerminalConfigItem(ConfigUtility.ParseInt(lines[n]), ConfigUtility.ParseInt(lines[n + 1]), ConfigUtility.ParseFloat(lines[n + 2]), ConfigUtility.ParseIntList(lines[n + 3]), ConfigUtility.ParseIntList(lines[n + 4]));
                terminalConfigData[item.UniqueKey] = item;
            }
            TerminalConfig = new BaseConfig<int, TerminalConfigItem>("TerminalConfig", terminalConfigData);

            // TorchConfig
            section = sections[7];
            lines = Regex.Split(section, "\r\n");
            Dictionary<int, TorchConfigItem> torchConfigData = new Dictionary<int, TorchConfigItem>();
            for (int n = 0; n < lines.Length - 1; n += 5)
            {
                var item = new TorchConfigItem(ConfigUtility.ParseInt(lines[n]), ConfigUtility.ParseInt(lines[n + 1]), ConfigUtility.ParseInt(lines[n + 2]), ConfigUtility.ParseFloat(lines[n + 3]), ConfigUtility.ParseFloat(lines[n + 4]));
                torchConfigData[item.UniqueKey] = item;
            }
            TorchConfig = new BaseConfig<int, TorchConfigItem>("TorchConfig", torchConfigData);

            // ItemConfig
            section = sections[8];
            lines = Regex.Split(section, "\r\n");
            Dictionary<int, ItemConfigItem> itemConfigData = new Dictionary<int, ItemConfigItem>();
            for (int n = 0; n < lines.Length - 1; n += 8)
            {
                var item = new ItemConfigItem(ConfigUtility.ParseInt(lines[n]), ConfigUtility.ParseInt(lines[n + 1]), lines[n + 2], ConfigUtility.ParseInt(lines[n + 3]), lines[n + 4], lines[n + 5], (ItemType) ConfigUtility.ParseInt(lines[n + 6]), ConfigUtility.ParseBool(lines[n + 7]));
                itemConfigData[item.UniqueKey] = item;
            }
            ItemConfig = new BaseConfig<int, ItemConfigItem>("ItemConfig", itemConfigData);
        }
    }
}