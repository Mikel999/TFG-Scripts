using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{

    //https://answers.unity.com/questions/1384868/help-understanding-game-controllers.html
    //hobeto ez erabiltzea gamecontroller bakarra oso nahaskorra egiten bada guztia

    //Script batetik beste batera deia egiteko, deitu nahi den scripta duen objektua kargatu behar da 
    //interfazetik
    public HealthBar hpScript; //HP barra kontrolatzen duen scripta.
    public MenuControl menu; //Menua kontrolatzen duen scripta.
    public DianaControl dianaControl; //dianak kontrolatzen dituen scripta.
    public QuestionController questionController; //Galderen canvasa kontrolatzen duen scripta.

    public GameObject gameOverMenu;
    public GameObject canvasTimeText; //Canvaseko denbora testuan aldaketak egiteko.
    public GameObject canvasPuntuationText; //Canvaseko puntuazio testua aldatzeko.
    public int timeHandicap;//zailtasun ezberdinen arteko denbora desberdintasuna.


    private int difficulty; //zailtasun maila
    private TextMeshProUGUI timeText; //denbora testua eguneratzeko aldagaia.
    private TextMeshProUGUI puntuationText; //puntuazio testua eguneratzeko aldagaia.
    private float timer; //zenbat denbora geratzen den kontrolatzeko, denbora eguneraketa.
    private float totalTimePerRound; //puntuazio kalkuluak egiteko, hasieran daukagun denbora jaso.
    private int puntuation;
    private bool respondedQuestion; //galdera erantzun bada jakiteko.
    private bool wrongAnswer; //galdera txarto erantzun bada, bizitza galdu.

    private bool oneTime; //galderak maneiatzeko behin sartzeko ifean.

    private int[] highScores; //Zailtasun araberako 3 highScoreak lortu, UpdateHighScore funtzioan erabilia

    public TextMeshProUGUI [] languageTextChange;
    private string[] englishTexts ={"Score:", "Time", "How much is", "Resume", "Menu", "Exit", "Paused Game" , "Restart", "Choose level",
        "Menu", "Exit", "Game Over"};

    private string[] spanishTexts ={"Puntuación:", "Tiempo", "¿Cuánto es" ,"Reanudar", "Menú", "Salir", "Juego Pausado", "Reiniciar", "Escoger nivel",
        "Menú", "Salir", "Fin del Juego"};

    private string[] basqueTexts = {"Puntuazioa:", "Denbora", "Zenbat da","Jarraitu", "Menua", "Irten", "Jokoa Eteta", "Berrabiarazi", "Maila aukeratu",
        "Menua", "Irten", "Joko Amaiera"};

    // Start is called before the first frame update
    void Start()
    {
        //Hizkuntzaren arabera aldatu canvasaren testuak
        ChangeMainSceneLanguage(PlayerPrefs.GetInt("LanguageVR"));

        //TextMeshPro konponenteak lortu behar dira, bertako testua aldatu ahal izateko.
        timeText = canvasTimeText.GetComponent<TextMeshProUGUI>();
        puntuationText = canvasPuntuationText.GetComponent<TextMeshProUGUI>();

        //Menu eszenatik zailtasuna lortu, horren araberako denbora ezarri ahal izateko.
        difficulty = menu.GetDifficulty();

        //Zailtasunaren araberako denbora.
        totalTimePerRound = 20 - difficulty * timeHandicap;

        //Eguneratuko den denbora balioa hasieratu, balio maximoarekin.
        timer = totalTimePerRound;

        //Hasierako puntuazioa.
        puntuation = 0;

        //Denbora testua eguneratu, hasierako balio maximoarekin.
        timeText.SetText(timer.ToString());

        //Jokoa berrabiarazten denean, eskala 1ra jartzeko.
        Time.timeScale = 1;

    }

    // Update is called once per frame
    void Update()
    {
        //Erantzuna okerra izan bada, edota denbora agortu bada, bizitza galdu.
        //Frame bat baino gehiagotan deitu ifari, baina behin baino ez kendu bizitza, galderei
        //zorizko efektua emateko.
        if ((respondedQuestion || timer<=0)) 
        {
            if ((wrongAnswer || timer <= 0) && !oneTime)
            {
                oneTime = true;
                wrongAnswer = false; //balioa eguneratu hurrengo galderarako.
                hpScript.TakeDamage(1); //bizitza kendu.
                questionController.DestroyBullets(); //unean aktibo dauden balak suntsitu,
                //denbora 0ra heltzen bada unean aktibo dauden beste jaurtigaiak talka ez egiteko.
                
                dianaControl.Invoke("WrongCollision", 0.25f);//Erantzuna okerra bada edo denb. agortu bada
                //erantzun zuzena zein izan den adierazteko efektua sortu.
            }

            //Oraindik bizitzak badauzka jokalariak, hurrengo galdera atera, bestela jokoa amaitu da.
            //Zorizko zenbaki askoren efektua ateratzeko, behin baino gehiagotan sartu funtzio honetan.
            if (hpScript.GetHp() > 0)
            {
                //Invoke, Time.timeScaleren arabera funtzionatzen du.
                //Hurrengo galdera ateratzeko trantsizioa moteltzeko, atzerapen bat jarri.
                Invoke("NextQuestion", 1.75f);
            }
            else //hpScript.GetHp() <= 0 bada.
            {
                //Bizitza guztiak galtzean, jokoa amaitu.
                //Funtzioa atzerapenarekin deitu, azkenengo galderaren erantzun zuzena ikustarazteko.
                Invoke("GameOver", 3f);
            }

        }
        else
        {
            //float aldagaia denez, toString erabiltzean "0" parametroa jartzean,
            //adieraziko da ez dela zenbakirik nahi koma eta gero, beraz, zenbaki osoak nahi direla.
            timeText.SetText(timer.ToString("0"));
        }

        //Galdera erantzuten bada edo denbora agortu bada, gelditu kontatzen, bestela, segi.
        if(!respondedQuestion){
            timer -= Time.deltaTime;
        }
    }

    //tiro egiteko gaitasuna kontrolatzeko.
    public float GetTotalTime()
    {
        return totalTimePerRound;
    }

    //tiro egiteko gaitasuna kontrolatzeko.
    public float GetTimer()
    {
        return timer;
    }
    //DianaCollision scriptetik deitzeko, kolisio zuzena gertatu bada.
    public void AddPuntuation() 
    {
        //Galdera bakoitza ondo erantzutean, 100 puntu jasoko dira gutxienez.
        //Puntu gehigarriak, denboraren araberakoak izango dira. Geroz eta azkarrago erantzun, hobeto.
        //Gehienez jota 150 puntu lortu ahalko dira.

        int timePuntuation = (int) (50 * timer/totalTimePerRound);
        puntuation += 100 + timePuntuation;

        //ToString("00000") jarriz parametro moduan, adierazi nahi da,
        //puntuazio testua 5 zifra izango dituela. Puntuazioa txikia bada, ezkerrean 0ekin beteko da.
        puntuationText.text = puntuation.ToString("00000");
    }


    //DianaCollision scriptetik funtzio honi deitu. Galdera okerra erantzun denean, true izango da balioa.
    public void SetWrongAnswer(bool wrong)
    {
        wrongAnswer = wrong;
    }

    //DianaCollision scriptetik funtzio honi deitu. Galdera erantzun denean, true izango da balioa.
    //Galdera eta erantzunak jarri direnean, QuestionController scriptetik invoke bidez berriro deitu.
    public void SetRespondedQuestion(bool resp)
    {
        respondedQuestion = resp;
    }
    //Weapon eta pause menu scriptetik deitzeko, ez erabiltzeko galdera berria sortzen ari den bitartean.
    public bool GetRespondedQuestion()
    {
        return respondedQuestion;
    }

    //Hurrengo galdera eta erantzunak sortzeko, QuestionController scriptari deitu.
    private void NextQuestion()
    {
        //Denbora erreseteatu hurrengo galderarako.
        timer = 20 - difficulty * timeHandicap; //(...)-1, galderaKop mod 20 galdera

        //Denbora testua eguneratu
        timeText.SetText(timer.ToString("0"));

        //Balioak eguneratu hurrengo galderarako.
        respondedQuestion = false;
        oneTime = false;

        //Galderaren canvaseko QuestionController osagaia aktibatu, hurrengo galdera kargatzeko.
        FindObjectOfType<Canvas>().GetComponent<QuestionController>().enabled = true;
    }

    //Bizitza guztiak galdu badira, denbora geldituko da, eta Game Over menua agertaraziko da.
    private void GameOver()
    {
        //Popup Game Over menua.
        gameOverMenu.SetActive(true);
    }


    //Partida amaitzen denean, puntuazioa uneko zailtasuneko highscoreekin konparatu, eta batenbat baino
    //handiagoa bada, lista eguneratu.
    public void UpdateHighScore()
    {
        //HighScores hitz gakoan gordeta dauden puntuazioak lortu, hutsik badago, bederatzi 0 lortu.
        string highScoresString = PlayerPrefs.GetString("HighScoresVR", "0 0 0 0 0 0 0 0 0");

        //HighScoreen stringa int array batera pasatu 9 balioak bakanduta izateko
        int[] highScoreArray = System.Array.ConvertAll(highScoresString.Split(' '), int.Parse);

        //Zailtasunaren araberako 3 highscoreak gorde partida amaieran konparaketa egiteko
        highScores = new int[3];
        for (int i= difficulty * 3; i < difficulty * 3 + 3; i++)
        {
            //Indizeak 0 eta 2 artekoak izateko, % erabili soberakina lortzeko
            highScores[i%3] = highScoreArray[i];
        }

        //Puntuazioa, highScoreen balio txikiena baino altuagoa bada, eguneratu arraya
        if (puntuation >= highScores[2]) 
        {
            highScores[2] = puntuation; //txikiena beti aterako da arraytik, ondoren ordenatu
            System.Array.Sort(highScores); //txikienetik handienera ordenatu 3 elementuko lista
            System.Array.Reverse(highScores); //buelta eman
        }

        //Behin uneko zailtasun araberako highScore lista eguneratu dela, 9 balioak dituen zerrendan 
        //eguneratu eta baioak gorde.
        for (int i = difficulty * 3; i < difficulty * 3 + 3; i++)
        {
            //Indizeak 0 eta 2 artekoak izateko, % erabili soberakina lortzeko
            highScoreArray[i] = highScores[i%3];
        }

        //High score stringa eguneratu eta Playerpref aldaketak gorde
        highScoresString = string.Join(" ", highScoreArray); //Arraya string batean elkartu 
        //join funtzio bidez, espazio banaketarekin

        PlayerPrefs.SetString("HighScoresVR", highScoresString);
        PlayerPrefs.Save();
        //Debug.Log(highScoresString);
    }

    //Eszena nagusiaren hizkuntza aldatzeko, menu nagusiko eszenan aukeratu den aukeraren arabera.
    private void ChangeMainSceneLanguage(int language)
    {
        switch (language)
        {
            case 0:
                //Beste aldagai guztien testuak aldatzeko
                for (int i = 0; i < englishTexts.Length; i++)
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
