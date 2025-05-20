using UnityEngine;
using TMPro;

using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;

public class EvaTime : MonoBehaviour
{
    public TMP_Text display; // Assign this in Unity Inspector
    private float elapsedTime = 0f; // Start at 0:00
    private bool timerActive = false; // Timer starts paused

    void Start()
    {
        display.text = "0:00"; // Initial display
        StartCoroutine(GetDataFromServer()); // Start the coroutine to fetch data from the server
    }

    IEnumerator GetDataFromServer()
    {
        while (true) {
            string url = "http://127.0.0.1:8000/all";
            UnityWebRequest request = UnityWebRequest.Get(url); // Create a GET request to the FastAPI endpoint
            yield return request.SendWebRequest(); // Tells Unity to wait for the request to complete

            if (request.result == UnityWebRequest.Result.Success) // Check if the request was successful
            {
                string json = request.downloadHandler.text; // Get the raw JSON string returned by FastAPI
                TimeResponse data = JsonUtility.FromJson<TimeResponse>(json); // Turns that JSON into a C# object

                display.text = $"{data.eva_time}"; // Update the display with the time from the API

                // Convert API data to Vector3 and update the positions array in DropPin
                //dropPin.SetPositions(new Vector3[] { data.ToVector3() });
                //textDisplay.text = $"posx: {data.posx}\nposy: {data.posy}\nheading: {data.heading}";
            }
            else
            {
                Debug.LogError("API Error: " + request.error); // If the request fails, log the error in console
            }
            yield return new WaitForSecondsRealtime(1f); // Adjust this delay as needed
        }
    }

}


// TimeResponse class is used to deserialize the JSON response from the FastAPI server
[System.Serializable] // Makes the class serializable so JsonUtility can turn JSON into this object
public class TimeResponse
{
    // Fields should match the JSON keys returned by FastAPI
    public string eva_time;
}
