using UnityEngine;

public class UiManger : MonoBehaviour
{
    public GameObject mainMenuobj;
    public GameObject credits;

    //public Canvas gameCanvas;
    //public Canvas mainMenucan;
    public GameObject youSure;

    public GameObject howManyPlayer;
    public GameObject CanvasGame;
    public GameObject Finishobj;

    private void Start()
    {
        Finishobj.SetActive(false);
        mainMenuobj.SetActive(true);
        credits.SetActive(false);
        youSure.SetActive(false);
        howManyPlayer.SetActive(false);
        CanvasGame.SetActive(false);
    }

    public void MainMenuObj()
    {
        Finishobj.SetActive(false);
        mainMenuobj.SetActive(true);
        credits.SetActive(false);
        youSure.SetActive(false);
        howManyPlayer.SetActive(false);
        CanvasGame.SetActive(false);
    }

    public void HowManyPlayer_()
    {
        Finishobj.SetActive(false);
        howManyPlayer.SetActive(true);
        mainMenuobj.SetActive(false);
        credits.SetActive(false);
        youSure.SetActive(false);
    }

    public void Credits_()
    {
        Finishobj.SetActive(false);
        howManyPlayer.SetActive(false);
        mainMenuobj.SetActive(false);
        credits.SetActive(true);
        youSure.SetActive(false);
    }

    public void YouSure()
    {
        Finishobj.SetActive(false);
        howManyPlayer.SetActive(false);
        mainMenuobj.SetActive(false);
        credits.SetActive(false);
        youSure.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void GameStart()
    {
        Finishobj.SetActive(false);
        CanvasGame.SetActive(true);
        howManyPlayer.SetActive(false);
        mainMenuobj.SetActive(false);
        credits.SetActive(false);
        youSure.SetActive(false);
    }

    public void EndScene()
    {
        CanvasGame.SetActive(false);
        howManyPlayer.SetActive(false);
        mainMenuobj.SetActive(false);
        credits.SetActive(false);
        youSure.SetActive(false);
        Finishobj.SetActive(true);
    }
}