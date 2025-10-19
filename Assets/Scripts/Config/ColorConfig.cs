using System.Collections.Generic;
using UnityEngine;

namespace Config
{
    [CreateAssetMenu(fileName = "ColorConfig", menuName = "ToyFactory/ColorConfig", order = 0)]
    public class ColorConfig : ScriptableObject
    {
        public EColorType Color;
        public Mesh BrickMesh;
        public Color DemoColor;
        public Material BrickMaterial;


        private static ColorConfig[] Configs;
        private static Dictionary<EColorType, ColorConfig> _configMapping = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            Configs = Resources.LoadAll<ColorConfig>("ColorConfigs");
            _configMapping = new Dictionary<EColorType, ColorConfig>();
            foreach (ColorConfig config in Configs)
            {
                _configMapping.Add(config.Color, config);
            }
        }

        public static ColorConfig GetConfig(EColorType color) => _configMapping[color];
    }
}