using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ButtonLoadScene : MonoBehaviour,IPointerClickHandler
{
    [SerializeField] private string sceneName;


    public void OnPointerClick(PointerEventData eventData)
    {
        SceneManager.LoadScene(sceneName);
    }
}