using Config;
using Sirenix.OdinInspector;
using UnityEngine;

public class Brick : MonoBehaviour
{
    public EColorType Color;
    private IBrickContainer _container;

    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer[] renderers;

    [Button]
    public void SetColor(EColorType c)
    {
        Color = c;

        var config = ColorConfig.GetConfig(c);
        // foreach (var renderer in renderers)
        // {
        //     renderer.material = config.BrickMaterial;
        // }
        meshFilter.mesh = config.BrickMesh;
    }
}