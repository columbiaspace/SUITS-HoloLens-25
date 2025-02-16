using UnityEngine;
using UnityEngine.UI;

public class ArrowToggler : MonoBehaviour
{
    // Reference to the arrow GameObject
    public GameObject arrowObject;
    
    // Reference to the toggle button
    //public Button toggleButton;
    
    private void Start()
    {
    }

    public void ToggleArrowVisibility()
    {
        if (arrowObject != null)
        {
            // Toggle the arrow's visibility
            arrowObject.SetActive(!arrowObject.activeSelf);
        }
    }

    private void OnDestroy()
    {

    }
}