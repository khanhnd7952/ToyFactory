using System;
using System.Collections.Generic;
using UnityEngine;

public class LineRow : MonoBehaviour
{
    private Brick[] _bricks;
    public Brick[] Bricks => _bricks;

    public EColorType Color { get; private set; }

    private void Awake()
    {
        _bricks = GetComponentsInChildren<Brick>();
    }

    public void SetColor(EColorType colorType)
    {
        Color = colorType;
        foreach (var brick in Bricks)
        {
            brick.SetColor(colorType);
        }
    }
}