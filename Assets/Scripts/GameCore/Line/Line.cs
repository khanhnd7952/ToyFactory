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
        _rows = new Queue<LineRow>();

        for (int i = 0; i < 100; i++)
        {
            var row = Instantiate(linePrefab, rowContainer);
            row.transform.localPosition = new Vector3(0, 0, -0.5f * i);
            _rows.Enqueue(row);
        }
    }

    public async void PushToConveyor()
    {
        if (_rows.Count == 0) return;
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
            var targetPos = new Vector3(0, 0, -0.5f * i);
            cache[i].transform.DOLocalMove(targetPos, 0.2f).SetEase(Ease.InSine);
        }
    }
}