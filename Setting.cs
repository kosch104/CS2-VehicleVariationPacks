using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI;
using Game.UI.Widgets;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace CarColorChanger
{
    [FileLocation(nameof(CarColorChanger))]
    public class Setting : ModSetting
    {

        public Setting(IMod mod) : base(mod)
        {
        }

        public bool UpdateEntities
        {
            set { CarColorChangerSystem.Instance.UpdateEntityInstances(); }
        }

        private string _packDropdown = "Vanilla";

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
                    CarColorChangerSystem.Instance.LoadVariationPack(value);
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
                { m_Setting.GetSettingsLocaleID(), "Car Color Changer" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UpdateEntities)), "Reload active Pack" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.UpdateEntities)),
                    $"This button doesn't do anyting yet, but it will reload the active pack. Probably."
                },


                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PackDropdown)), "Active Pack" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.PackDropdown)),
                    $"Choose which Variation Pack to use"
                },
            };
        }

        public void Unload()
        {
        }
    }
}
