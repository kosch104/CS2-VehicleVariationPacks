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
    [SettingsUIGroupOrder(kButtonGroup, kToggleGroup, kSliderGroup, kDropdownGroup)]
    [SettingsUIShowGroupName(kButtonGroup, kToggleGroup, kSliderGroup, kDropdownGroup)]
    public class Setting : ModSetting
    {
        public const string kSection = "Main";

        public const string kButtonGroup = "Button";
        public const string kToggleGroup = "Toggle";
        public const string kSliderGroup = "Slider";
        public const string kDropdownGroup = "Dropdown";

        public Setting(IMod mod) : base(mod)
        {
        }

        [SettingsUISection(kSection, kButtonGroup)]
        public bool UpdateEntities
        {
            set { CarColorChangerSystem.UpdateEntitiesManually(); }
        }


        [SettingsUISection(kSection, kToggleGroup)]
        public bool Toggle { get; set; }

        [SettingsUISlider(min = 0, max = 100, step = 1, scalarMultiplier = 1, unit = Unit.kDataMegabytes)]
        [SettingsUISection(kSection, kSliderGroup)]
        public int IntSlider { get; set; }

        private string _packDropdown = "Vanilla";

        [SettingsUIDropdown(typeof(Setting), nameof(GetNameDropdownItems))]
        [SettingsUISection(kSection, kDropdownGroup)]
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
                    CarColorChangerSystem.LoadVariationPack(value);
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
                { m_Setting.GetOptionTabLocaleID(Setting.kSection), "Main" },

                { m_Setting.GetOptionGroupLocaleID(Setting.kButtonGroup), "Buttons" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kToggleGroup), "Toggle" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kSliderGroup), "Sliders" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kDropdownGroup), "Dropdowns" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.UpdateEntities)), "Button" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.UpdateEntities)),
                    $"Simple single button. It should be bool property with only setter or use [{nameof(SettingsUIButtonAttribute)}] to make button from bool property with setter and getter"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Toggle)), "Toggle" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.Toggle)),
                    $"Use bool property with setter and getter to get toggable option"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.IntSlider)), "Int slider" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.IntSlider)),
                    $"Use int property with getter and setter and [{nameof(SettingsUISliderAttribute)}] to get int slider"
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
