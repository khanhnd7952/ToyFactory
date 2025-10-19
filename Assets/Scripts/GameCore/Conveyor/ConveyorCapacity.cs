using System;
using TMPro;
using UnityEngine;

namespace Kelsey.UGUI
{
    public class ConveyorCapacity : MonoBehaviour
    {
        public static ConveyorCapacity Instance { get; private set; }
    [SerializeField]    SlicedFilledImage _image;
    [SerializeField] private TextMeshProUGUI txtPercent;
    
        private void Awake()
        {
            Instance = this;
        }
        
        public void SetFillAmount(float amount)
        {
            _image.fillAmount = amount;
            txtPercent.text = $"{(int)(amount * 100)}%";
        }
    }
}