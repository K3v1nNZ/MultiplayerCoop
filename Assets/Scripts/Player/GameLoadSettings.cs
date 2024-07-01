using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Game.Player
{
    public class GameSettings
    {
        public int WindowMode;
        public bool VSync;
        public int MsaaSampleCount;
    }
    
    public static class GameLoadSettings
    {
        public static UniversalRenderPipelineAsset UrpAsset;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void LoadSettings()
        {
            UrpAsset = GraphicsSettings.defaultRenderPipeline as UniversalRenderPipelineAsset;
            
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
            UrpAsset.msaaSampleCount = settings.MsaaSampleCount switch
            {
                0 => 1,
                1 => 2,
                2 => 4,
                3 => 8,
                _ => UrpAsset.msaaSampleCount
            };
        }

        private static void GenerateSettings()
        {
            File.WriteAllText(Application.persistentDataPath + "/settings.json", JsonUtility.ToJson(new GameSettings
            {
                WindowMode = 1,
                VSync = false,
                MsaaSampleCount = 2
            }));
        }
    }
}
