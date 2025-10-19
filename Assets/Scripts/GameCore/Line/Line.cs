using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Line : MonoBehaviour
{
    [SerializeField] private LineRow linePrefab;
    [SerializeField] private Transform rowContainer;

    Queue<LineRow> _rows = new Queue<LineRow>();

    private void Start()
    {
     
    }

    public void Spawn(EColorType[] Data)
    {
        foreach (var colorType in Data)
        {
            var row = Instantiate(linePrefab, rowContainer);
            row.SetColor(colorType);
            _rows.Enqueue(row);
        }
        RepositionRows();
    }

    public async void PushToConveyor()
    {
        if (_rows.Count == 0) return;
        if (Conveyor.Instance.IsFull()) return;
        await Conveyor.Instance.InsertBricksToConveyor(_rows.Dequeue().Bricks);
        RepositionRows();
    }

    public async void TryPushToConveyor()
    {
        if (_rows.Count == 0) return;
        if (!Conveyor.Instance.CanInsert()) return;
        await Conveyor.Instance.InsertBricksToConveyor(_rows.Dequeue().Bricks);
        RepositionRows();
    }

    void RepositionRows()
    {
        var cache = _rows.ToArray();
        for (int i = 0; i < cache.Length; i++)
        {
            var currentPos = cache[i].transform.localPosition;
            var targetPos = new Vector3(0, 0, -0.9f * i);
            cache[i].transform.DOLocalMove(targetPos, 0.2f).SetEase(Ease.InSine);
        }
    }
}