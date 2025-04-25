using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // // Start is called before the first frame update
    // void Start()
    // {
    // }
    // Update is called once per frame
    // void Update()
    // {
    // }
    public GameObject MainMenuObj;
    public GameObject AnalysisScreenObj;
    public GameObject ViewDatabaseObj;
    public GameObject LoadingScreenObj;

    public void hideAllScreens(){
        MainMenuObj.SetActive(false);
        AnalysisScreenObj.SetActive(false);
        ViewDatabaseObj.SetActive(false);
        LoadingScreenObj.SetActive(true);
    }

    public void showMainScreen(){
        hideAllScreens();
        MainMenuObj.SetActive(true);
    }

    public void showDatabaseScreen(){
        hideAllScreens();
        ViewDatabaseObj.SetActive(true);
    }

    public void showRockAnalysisScreen(){
        hideAllScreens();
        AnalysisScreenObj.SetActive(true);
    }

    public void showLoadingScreen(){
        hideAllScreens();
        LoadingScreenObj.SetActive(true);
    }

    // public void showViewDatabaseScreen(){
    //     hideAllScreens();
    //     ViewDatabaseObj.SetActive(true);
    // }


}
