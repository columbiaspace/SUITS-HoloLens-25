using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUITSMenu : MonoBehaviour
{
    public static CUITSMenu Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
