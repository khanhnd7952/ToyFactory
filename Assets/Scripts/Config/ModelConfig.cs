using UnityEngine;

namespace Config
{
    public class ModelConfig : ScriptableObject
    {
        private static Sprite[] _sprites;
        [ RuntimeInitializeOnLoadMethod]
        static void Init()
        {
            _sprites = Resources.LoadAll<Sprite>("DemoModels");
        }
        
        public static Sprite GetRandomSprite()
        {
            return _sprites[Random.Range(0, _sprites.Length)];
        }
    }
}