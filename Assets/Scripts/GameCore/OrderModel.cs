using System;
using DefaultNamespace;
using UnityEngine;

public class OrderModel : MonoBehaviour
{
    public OrderModelData[] colors;
}

[Serializable]
public class OrderModelData
{
    public EColorType color;
    public float amount;
}