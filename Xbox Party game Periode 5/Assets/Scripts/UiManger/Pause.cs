using UnityEngine;
using XboxCtrlrInput;

public class Pause : MonoBehaviour
{
    public GameObject Pausemenu;
    public GameObject AreYouSure;
    private bool IsPaused = false;

    private void Start()
    {
        Pausemenu.SetActive(false);
        AreYouSure.SetActive(false);
    }

    private void Update()
    {
        if (XCI.GetButtonUp(XboxButton.Start, XboxController.Any)|| Input.GetKeyDown(KeyCode.Escape))
        {
            IsPaused = !IsPaused;
            UnPauseOrPause();
        }
    }

    private void UnPauseOrPause()
    {
        Pausemenu.SetActive(IsPaused);
        Time.timeScale = IsPaused ? 0 : 1;
    }

    public void QuitAreYouSure()
    {
        Pausemenu.SetActive(false);
        AreYouSure.SetActive(true);
    }

    public void PauseMenu()
    {
        Pausemenu.SetActive(true);
        AreYouSure.SetActive(false);
    }

    public void UnPause()
    {
        Pausemenu.SetActive(false);
        AreYouSure.SetActive(false);
        Time.timeScale = 1;
    }

    public void Quit()
    {
        Application.Quit();
    }
}