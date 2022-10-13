using System.Collections;
using System.Collections.Generic;
using TMPro; //TextMeshPro testuak erabiltzeko.
using UnityEngine;

public class QuestionController : MonoBehaviour
{
    public GameController gameController; //Jokoa kontrolatzen duen script nagusia, erlojua berbiarazteko.
    private TextMeshProUGUI timerText; //denbora testua eguneratzeko.

    public TextMeshProUGUI playerInterfaceQuestText; //galdera testua jokalari interfazean.

    public MenuControl menu; //Menua kontrolatzen duen scripta.
    public DianaControl dianas; //dianen balioak kontrolatzeko scripta
    private int difficulty;//zailtasun maila
    private HashSet<int> answers;

    private int questCount; //zein galderatan gauden jakiteko kontagailua

    //Galderak zailtasunaren arabera jartzeko;
    private int[] questNumbers = {1, 2, 3, 10, 4, 5, 6, 7, 8, 9, 11, 12, 13, 14, 15};
    int randomNumb;
    int randomNumb2;

    public AudioSource numberShuffle;

    // Start is called before the first frame update
    void Start()
    {
        //Jokuaren zailtasuna lortu, galderak zailtasun arabera egiteko.
        difficulty = menu.GetDifficulty();

        //Erantzunak gordetzeko hashset berria sortu.
        answers = new HashSet<int>();

        //Galdera eta erantzunak sortu.
        MakeQuestionAndAnswers(difficulty);

        //Scripta desaktibatu, hurrengo galderan aktibatu ahal izateko.
        GetComponent<QuestionController>().enabled = false;
    }


    private void OnEnable() //galdera canvasa aktibatzen den bakoitzean, start funtzioari deitu.
    {
        questCount++;
        if (questCount > 1) //ez bada lehen aldia kontrolatzen, bi aldiz deitzen da hasieran start funtzioa.
        {
            Start();
        }
    }

    public void MakeQuestionAndAnswers(int diff)
    {
        //Bi zenbaki aleatorio sortu, galdera sortu ahal izateko. Balioak zailtasun maila araberakoak.
        //Random.Range(), balio altuena baztertzen du: 0-10 eremuan nahi bada, Range(0,11) erabili behar da.
        switch (diff)
        {
            case 0: //1,2,3 edo 10 balioen artean aukeratzeko zailtasun errazean.
                randomNumb = questNumbers[Random.Range(0, 4)];
                randomNumb2 = questNumbers[Random.Range(0, 4)];
                break;
            case 1: //2,3,4,5,6 edo 10 balioen artean aukeratzeko zailtasun normalean.
                randomNumb = questNumbers[Random.Range(1, 7)];
                randomNumb2 = questNumbers[Random.Range(1, 7)];
                break;
            case 2: //2-15 balioen artean aukeratzeko.
                randomNumb = questNumbers[Random.Range(1, questNumbers.Length)];
                if (randomNumb >= 11) //12 - 15 arteko balioa ateratzen bada, hurrengo balioa 1-10 artekoa.
                {
                    randomNumb2 = questNumbers[Random.Range(1, 10)];
                }
                else
                {
                    randomNumb2 = questNumbers[Random.Range(1, questNumbers.Length)];
                }
                break;
        }
        

        //Sortutako bi zenbaki aleatorioak, jokalari interfazean jarri.
        playerInterfaceQuestText.text = randomNumb.ToString() +" x "+ randomNumb2.ToString()+"?";

        
        //4 dianentzako erantzunak sortu; 3 aleatorioak, erantzun zuzenetik hurbil egongo direnak.
        int rightAnswer = randomNumb * randomNumb2;
        answers.Add(rightAnswer);

        //4 emaitza lortzen ez diren bitartean, jarraitu loopean.
        while (answers.Count != 4){
            int randAnswer;
            if (rightAnswer >= 5)
            {
                //Zenbaki aleatorioa, erantzun zuzenetik -+5 distantziara egongo da gehienez jota.
                randAnswer = Random.Range( rightAnswer-5, rightAnswer+6);
            }
            else // 0 eta 5 arteko balioa bada, 0 eta 10 arteko zenbaki aleatorioa sortu, negatiboa ez izateko.
            {
                randAnswer = Random.Range(0, 11);
            }
            if (!answers.Contains(randAnswer))//lortutako zenbaki aleatorioa ez bada errepikatu, sartu.
            {
                answers.Add(randAnswer);
            }
        }
        //Zenbakien nahasketa soinua sortu.
        numberShuffle.Play();

        //4 Erantzunak dianetako textuetan jarri, aleatorioki.
        dianas.PutAnswers(answers);

        //Debug.Log(string.Join(",", answers)+" erantzunak");
    }


    //Unean aktibo dauden balak desagertarazi, 
    //gerta daitekeelako hurrengo galderako erantzuna jotzea nahigabe, edota
    //lehen bala jaurti eta erantzuna jo baino lehen hurrengo bala botatzea eta bi aldiz erantzutea.
    public void DestroyBullets()
    {
        //Gameobject.FindGameObjectsOfType<Bullet>() ondo detektatzen ditu balak, 
        //ondo borratzen ditu jokutik, baina aktibo zeuden balen prefabak inspektorean geratzen dira

        //FindGameObjectsWithTag metodoarekin aldiz, dena ondo egiten du.
        GameObject[] activeBullets = GameObject.FindGameObjectsWithTag("Bullet");
        for (int i = 0; i < activeBullets.Length; i++)
        {
            Destroy(activeBullets[i]);//objektuak apurtzeko markatzen dira, beraz, ez da problemarik
            //egongo arrayaren tamainarekin.
        }
    }
}
