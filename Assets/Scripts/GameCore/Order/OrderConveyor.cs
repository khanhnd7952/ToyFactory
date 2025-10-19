using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Dreamteck.Splines;
using UnityEngine;

public class OrderConveyor : MonoBehaviour
{
    public OrderConveyorData[] OrderData;
    public OrderModel modelPrefab;

    [SerializeField] private Transform slotCurrent, slotNext;

    private Queue<OrderConveyorData> _queue;

    public OrderModel CurrentOrder { get; private set; }
    public OrderModel NextOrder { get; private set; }


    public float Percent { get; private set; }

    private void Awake()
    {
        Percent = (float)GetComponent<SplinePositioner>().GetPercent();
    }

    private void Start()
    {
        _queue = new Queue<OrderConveyorData>();
        foreach (var data in OrderData)
            _queue.Enqueue(data);
    }

    public void StartOrder()
    {
        if (_queue.Count > 0) CheckOrder().Forget();
    }

    async UniTaskVoid CheckOrder()
    {
        if (CurrentOrder == null)
        {
            if (NextOrder != null)
            {
                CurrentOrder = NextOrder;
                NextOrder = null;
                CurrentOrder.transform.SetParent(slotCurrent);
                 CurrentOrder.transform.DOLocalMove(Vector3.zero, 0.2f).SetEase(Ease.OutBack);
            }
            else
            {
                if (_queue.Count == 0) return;
                var data = _queue.Dequeue();
                CurrentOrder = Instantiate(modelPrefab, slotCurrent);
                CurrentOrder.transform.localPosition = Vector3.zero;

                CurrentOrder.SetOrderData(data.OrderData);
            }

            if (CurrentOrder != null)
            {
                CurrentOrder.ShowDetail();
                CurrentOrder.OnComplete += OnCurrentOrderComplete;
            }
        }

        if (NextOrder == null)
        {
            if (_queue.Count > 0)
            {
                var data = _queue.Dequeue();
                NextOrder = Instantiate(modelPrefab, slotNext);
                NextOrder.SetOrderData(data.OrderData);

                NextOrder.transform.localPosition = Vector3.zero;
                NextOrder.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack).From(0);
            }
        }
    }

    void OnCurrentOrderComplete()
    {
        CurrentOrder = null;
        CheckOrder();
    }

    public Queue<EColorType> GetAllOrder()
    {
        var result = new Queue<EColorType>();
        foreach (OrderConveyorData conveyorData in OrderData)
        {
            foreach (var orderModelData in conveyorData.OrderData)
            {
                for (int i = 0; i < orderModelData.amount; i++)
                {
                    result.Enqueue(orderModelData.color);
                }
            }
        }

        return result;
    }
}

[System.Serializable]
public class OrderConveyorData
{
    public OrderModelData[] OrderData;
}