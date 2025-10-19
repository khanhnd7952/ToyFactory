using Config;
using TMPro;
using UnityEngine;

public class OrderDetail : MonoBehaviour
{
    [SerializeField] private TextMeshPro txtCount;
    [SerializeField] private MeshFilter _meshFilter;
    OrderModelData _data;

    public void SetData(OrderModelData data)
    {
        if (data == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
        _data = data;
        _data.OnChange += OnDataChange;
        var config = ColorConfig.GetConfig(_data.color);

        _meshFilter.mesh = config.BrickMesh;
        OnDataChange();
    }

    private void OnDataChange()
    {
        txtCount.text = _data.amount.ToString();
    }
}