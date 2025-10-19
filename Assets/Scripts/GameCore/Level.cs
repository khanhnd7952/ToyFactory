using System;
using System.Collections.Generic;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class Level : MonoBehaviour
{
    public static Level Instance { get; private set; }
    public OrderConveyor[] OrderConveyors { get; private set; }

    public int[] Distributions = new int[]
    {
        1, 1, 1, 1
    };

    public EColorType[] LineData;

    private void Awake()
    {
        Instance = this;
    }

    public void Init()
    {
        OrderConveyors = GetComponentsInChildren<OrderConveyor>();
    }

    public void StartGame()
    {
        foreach (OrderConveyor orderConveyor in OrderConveyors)
        {
            orderConveyor.StartOrder();
        }
    }

    List<OrderModel> _cachedOrderModels = new List<OrderModel>(5);

    public List<OrderModel> GetCurrentOrderModels()
    {
        _cachedOrderModels.Clear();
        foreach (OrderConveyor orderConveyor in OrderConveyors)
        {
            if (orderConveyor.CurrentOrder != null) _cachedOrderModels.Add(orderConveyor.CurrentOrder);
        }

        return _cachedOrderModels;
    }

    [Button]
    void BakeLineData()
    {
        OrderConveyors = GetComponentsInChildren<OrderConveyor>();
        var Orders = new List<Queue<EColorType>>();
        foreach (var orderConveyor in OrderConveyors)
        {
            Orders.Add(orderConveyor.GetAllOrder());
        }

        var result = new List<EColorType>();

        int index = 0;
        for (int i = 0; i < 1000; i++)
        {
            foreach (var queue in Orders)
            {
                if (queue.Count > 0)
                {
                    result.Add(queue.Dequeue());
                }
            }
        }
        
        // Chuyển đổi thành array và shuffle với distribution logic
        LineData = ShuffleWithDistribution(result, Distributions);

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
    
    private EColorType[] ShuffleWithDistribution(List<EColorType> sourceData, int[] distributions)
    {
        // if (sourceData == null || sourceData.Length <= 1 || distributions == null || distributions.Length == 0) 
        //     return sourceData;
        
        var queue = new Queue<EColorType>(sourceData);
        var count = queue.Count;
        Debug.Log(count);
        // Tính tổng weight
        int totalWeight = 0;
        foreach (int weight in distributions)
        {
            totalWeight += weight;
        }
        
        // Tạo list mới để sắp xếp lại
        var newData = new List<EColorType>();
        
        // Lặp qua từng distribution step
        for (int step = 0; step < distributions.Length; step++)
        {
            // Tính số lượng color cho step này
            int stepCount = Mathf.RoundToInt((float)distributions[step] / totalWeight * count);
            
            var list = new List<EColorType>();
            // Lấy ngẫu nhiên và shuffle color cho step này
            for (int i = 0; i < stepCount; i++)
            {
                if(queue.Count == 0) break;
                list.Add(queue.Dequeue());
            }
            list.MMShuffle();
            newData.AddRange(list);
        }

        if (queue.Count > 0)
        {
            newData.AddRange(queue);
        }
        
        return newData.ToArray();
    }
}