using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Dreamteck.Splines;
using Sirenix.OdinInspector;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    public static Conveyor Instance { get; private set; }
    [SerializeField] public int Capacity = 20;

    [SerializeField] private SplineComputer splineComputer;
    [SerializeField] private BrickContainer ContainerPrefab;
    [SerializeField] private Transform containerParent;


    [ShowInInspector, ReadOnly] private List<BrickContainer> containers = new List<BrickContainer>();
    [SerializeField] private float moveSpeed = 0.1f;
    [SerializeField] private float insertMoveSpeed = 0.03f;
    Dictionary<float, BrickContainer> _containerPositions = new Dictionary<float, BrickContainer>();

    private bool _isBlocking = false;

    bool _isInserting = false;
    private float _stepDistance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _stepDistance = 0.98f / Capacity;
    }

    private void Update()
    {
        SortContainers();
        UpdateConveyor();
    }

    void SortContainers()
    {
        _containerPositions.Clear();
        containers.Sort((a, b) => a.GetPosition().CompareTo(b.GetPosition()));
        foreach (var container in containers)
        {
            var pos = container.GetPosition();
            _containerPositions.Add(pos, container);
        }
    }

    void UpdateConveyor()
    {
        if (containers.Count == 1)
        {
            containers[0].UpdatePosition(moveSpeed * Time.deltaTime);
            return;
        }

        for (int i = containers.Count - 1; i >= 0; i--)
        {
            var isLastElement = i == containers.Count - 1;
            var container = containers[i];
            var nextContainer = isLastElement ? containers[0] : containers[i + 1];
            var currentPos = container.GetPosition();
            var nextPos = nextContainer.GetPosition();
            // check if subtract position is greater than step distance, position is between 0 and 1
            if (isLastElement && (_isBlocking || _isInserting) && currentPos >= 1 - _stepDistance)
            {
            }
            else
            {
                var subtract1 = Mathf.Abs(currentPos - nextPos);
                if (subtract1 > _stepDistance)
                {
                    container.UpdatePosition((_isInserting || _isBlocking ? insertMoveSpeed : moveSpeed) * Time.deltaTime);
                }
            }
        }
    }

    public bool IsFull()
    {
        return containers.Count >= Capacity;
    }

    public bool CanInsert()
    {
        if (containers.Count == 0) return true;
        var first = containers[0];
        var percent = first.GetPosition();
        return percent >= _stepDistance;
    }

    public void SetBlocking(bool blocking)
    {
        _isBlocking = blocking;
    }

    public async UniTask InsertBricksToConveyor(Brick[] bricks)
    {
        if (IsFull()) return;
        _isInserting = true;
        
        while (!CanInsert())
        {
            await UniTask.Yield();
        }

        var container = CreateContainer();
        await UniTask.WaitForEndOfFrame();
        await UniTask.Delay(TimeSpan.FromSeconds(0.02f));
        await container.MoveBricks(bricks);
        _isInserting = false;
    }

    [Button]
    BrickContainer CreateContainer()
    {
        var container = Instantiate(ContainerPrefab, containerParent);
        container.SetSpline(splineComputer);
        //container.SetNeighbors(containers[0] , containers[^1]);
        containers.Add(container);
        return container;
    }
}