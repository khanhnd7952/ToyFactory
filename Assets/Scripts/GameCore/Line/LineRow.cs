using System;
using System.Collections.Generic;
using UnityEngine;

public class LineRow : MonoBehaviour
{
    private Brick[] _bricks;
    public Brick[] Bricks => _bricks;

    private void Awake()
    {
        _bricks = GetComponentsInChildren<Brick>();
    }
}