using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonLinker : MonoBehaviour
{
    public Button primaryButton;
    public Button linkedButton;

    void Start()
    {
        primaryButton.onClick.AddListener(() => LinkButtonSelection());
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == primaryButton.gameObject)
        {
            EventSystem.current.SetSelectedGameObject(linkedButton.gameObject);
        }
    }

    void LinkButtonSelection()
    {
        EventSystem.current.SetSelectedGameObject(linkedButton.gameObject);
    }
}
