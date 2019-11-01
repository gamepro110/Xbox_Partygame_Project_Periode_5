using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManger : MonoBehaviour
{
    public GameObject mainMenuobj;
    public GameObject credits;
    public Canvas gameCanvas;
    public Canvas mainMenucan;
    public GameObject youSure;
    public GameObject howManyPlayer;

    private void Start()
    {
        mainMenuobj.SetActive(true);
        credits.SetActive(false);
        youSure.SetActive(false);
        howManyPlayer.SetActive(false);
    }
    public void MainMenuObj()
    {

    }
}
