using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class APIClient : MonoBehaviour
{
    public DropPin dropPin;
    public TextMeshProUGUI textDisplay;
    public static Vector3 LatestPosition;

    // for measuring intervals
    private float lastFetchTime = 0f;

    void Start()
    {
        StartCoroutine(GetDataFromServer()); // Start the coroutine to fetch data from the server
    }

    IEnumerator GetDataFromServer()
    {
        while (true)
        {
            float sendTime = Time.realtimeSinceStartup;

            string url = "http://127.0.0.1:8000/eva1/imu";
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {   
                // measure the moment we got a response
                float receiveTime = Time.realtimeSinceStartup;
                float interval = receiveTime - lastFetchTime;
                lastFetchTime = receiveTime;

                string json = request.downloadHandler.text;
                PositionResponse data = JsonUtility.FromJson<PositionResponse>(json);

                Vector3 pos = data.ToVector3();
                dropPin.SetPosition(pos);
                LatestPosition = pos;
                textDisplay.text = $"posx: {data.posx}\nposy: {data.posy}\nheading: {data.heading}";

                // log interval AND values
                Debug.Log(
                        $"[API] Fetch #{Time.frameCount} @" +
                        $"{receiveTime:F2}s  Δ={interval:F3}s  " +
                        $"raw=(x:{data.posx:F2},y:{data.posy:F2},h:{data.heading:F2})  " +
                        $"world={pos}"
                );


            }
            else
            {
                Debug.LogError("API Error: " + request.error);
            }
            //WaitForSecondsRealtime changed to this 
            yield return new WaitForSecondsRealtime(1f); // Adjust this delay as needed
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
