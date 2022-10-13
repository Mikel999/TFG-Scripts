using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 
using UnityEngine.UI; //UIko elementuak kudeatzeko beharrezkoa
using UnityEngine.SceneManagement;//Eszenak kudeatu ahal izateko beharrezkoa

public class MenuControl : MonoBehaviour
{
    public GameOverMenuControl gameOverScript; //kontrolatzeko ea game over menua atera den.
    public string GameSceneName; //jokoaren eszena kargatzeko.
    public Button playButton; //Play botoia, script bidez sakatzeko.
    private static int gameDifficulty;//static ez bada, eszena aldatzean ez da balioa mantenduko.

    public TextMeshProUGUI highScoreText;
    private int h1, h2, h3; //highScoreak maneiatzeko, 3naka erakutsiko direlako.


    public TextMeshProUGUI[] languageTextChange; //hizkintzaz aldatuko diren testu guztiak
    private string[] englishTexts ={"Play", "HighScores", "Options", "Exit", "Easy", "Medium" , "Hard", "Back",
        "Highest scores per level", "Back", "Options", "Language", "Volume", "Background Music", "Sound Effects","Back", 
        "Easy", "Medium" , "Hard", "On", "Off", "On", "Off", "On", "Off"};

    private string[] spanishTexts ={"Jugar", "Puntuaciones", "Opciones", "Salir", "Fácil", "Normal", "Difícil", "Atrás",
        "Puntuaciones más altas por nivel", "Atrás", "Opciones", "Lenguaje", "Volumen", "Música de fondo", "Efectos de sonido", "Atrás", 
        "Fácil", "Normal", "Difícil", "Activar", "Desactivar", "Activar", "Desactivar", "Activar", "Desactivar"};

    private string[] basqueTexts = {"Jolastu", "Puntuazioak", "Aukerak", "Irten", "Erraza", "Normala", "Zaila", "Atzera",
        "Puntuazio altuenak maila arabera", "Atzera", "Aukerak", "Hizkuntza", "Bolumena", "Atzeko musika", "Soinu efektuak", "Atzera", 
        "Erraza", "Normala", "Zaila", "Gaitu", "Ezgaitu", "Gaitu", "Ezgaitu", "Gaitu", "Ezgaitu"};

    private bool setActiveButton;

    public Button volumeOn, volumeOff;
    public Button backgroundMusicOn, backgroundMusicOff;
    public Button soundEffectsOn, soundEffectsOff;

    void Start()
    {
        //Bolumenaren botoien balioak (aktibo dauden) eguneratu, aurretiko saioaren balioen arabera, 
        //lehen saioa bada, defektuz 1 balioa ezarri guztietan.
        //Balio boolearra behar denez VRko kasuan, 1 edo 0 balioak lortu eta boolearrera pasatu.
        setActiveButton = System.Convert.ToBoolean(PlayerPrefs.GetFloat("VolumeVR", 1));
        volumeOn.gameObject.SetActive(!setActiveButton);
        volumeOff.gameObject.SetActive(setActiveButton);
        setActiveButton = System.Convert.ToBoolean(PlayerPrefs.GetFloat("BackgroundMusicVR", 1));
        backgroundMusicOn.gameObject.SetActive(!setActiveButton);
        backgroundMusicOff.gameObject.SetActive(setActiveButton);
        setActiveButton = System.Convert.ToBoolean(PlayerPrefs.GetFloat("SoundEffectsVR", 1));
        soundEffectsOn.gameObject.SetActive(!setActiveButton);
        soundEffectsOff.gameObject.SetActive(setActiveButton);


        if (gameOverScript.GetIsLvlClicked()) //Jokoa amaitu bada, eta chooselevel botoia sakatu bada.
        {
            playButton.onClick.Invoke(); //Menu nagusiko play botoia sakatu, script bidez.
        }
        //Puntuazio altuenen maila kargatzean, defektuz maila erraza erakutsiko da.
        ShowHighScores(0);

        //Joko hasieran, edo menu eszena kargatzen den bakoitzean hizkuntza aldatu, 
        //dropdown menuan lehenago aukeratu denaren arabera
        //lehen aldia bada, 2 hartu defektuz.
        ChangeLanguage(PlayerPrefs.GetInt("LanguageVR", 2));
    }

    public void StartGame() //Edozein zailtasuna aukeratzean, hurrengo eszena kargatuko da.
    {
        SceneManager.LoadScene(GameSceneName);
    }

    public void SetDifficulty(int diff) //zailtasun botoiren batean klikatzean zailtasuna ezarri.
    {
        gameDifficulty = diff;
        //Debug.Log(gameDifficulty);
    }
    public int GetDifficulty() //GameController scriptetik zailtasun balioa lortzeko.
    {
        return gameDifficulty;
    }
    public void ExitGame() //Jokutik ateratzeko botoia sakatzean
    {
        Application.Quit();
    }

    //highscoreen menuko zerrenda textutik int lortu, horren arabera 3 balioak lortu highscoreetatik
    //for (int i=zailtasuna*3;i<zailtasuna*3+3;i++)
    //highscoreak erreseteatzeko aukera eman botoi bidez
    public void ShowHighScores(int diff)
    {
        string highScoresString = PlayerPrefs.GetString("HighScoresVR", "0 0 0 0 0 0 0 0 0");
        int[] highScoreArray = System.Array.ConvertAll(highScoresString.Split(' '), int.Parse);

        //Zailtasunaren araberako hiru balioak lortu.
        h1=highScoreArray[diff * 3];
        h2=highScoreArray[diff * 3 + 1];
        h3=highScoreArray[diff * 3 + 2];

        //Textua eguneratu balio berriekin, posizio egokiarekin. Balioek 5 zifra izango dute,
        //5 zifra baino txikiagoko balioak badira, 0ekin beteko dira aurreko posizioak.
        highScoreText.text = ("1. "+ h1.ToString("00000")+"\n"
            +"2. "+ h2.ToString("00000") +"\n"
            +"3. "+ h3.ToString("00000"));

    }

    //Lengoaia aukeratzeko zerrendatik balioa lortu, 0 ingelesa, 1 gaztelania, 2 euskera
    //Balio horien araberako textuak idazteko.
    public void ChangeLanguage(int language)
    {
        //Lengoaia int moduan gorde testu fitxategi batean, eguneratua mantentzeko hurrengo sesio hasieraketetan.
        //Hizkuntza aldatzeko aukera sakatzen den bakoitzean eguneratuko da txt fitxategiko hizkuntza balioa.
        PlayerPrefs.SetInt("LanguageVR", language);
        PlayerPrefs.Save();

        switch (language)
        {
            case 0:
                //Beste aldagai guztien testuak aldatzeko
                for (int i=0;i<englishTexts.Length;i++)
                {
                    languageTextChange[i].text = englishTexts[i];
                }
                break;
            case 1:
                //Beste aldagai guztien testuak aldatzeko
                for (int i = 0; i < englishTexts.Length; i++)
                {
                    languageTextChange[i].text = spanishTexts[i];
                }
                break;
            case 2:        
                //Beste aldagai guztien testuak aldatzeko
                for (int i = 0; i < englishTexts.Length; i++)
                {
                    languageTextChange[i].text = basqueTexts[i];
                }
                break;
        }
    }
}
