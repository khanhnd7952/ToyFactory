using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Dreamteck.Splines;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(SplinePositioner))]
public class BrickContainer : MonoBehaviour, IBrickContainer
{
    [SerializeField] private Transform[] slots;

    public List<Brick> Bricks { get; } = new List<Brick>(4);

    private SplinePositioner _positioner;
    private SplineComputer _splineComputer;

    private void Awake()
    {
        _positioner = GetComponent<SplinePositioner>();
    }

    public void SetSpline(SplineComputer splineComputer)
    {
        _splineComputer = splineComputer;
        _positioner.spline = splineComputer;
        _positioner.SetPercent(0f);
    }

    public async UniTask MoveBricks(Brick[] bricks)
    {
        for (var i = 0; i < bricks.Length; i++)
        {
            var brick = bricks[i];
            var target = slots[i];
            brick.transform.SetParent(target);
            brick.transform.DOLocalJump(Vector3.zero, 2f, 1, 0.15f).SetEase(Ease.InQuad);
            brick.transform.DOLocalRotate(Vector3.zero, 0.15f).SetEase(Ease.InQuad);
            await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
        }
    }

    [Button]
    void TestMoveBrick(Brick[] bricks)
    {
        MoveBricks(bricks).Forget();
    }

    public void UpdatePosition(float range)
    {
        var currentPos = _positioner.GetPercent();
        var newPos = currentPos + range;
        if (newPos > 1f) newPos -= 1;
        _positioner.SetPercent(newPos);
    }

    public float GetPosition()
    {
        return (float)_positioner.GetPercent();
    }

}