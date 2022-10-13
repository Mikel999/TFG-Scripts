using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; //UIko elementuak kudeatzeko beharrezkoa
using UnityEngine.SceneManagement;//Eszenak kudeatu ahal izateko beharrezkoa
using UnityEngine;

public class GameOverMenuControl : MonoBehaviour
{
    public GameController gameController;
    public string GameSceneName;
    public string MenuSceneName;
    public GameObject gameOverMenu;
    private static bool isLvlClicked; //eszenas aldatzean balioa mantentzeko.
    // Start is called before the first frame update
    void Start()
    {
        isLvlClicked = false;
    }
    void Update()
    {
        if (gameOverMenu.activeSelf)
        {
            //Sagua erakutsi
            Cursor.visible = true;
            //Denbora gelditu
            Time.timeScale = 0;
            //Kurtsorea desblokeatu, mugitu ahal izateko.
            Cursor.lockState = CursorLockMode.None; 
        }
    }
    public void ChooseLevel()
    {
        gameController.UpdateHighScore();
        isLvlClicked = true;
        SceneManager.LoadScene(MenuSceneName);
    }

    public void RestartGame()
    {
        gameController.UpdateHighScore();
        SceneManager.LoadScene(GameSceneName);
    }
    public bool GetIsLvlClicked()
    {
        return isLvlClicked;
    }
}
