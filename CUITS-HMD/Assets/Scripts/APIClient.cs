using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

[System.Obsolete("APIClient is deprecated. Please use BackendDataService instead for all data access.", false)]
public class APIClient : MonoBehaviour
{
    public DropPin dropPin;
    public TextMeshProUGUI textDisplay;
    public static Vector3 LatestPosition1;
    public static Vector3 LatestPosition2;

    // for measuring intervals
    private float lastFetchTime = 0f;
    
    private bool warnedAboutDeprecation = false;

    void Start()
    {
        // Warn about using deprecated component
        Debug.LogWarning("APIClient is deprecated and will be removed in a future update. Please update your code to use BackendDataService singleton instead.");
        
        // Check if BackendDataService exists and is operating
        if (BackendDataService.Instance != null)
        {
            Debug.Log("BackendDataService is available - using it to update LatestPosition data for compatibility");
            // We continue to run the legacy APIClient for compatibility, but
            // we'll update the static position data from BackendDataService
        }
        else
        {
            Debug.LogError("BackendDataService is not present in the scene! Position data may be unavailable or incorrect.");
            StartCoroutine(GetDataFromServer()); // Fallback to old method
        }
    }
    
    void Update()
    {
        // Use BackendDataService if available to update the static properties
        // This ensures existing code using LatestPosition1/2 still works
        if (BackendDataService.Instance != null && 
            BackendDataService.Instance.LatestData != null)
        {
            var data = BackendDataService.Instance.LatestData;
            
            // Update EV1 position
            if (data.eva1 != null && data.eva1.imu != null)
            {
                float posx = data.eva1.imu.posx;
                float posy = data.eva1.imu.posy;
                
                if (posx != 0 || posy != 0)
                {
                    // Convert using the same formula as before
                    LatestPosition1 = new Vector3(0.162f * posx + 916.52f, 0.162f * posy + 1619.58f, -0.5f);
                    
                    // Update UI if text display is assigned
                    if (textDisplay != null) 
                    {
                        textDisplay.text = $"posx: {posx}\nposy: {posy}\nheading: {data.eva1.imu.heading}";
                    }
                    
                    // Update pin if assigned
                    if (dropPin != null)
                    {
                        dropPin.SetPosition(LatestPosition1);
                    }
                }
            }
            
            // Update EV2 position
            if (data.eva2 != null && data.eva2.imu != null)
            {
                float posx = data.eva2.imu.posx;
                float posy = data.eva2.imu.posy;
                
                if (posx != 0 || posy != 0)
                {
                    // Convert using the same formula as before
                    LatestPosition2 = new Vector3(0.162f * posx + 916.52f, 0.162f * posy + 1619.58f, -0.5f);
                }
            }
            
            // Only show the warning message once per session
            if (!warnedAboutDeprecation)
            {
                Debug.LogWarning("APIClient is using BackendDataService under the hood. Please update your code to access BackendDataService directly.");
                warnedAboutDeprecation = true;
            }
        }
    }

    // Legacy coroutine for fallback compatibility
    IEnumerator GetDataFromServer()
    {
        Debug.LogWarning("Using legacy APIClient data fetching. This is inefficient and will be removed in the future.");
        
        while (true)
        {
            float sendTime = Time.realtimeSinceStartup;
            
            //for ev1
            string url1 = "http://127.0.0.1:8000/eva1/imu";
            UnityWebRequest request = UnityWebRequest.Get(url1);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {   
                // measure the moment we got a response
                float receiveTime = Time.realtimeSinceStartup;
                float interval = receiveTime - lastFetchTime;
                lastFetchTime = receiveTime;

                string json = request.downloadHandler.text;
                PositionResponse data = JsonUtility.FromJson<PositionResponse>(json);

                Vector3 pos1 = data.ToVector3();
                dropPin.SetPosition(pos1);
                LatestPosition1 = pos1;
                textDisplay.text = $"posx: {data.posx}\nposy: {data.posy}\nheading: {data.heading}";

                // log interval AND values
                Debug.Log(
                        $"[API] Fetch #{Time.frameCount} @" +
                        $"{receiveTime:F2}s  Δ={interval:F3}s  " +
                        $"raw=(x:{data.posx:F2},y:{data.posy:F2},h:{data.heading:F2})  " +
                        $"world={pos1}"
                );
            }
            else
            {
                Debug.LogError("API Error: " + request.error);
            }

            //for ev2
            string url2 = "http://127.0.0.1:8000/eva2/imu";
            UnityWebRequest request2 = UnityWebRequest.Get(url2);
            yield return request2.SendWebRequest();

            if (request2.result == UnityWebRequest.Result.Success)
            {   
                // measure the moment we got a response
                float receiveTime = Time.realtimeSinceStartup;
                float interval = receiveTime - lastFetchTime;
                lastFetchTime = receiveTime;

                string json2 = request.downloadHandler.text;
                PositionResponse data2 = JsonUtility.FromJson<PositionResponse>(json2);

                Vector3 pos2 = data2.ToVector3();
                dropPin.SetPosition(pos2);
                LatestPosition2 = pos2;

                // log interval AND values
                Debug.Log(
                        $"[API] Fetch #{Time.frameCount} @" +
                        $"{receiveTime:F2}s  Δ={interval:F3}s  " +
                        $"raw=(x:{data2.posx:F2},y:{data2.posy:F2},h:{data2.heading:F2})  " +
                        $"world={pos2}"
                );
            }
            else
            {
                Debug.LogError("API Error: " + request.error);
            }
            
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
        //test changing to -0.5f instead of -0.35f
        return new Vector3(0.162f * posx + 916.52f, 0.162f * posy + 1619.58f, -0.5f);
    }
}