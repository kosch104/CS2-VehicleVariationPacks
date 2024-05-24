using System.IO;
using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;
using Game.SceneFlow;
using Colossal.IO.AssetDatabase;
using Colossal.PSI.Environment;

namespace CarVariationChanger
{
    public class Mod : IMod
    {
        public static ILog log = LogManager.GetLogger($"{nameof(CarVariationChanger)}.{nameof(Mod)}")
            .SetShowsErrorsInUI(false);

        private Setting m_Setting;
        public static string path;

        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                path = asset.path;

            MigrateSettings();
            updateSystem.UpdateAt<CarVariationChangerSystem>(SystemUpdatePhase.MainLoop);

            m_Setting = new Setting(this);
            m_Setting.RegisterInOptionsUI();
            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(m_Setting));
            Setting.Instance = m_Setting;

            AssetDatabase.global.LoadSettings(nameof(CarVariationChanger), m_Setting, new Setting(this));
        }

        private void MigrateSettings()
        {
            var oldLocation = Path.Combine(EnvPath.kUserDataPath, nameof(CarVariationChanger) + ".coc");
            if (File.Exists(oldLocation))
            {
                var newLocation = Path.Combine(EnvPath.kUserDataPath, "ModsSettings", nameof(CarVariationChanger), nameof(CarVariationChanger) + ".coc");
                if (!File.Exists(newLocation))
                {
                    if (!Directory.Exists(Path.GetDirectoryName(newLocation)))
                        Directory.CreateDirectory(Path.GetDirectoryName(newLocation));
                    File.Move(oldLocation, newLocation);
                }
                else
                {
                    File.Delete(oldLocation);
                }
            }
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
            if (m_Setting != null)
            {
                m_Setting.UnregisterInOptionsUI();
                m_Setting = null;
            }
        }
    }
}