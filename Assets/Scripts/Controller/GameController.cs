using System;
using UnityEngine;

namespace Controller
{
    public class GameController : MonoBehaviour
    {
        private void Start()
        {
            Application.targetFrameRate = 60;
        }
    }
}