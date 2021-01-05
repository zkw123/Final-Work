using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleGameObjectButton : MonoBehaviour
{
    public GameObject objectToToggle;
    public GameObject objectElse;
    public GameObject mainScenekart;
    public bool resetSelectionAfterClick;

    void Update()
    {
        resetSelectionAfterClick = true;
        if (objectToToggle.activeSelf && Input.GetButtonDown(GameConstants.k_ButtonNameCancel))
        {
            SetGameObjectActive(false);
        }
    }

    public void SetGameObjectActive(bool active)
    {
        if (objectElse.active == true)
        {
            objectElse.SetActive(!active);
        }
        if (objectToToggle.active == true)
        {
            objectToToggle.SetActive(!active);
        }
        else
        {
            objectToToggle.SetActive(active);
        }
        if (mainScenekart != null)
        {
            if (mainScenekart.active == true)
                mainScenekart.SetActive(!active);
            else
                mainScenekart.SetActive(active);
        }


        if (resetSelectionAfterClick)
            EventSystem.current.SetSelectedGameObject(null);
    }
}
