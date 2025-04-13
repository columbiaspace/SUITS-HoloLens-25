using UnityEngine;
using UnityEngine.UI;

public class MapZoom : MonoBehaviour
{
    public GameObject zoomedRockYardMap;  // This is your map sprite GameObject
    public Button MapZoomDisplay;         // This is your button that will trigger the sprite visibility

    private void Start()
    {
        zoomedRockYardMap.SetActive(false);  // Initially hide the zoomedRockYardMap sprite
        MapZoomDisplay.onClick.AddListener(ToggleSprite);  // Set up button to call ToggleSprite method when clicked
    }

    public void ToggleSprite()
    {
        zoomedRockYardMap.SetActive(!zoomedRockYardMap.activeSelf);  // Toggle the visibility of the zoomedRockYardMap sprite
        print("Adwerw");
    }
}
