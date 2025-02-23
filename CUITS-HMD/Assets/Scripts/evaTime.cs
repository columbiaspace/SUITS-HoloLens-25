using UnityEngine;
using TMPro;

public class EvaTime : MonoBehaviour
{
    public TMP_Text display; // Assign this in Unity Inspector
    private float elapsedTime = 0f; // Start at 0:00
    private bool timerActive = false; // Timer starts paused

    void Start()
    {
        display.text = "0:00"; // Initial display
    }

    void Update()
    {
        // Check if any button is pressed (Input.GetButton or Input.anyKey)
        if (!timerActive && (Input.anyKeyDown || Input.GetMouseButtonDown(0))) // Can use other conditions if you want more specific buttons
        {
            timerActive = true;
        }

        // If the timer is active, count up the time
        if (timerActive)
        {
            elapsedTime += Time.deltaTime;

            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);

            display.text = $"{minutes}:{seconds:D2}"; // Format the timer correctly
        }
    }
}
