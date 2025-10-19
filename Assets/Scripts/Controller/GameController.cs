using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Controller
{
    public class GameController : MonoBehaviour
    {
        private Level _level;

        [ShowInInspector, ReadOnly] public List<OrderModelData> OrderData { get; private set; }
        [ShowInInspector, ReadOnly] public Dictionary<EColorType, int> OrderCount { get; private set; }

        private async void Start()
        {
            Application.targetFrameRate = 60;
            _level = Level.Instance;
            await UniTask.Yield();
            InitLevel();
            Conveyor.Instance.Init();
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
            ;
            StartGame();
        }

        void InitLevel()
        {
            _level.Init();
            OrderData = new List<OrderModelData>();
            OrderCount = new Dictionary<EColorType, int>();
            
            LineSpawner.Instance.SpawnLine(_level.LineData, _level.Distributions);
        }

        void StartGame()
        {
            _level.StartGame();
        }
    }
}