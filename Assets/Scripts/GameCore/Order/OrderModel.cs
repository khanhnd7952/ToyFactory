using System;
using Config;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class OrderModel : MonoBehaviour
{
    [SerializeField] private GameObject detailContainer;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private OrderDetail[] details;
    [SerializeField] private SpriteRenderer orderPanel;


    OrderModelData[] _orderData;

    private void Awake()
    {
        detailContainer.SetActive(false);
    }

    public void SetOrderData(OrderModelData[] data)
    {
        _orderData = new OrderModelData[data.Length];
        for (int i = 0; i < data.Length; i++)
            _orderData[i] = new OrderModelData()
            {
                color = data[i].color,
                amount = data[i].amount * 4,
                predictAmount = data[i].amount * 4
            };

        for (int i = 0; i < details.Length; i++)
        {
            if (i < data.Length)
            {
                details[i].SetData(_orderData[i]);
            }
            else details[i].SetData(null);
        }

        var orderCount = data.Length;
        switch (orderCount)
        {
            case 1:
                orderPanel.size = new Vector2(12, 8.9f);
                break;
            case 2:
                orderPanel.size = new Vector2(12, 12f);
                break;
            case 3:
                orderPanel.size = new Vector2(12, 15.54f);
                break;
        }

        _renderer.sprite = ModelConfig.GetRandomSprite();
    }

    public bool IsHasColor(EColorType color)
    {
        foreach (var data in _orderData)
        {
            if (data.color == color && data.predictAmount > 0) return true;
        }

        return false;
    }

    public void ShowDetail()
    {
        detailContainer.SetActive(true);
    }

    public async UniTask Build(Brick[] bricks, Action onDone)
    {
        var color = bricks[0].Color;
        foreach (var data in _orderData)
        {
            if (data.color == color) data.predictAmount -= bricks.Length;
        }

        foreach (Brick brick in bricks)
        {
            brick.transform.SetParent(transform);
            var sequence = DOTween.Sequence();
            sequence.Append(brick.transform.DOLocalJump(Vector3.zero, 3f, 1, 0.15f).SetEase(Ease.InQuad));
            sequence.Append(brick.transform.DOLocalRotate(Vector3.zero, 0.15f).SetEase(Ease.InQuad));
            sequence.onComplete += () =>
            {
                foreach (var data in _orderData)
                {
                    if (data.color == color) data.DecreaseAmount();
                }

                CheckComplete();
                Destroy(brick.gameObject);
            };

            await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
        }

        onDone?.Invoke();
    }

    void CheckComplete()
    {
        if (_isComplete) return;
        foreach (var data in _orderData)
        {
            if (data.amount > 0) return;
        }

        Complete().Forget();
    }

    bool _isComplete = false;

    async UniTaskVoid Complete()
    {
        if (_isComplete) return;
        _isComplete = true;
        Debug.Log("Complete");
        transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
        {
            OnComplete?.Invoke(); 
            Destroy(gameObject);
        });
    }

    public Action OnComplete;
}

[Serializable]
public class OrderModelData
{
    public EColorType color;
    public int amount;
    [HideInInspector] public int predictAmount;

    public void DecreaseAmount()
    {
        amount--;
        OnChange?.Invoke();
    }

    public Action OnChange;
}