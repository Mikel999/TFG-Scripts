using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DianaCollision : MonoBehaviour
{

    public GameObject rightParticleEffect; //erantzuna zuzena bada, efektu berdea sortu.
    public GameObject wrongParticleEffect; //erantzuna okerra bada, efektu gorria sortu.

    public MenuControl menuControl; //zailtasuna lortzeko, GetDifficulty funtzioa erabiliz.
    public GameController gameController; //puntuazioa gehitzeko, scriptaren AddPuntuation funtzioa erabili.
    public QuestionController questionController; //aktibo dauden balak apurtzeko, DestroyBullets funtzio bidez.
    private DianaControl dianaControl; //Gurasoaren scripta erabiltzeko.
    private TextMeshPro dianaText; //dianaren testua lortzeko.

    private int gameDifficulty; //jokoaren zailtasuna gordetzeko.
    private float timeMoving; //mugimenduak daraman denbora.
    public float timePerMove; //mugimenduak iraungo duen denbora totala.
    public float movePower; //mugitzeko indarra.
    private Vector3 direction; //ituak mugituko diren norabidea.
    private int randomDirection; //norabide aleatorioa sortzeko balioa.
    private float angle; //angelua lortzeko.
    private bool turned; //norabideari buelta emateko.
    public AudioSource impactSound; //talka soinua.

    private Vector3 hasierakoPosizioa; //galdera bakoitzean birkokatzeko.

    void Start()
    {
        //Gurasotik DianaControl script konponentea lortu, GetRightAnswer funtzioa erabiltzeko.
        dianaControl = GetComponentInParent<DianaControl>();

        //Dianaren umeetan bilatu TMP konponentea, textuaren balioa talka egitean konparatu ahal izateko.
        dianaText = GetComponentInChildren<TextMeshPro>();

        //Hasierako posizioa gorde, galdera bakoitza hastean ituak hasiera posiziora mugitzeko.
        hasierakoPosizioa = transform.position;

        //Denbora kontagailu hasieraketa.
        timeMoving = timePerMove;

        //Menutik zailtasuna lortu.
        gameDifficulty = menuControl.GetDifficulty();

        //Mugituko den norabidea hasieran aukeratu.
        if (gameDifficulty != 0)
        {
            ChangeDirection();
        }
    }

    //Ituen mugimendua ezartzeko.
    private void Update()
    {
        //Mugimendua baimendu aukeratutako zailtasuna normala edo zaila denean.
        if (gameDifficulty != 0)
        {
            //Oraindik ez bada galdera erantzun, eta erantzuteko kontagailua hasi bada, mugitu ituak.
            if (!gameController.GetRespondedQuestion() && gameController.GetTimer() >= 0
                && gameController.GetTotalTime() > gameController.GetTimer() + 0.1f)
            {
                //Localposition, gurasoarekiko posizioa jakiteko, ez munduarekikoa.
                //Definitu den lauki baten barnean mugitzen dela egiaztatzeko.
                if (transform.localPosition.x >= -10 && transform.localPosition.x <= 45.6 &&
                    transform.localPosition.y >= -1.4 && transform.localPosition.y <= 12)
                {
                    //Itua mugitu.
                    MoveDiana(direction);
                }
                else //Itua definitu den laukitik ateratzen bada, buelta emango du.
                {
                    if (!turned)
                    {
                        //Norabideari buelta eman.
                        direction = -direction;
                        //Behin baino ez norabidea aldatzeko.
                        turned = true;
                    }
                    //Itua norabide berrian mugitu.
                    MoveDiana(direction);
                }
                if (timeMoving <= 0) //Mugimenduaren denbora agortu bada, zoriz beste norabide bat aukeratu.
                {
                    //Norabidea aldatu.
                    ChangeDirection();
                    //Mugimenduaren denbora eguneratu.
                    timeMoving = timePerMove;
                    //Buelta emateko balioa eguneratu.
                    turned = false;
                }
                timeMoving -= Time.deltaTime;
            }
            else //galdera erantzun bada, denbora agortu bada, edo galdera sortzen ari den bitartean.
            {
                //Ituak hasierako posizioetan jarri.
                transform.position = hasierakoPosizioa;
                //Denbora eguneratu.
                timeMoving = timePerMove;
                //Buelta emateko balioa eguneratu.
                turned = false;
                //Galdera bat hasten den bakoitzean, ituek kolisioarengatik izan dezaketen abiadura kendu.
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
        }

    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Bullet")) //Dianaren bat, bala batekin talka egiten badu.
        {
            //Destroy other active bullets
            questionController.DestroyBullets();

            //Galdera erantzun dela adierazi
            gameController.SetRespondedQuestion(true);

            //Talka soinua sortu, bestela jaurtigaien soinua bugeatzen da, jaurtigaia desagertzen delako.
            impactSound.Play();

            //Rightanswer integer balioa da, beraz, konparatzeko string motara pasatu behar da lehenengo.
            if (dianaControl.GetRightAnswer().ToString().Equals(dianaText.text))
            {
                //Partikula efektua sortu, dianaren posizio berean (zentroan), eta partikula efektuak 
                //daukan rotazio berearekin.
                Instantiate(rightParticleEffect, transform.position, rightParticleEffect.transform.rotation);

                //GameController scriptaren addPuntuation funtzioari deitu
                gameController.AddPuntuation();
            }
            else
            {
                //Erantzuna okerra izan dela adierazi.
                gameController.SetWrongAnswer(true);
                //Partikula-efektua sortu.
                Instantiate(wrongParticleEffect, transform.position, wrongParticleEffect.transform.rotation);
            }
        }
        else if (col.gameObject.CompareTag("Diana"))
        {
            //Beste diana batekin talka egiten badu, norabidea aldatu, kontrakoa ezarriz.
            if (!turned)
            {
                direction = -direction;
                turned = true;
            }
            MoveDiana(direction);
        }
        //Colisio bat gertatu eta gero, abiadura eta rotazioa kendu, abiadura ez gehitzeko.
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }
    //Diana norabide batean mugitzeko.
    private void MoveDiana(Vector3 dir)
    {
        //Indarraren arabera mugitu, parametro bezala pasatutako norabidean.
        transform.position += dir * movePower * Time.deltaTime;
    }

    //Ituen norabidea aldatzeko x segunduro.
    private void ChangeDirection()
    {
        //Zorizko norabidea aukeratu, zailtasun normalean, 4 norabide, zailean 8.
        randomDirection = Random.Range(0, 4 * gameDifficulty);

        //Zailtasuna normala bada, 4 norabidetan mugitu ahalko da itua, beraz, angeluak 90ºnaka. 
        //Zailtasun maximoan, 45ºnaka (8 norabide). Angelua radianetara pasatzeko, Deg2Rad ktearekin biderkatu.
        angle = 90 / gameDifficulty * randomDirection * Mathf.Deg2Rad;

        //Lortu den angeluaren araberako norabidea aukeratu.
        direction = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0);
    }
    private void OnCollisionExit(Collision col)
    {
        //Kolisiotik ateratzean, turned aldagaia eguneratu.
        if (col.gameObject.CompareTag("Diana"))
        {
            turned = false;
        }
    }
}