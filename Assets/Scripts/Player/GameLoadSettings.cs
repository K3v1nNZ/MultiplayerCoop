using System.IO;
using UnityEngine;

namespace Game.Player
{
    public class GameSettings
    {
        public int WindowMode;
        public bool VSync;
    }
    
    public static class GameLoadSettings
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void LoadSettings()
        {
            if (!File.Exists(Application.persistentDataPath + "/settings.json"))
            {
                GenerateSettings();
            }
            GameSettings settings = JsonUtility.FromJson<GameSettings>(File.ReadAllText(Application.persistentDataPath + "/settings.json"));

            Screen.fullScreenMode = settings.WindowMode switch
            {
                0 => FullScreenMode.Windowed,
                1 => FullScreenMode.FullScreenWindow,
                2 => FullScreenMode.ExclusiveFullScreen,
                _ => Screen.fullScreenMode
            };
            QualitySettings.vSyncCount = settings.VSync ? 1 : 0;
        }

        private static void GenerateSettings()
        {
            File.WriteAllText(Application.persistentDataPath + "/settings.json", JsonUtility.ToJson(new GameSettings
            {
                WindowMode = 1,
                VSync = false
            }));
        }
    }
}
