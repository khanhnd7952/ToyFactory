using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Dreamteck.Splines;
using Kelsey.UGUI;
using Sirenix.OdinInspector;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    public static Conveyor Instance { get; private set; }
    [SerializeField] public int Capacity = 20;
    [SerializeField] private float startLinePercent = 0.2f;

    [SerializeField] private SplineComputer splineComputer;
    [SerializeField] private BrickContainer ContainerPrefab;
    [SerializeField] private Transform containerParent;


    [ShowInInspector, ReadOnly] private List<BrickContainer> containers = new List<BrickContainer>();
    [SerializeField] private float moveSpeed = 0.1f;
    [SerializeField] private float insertMoveSpeed = 0.03f;

    Dictionary<float, OrderConveyor> _conveyorPositions = new Dictionary<float, OrderConveyor>();

    private Level _level;

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
        _level = Level.Instance;
    }

    public void Init()
    {
        _conveyorPositions = new Dictionary<float, OrderConveyor>();
        foreach (OrderConveyor orderConveyor in _level.OrderConveyors)
        {
            _conveyorPositions.Add(orderConveyor.Percent, orderConveyor);
        }
    }

    private void Update()
    {
        SortContainers();
        CheckBuildModel();
        UpdateConveyor();
        UpdateCapacityFill();
    }

    void UpdateCapacityFill()
    {
        ConveyorCapacity.Instance.SetFillAmount(containers.Count / (float)Capacity);
    }

    void SortContainers()
    {
        containers.Sort((a, b) =>
        {
            var posA = a.GetPosition();
            var posB = b.GetPosition();

            // Xử lý trường hợp container đi qua 1 sẽ về 0
            if (posA < startLinePercent) posA += 1f;
            if (posB < startLinePercent) posB += 1f;

            return posA.CompareTo(posB);
        });
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
            if (isLastElement && (_isBlocking || _isInserting) &&
                (currentPos < startLinePercent && Mathf.Abs(startLinePercent - currentPos) < _stepDistance))
            {
            }
            else
            {
                var subtract1 = nextPos - currentPos;
                if (subtract1 < 0 || subtract1 > _stepDistance)
                {
                    container.UpdatePosition(((_isInserting || _isBlocking) ? insertMoveSpeed : moveSpeed) *
                                             Time.deltaTime);
                }
            }
        }
    }

    void CheckBuildModel()
    {
        foreach (var conveyorMapping in _conveyorPositions)
        {
            var pos = conveyorMapping.Key;
            var model = conveyorMapping.Value.CurrentOrder;
            if (model == null) continue;

            foreach (var order in containers)
            {
                var percent = order.GetPosition();
                if (Mathf.Abs(percent - pos) < 0.005f)
                {
                    var hasColor = model.IsHasColor(order.Color);
                    if (hasColor)
                    {
                        containers.Remove(order);
                        model.Build(order.Bricks, () => { Destroy(order.gameObject); }).Forget();
                        break;
                    }
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
        if (IsFull()) return false;
        var first = containers[0];
        var percent = first.GetPosition();
        return percent >= startLinePercent + _stepDistance;
    }

    public void SetBlocking(bool blocking)
    {
        _isBlocking = blocking;
    }

    public async UniTask InsertBricksToConveyor(Brick[] bricks)
    {
        if (IsFull()) return;
        _isInserting = true;

        // while (!CanInsert())
        // {
        //     await UniTask.Yield();
        // }

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
        container.SetPosition(startLinePercent);
        //container.SetNeighbors(containers[0] , containers[^1]);
        containers.Add(container);
        return container;
    }
}