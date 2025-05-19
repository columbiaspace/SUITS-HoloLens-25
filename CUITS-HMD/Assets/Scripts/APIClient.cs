using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class APIClient : MonoBehaviour
{
    public DropPin dropPin;
    public TextMeshProUGUI textDisplay;

    void Start()
    {
        StartCoroutine(GetDataFromServer()); // Start the coroutine to fetch data from the server
    }

    IEnumerator GetDataFromServer()
    {
        string url = "http://127.0.0.1:14141/eva1/imu";
        UnityWebRequest request = UnityWebRequest.Get(url); // Create a GET request to the FastAPI endpoint
        yield return request.SendWebRequest(); // Tells Unity to wait for the request to complete

        if (request.result == UnityWebRequest.Result.Success) // Check if the request was successful
        {
            string json = request.downloadHandler.text; // Get the raw JSON string returned by FastAPI
            PositionResponse data = JsonUtility.FromJson<PositionResponse>(json); // Turns that JSON into a C# object

            // Convert API data to Vector3 and update the positions array in DropPin
            dropPin.SetPositions(new Vector3[] { data.ToVector3() });
            textDisplay.text = $"posx: {data.posx}\nposy: {data.posy}\nheading: {data.heading}";
        }
        else
        {
            Debug.LogError("API Error: " + request.error); // If the request fails, log the error in console
        }
    }
}

// PositionResponse class is used to deserialize the JSON response from the FastAPI server
[System.Serializable] // Makes the class serializable so JsonUtility can turn JSON into this object
public class PositionResponse
{
    // Fields should match the JSON keys returned by FastAPI
    public float posx;
    public float posy;
    public float heading;

    // Converts the response data into a Unity Vector3 that can be used for positioning pins
    public Vector3 ToVector3()
    {
        return new Vector3(0.162f * posx + 916.52f, 0.162f * posy + 1619.58f, -0.35f);
    }
}
