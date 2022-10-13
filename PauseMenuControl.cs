using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; //UIko elementuak kudeatzeko beharrezkoa
using UnityEngine.SceneManagement;//Eszenak kudeatu ahal izateko beharrezkoa
using UnityEngine;

public class PauseMenuControl : MonoBehaviour
{
    private bool pausedGame; //jakiteko jokoa pausatua dagoen edo ez, falserekin hasieratua defektuz.
    public GameObject pauseMenu; //Pause menua kontrolatzeko.

    public GameController gameController;
    public string MenuSceneName;


    // Update is called once per frame
    void Update()
    {
        //Jokoa pausatzeko, jokoa pausatzeko botoia sakatu dela egiaztatu; eta, botoia sakatzen den unean
        //begiratu galdera canvasa desaktibatuta dagoela(galdera irakurtzeko denbora amaitu da,
        //edo oraindik denbora dago uneko galdera erantzuteko), denbora 0ra iritsi bada ezin da jokoa pausatu.
        if (OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad) && !gameController.GetRespondedQuestion()) 
        {
            pausedGame = !pausedGame; //true jokoa pausatzen bada, bestela false.
            Cursor.visible = pausedGame; //kurtsorea ikusgarri jokoa pausatuta badago, bestela ez.
            if (pausedGame)
            {
                Time.timeScale = 0; //jokoaren denbora gelditu.
                Cursor.lockState = CursorLockMode.None; //kurtsorea mugitu ahal izatea.
            }
            else
            {
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked; //kurtsorea blokeatu pantaila erdira.
            }
            
            pauseMenu.SetActive(pausedGame); //agertarazi pause menua.
        }
        //0.5 sec delay on pause button
    }

    public void ResumeGame() //botoi bidez kendu nahi bada pausa menua.
    {
        pausedGame = !pausedGame;
        Cursor.visible = pausedGame;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenu.SetActive(pausedGame);
    }


    public void LoadMenu() //Menura ateratzeko botoia sakatzen bada
    {
        //Eguneratu uneko puntuazioa egiaztatzeko ea highscore taulan sar daitekeen.
        gameController.UpdateHighScore();
        //Menu nagusiko eszena kargatu
        SceneManager.LoadScene(MenuSceneName);
    }
    public void ExitGame() //Jokutik ateratzeko botoia sakatzen bada
    {
        //Eguneratu uneko puntuazioa egiaztatzeko ea highscore taulan sar daitekeen.
        gameController.UpdateHighScore();
        //Aplikaziotik atera
        Application.Quit();
    }

    public bool IsGamePaused()
    {
        return pausedGame;
    }
}
