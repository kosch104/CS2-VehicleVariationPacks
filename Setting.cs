using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI;
using Game.UI.Widgets;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace CarVariationChanger
{
    [FileLocation(nameof(CarVariationChanger))]
    public class Setting : ModSetting
    {
        public static Setting Instance;
        public Setting(IMod mod) : base(mod)
        {

        }

        public bool OpenPacksFolder
        {
            set
            {
                var file = Mod.path;
                var parentDir = Directory.GetParent(file).FullName;
                Process.Start(Path.Combine(parentDir, "packs"));
            }
        }

        public bool ReloadPacks
        {
            set
            {
                this.UnregisterInOptionsUI();
                this.RegisterInOptionsUI();
            }
        }

        private string _packDropdown = "Realistic Global";

        [SettingsUIDropdown(typeof(Setting), nameof(GetNameDropdownItems))]
        public string PackDropdown
        {
            get
            {
                return _packDropdown;
            }
            set
            {
                _packDropdown = value;
                if (value != null)
                {
                    CarVariationChangerSystem.Instance.LoadVariationPack(value);
                }
            }
        }

        public DropdownItem<string>[] GetNameDropdownItems()
        {
            var names =  VariationPack.GetVariationPackNames();

            List<DropdownItem<string>> items = new List<DropdownItem<string>>();
            foreach(string s in names)
            {
                items.Add(new DropdownItem<string>()
                {
                    value = s,
                    displayName = s,
                });
            }

            return items.ToArray();
        }

        public override void SetDefaults()
        {
            throw new System.NotImplementedException();
        }
    }

    public class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "Car Variation Changer" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenPacksFolder)), "Open Packs Folder" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenPacksFolder)),
                    $"Opens the folder where the packs are stored, allowing you to add your own"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ReloadPacks)), "Reload available Packs" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ReloadPacks)),
                    $"Reloads the available packs installed in your packs folder"
                },


                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PackDropdown)), "Active Pack" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.PackDropdown)),
                    $"Choose which Variation Pack to use. If you manually installed a pack and it is not displayed here, click on 'Reload available Packs' to refresh the list"
                },
            };
        }

        public void Unload()
        {
        }
    }
}
