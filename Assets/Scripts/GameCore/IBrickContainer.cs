using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public interface IBrickContainer
{
    Brick[] Bricks { get; }

    public UniTask MoveBricks(Brick[] bricks);
}