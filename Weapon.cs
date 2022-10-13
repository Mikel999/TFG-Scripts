using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; //UIko elementuak kudeatzeko beharrezkoa
using UnityEngine.SceneManagement;//Eszenak kudeatu ahal izateko beharrezkoa

public class Weapon : MonoBehaviour
{
    public GameController gameController; //balak ez jaurtitzeko erantzun bada uneko galdera.
    public PauseMenuControl pauseMenu; //jokoa geldituta dagoen kontrolatzeko, tiro egitea baimentzeko.
    public GameObject shootTransform; //pistolaren ze puntutan aterako den bala.
    public float fireRate; //zenbana denbora egin daitekeen tiro.
    public GameObject bullet; //balak instantziatzeko erreferentzia objektua.
    public GameObject gunShotEffect; //tiro egiten denean sortuko den efektu bisuala.
    private float rateCounter; //fire ratea kontrolatzeko kontagailua.

    private LineRenderer lineR; //jaurtigaiak izango duen bidea adierazteko.
    private Vector3 hitPoint;
    public Material lineMaterial;
    public Color lineColor;

    public AudioSource shotSound;
    private Ray ray;
    private RaycastHit hitInfo;

    List<RaycastResult> raycastResults;
    // Start is called before the first frame update
    void Start()
    {
        //Tiro egiteko kontagailua gorde, eguneratu ahal izateko geroago.
        rateCounter = fireRate;

        lineR = shootTransform.GetComponent<LineRenderer>();
        //Lerro errenderizatzailea ikusi ahal izateko parametroen hasieraketa.
        lineR.sortingOrder = 1;
        lineR.material = lineMaterial;
        lineR.material.color = lineColor;
        lineR.startColor= lineColor;
        lineR.endColor= lineColor;
        lineR.positionCount=2;

    }

    // Update is called once per frame
    void Update()
    {
        //Inputaren egoera frame bakoitzeko eguneratu.
        OVRInput.Update();

        //Sortu izpia, armatik lerro zuzen batean joango dena.
        ray = new Ray(shootTransform.transform.position, -shootTransform.transform.right);

        //Hasierako puntua eguneratu uneoro.
        lineR.SetPosition(0, shootTransform.transform.position);
        
        rateCounter -= Time.deltaTime;


        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) && rateCounter<=0 && gameController.GetTimer()>0 &&
            !pauseMenu.IsGamePaused() && !gameController.GetRespondedQuestion() && 
            gameController.GetTotalTime()>gameController.GetTimer()+0.5f)
        {
            //Tiro infinitoak ez egiteko kontagailua, Fire rate.
            rateCounter = fireRate;
            //Tiro egiteko soinua sortu.
            shotSound.Play();
            //Tiro egiten den bakoitzean, partikula effektuak sortu, tiro puntuaren leku berdinean
            //baina kamararen edo robot rotazio berearekin. Armaren rotazioa hartuko balitz, 90º rotatuta agertuko litzateke
            Instantiate(gunShotEffect, shootTransform.transform.position, transform.parent.parent.rotation);

            //Bala sortarazi, armak duen tiro egiteko puntuan, eta rotazio berearekin(esfera denez berdin du rotazioa).
            Instantiate(bullet, shootTransform.transform.position, shootTransform.transform.rotation);

        }

        if (Physics.Raycast(ray, out hitInfo))//Izpia bota, eta zeozerren kontra talka egiten badu,
                                              //erantzuna hitInfo aldagaian jaso.
        {
            
            //Menu nagusian, talka punturaino marraztu marra.
            if (SceneManager.GetActiveScene().name.Equals("MenuScene"))
            {
                //Talka punturaino marraztu lerroa.
                hitPoint = hitInfo.point;
            }
            else
            {
                //Jokoaren maila eszenan, jokoa geldituta badago, lerroa luzatu, raycastaren talka punturarte
                //bestela, lerro motzago bat erabili.
                if(Time.timeScale==0)
                {
                    hitPoint = hitInfo.point;
                }
                else
                {
                    hitPoint = shootTransform.transform.position - shootTransform.transform.right * 2;
                }
            }

            if (hitInfo.collider.gameObject.CompareTag("Button"))
            {
                //Canvaseko botoi bati apuntatzean eta atzerako botoia sakatzean, botoia sakatu.
                if (OVRInput.Get(OVRInput.Button.Back))
                {
                    //Botoiaren onClick funtzioa deitu invoke erabiliz.
                    hitInfo.collider.gameObject.GetComponent<Button>().onClick.Invoke();
                }
            }
        }
        else //bestela
        {
            //Denbora gelditu bada, interfazeekin elkarreragiteko, lerroa luzatu, bestela motz utzi.
            if (Time.timeScale == 0)
            {
                hitPoint = shootTransform.transform.position - shootTransform.transform.right * 1000;
            }
            else
            {
                //Uneko posiziotik 2 unitateko luzeerara dagoen lerroa ezarri defektuz.
                hitPoint = shootTransform.transform.position - shootTransform.transform.right * 2;
            }
        }
        lineR.SetPosition(1, hitPoint);
    }


    /*void OnDrawGizmos()
    {

        hitPoint = shootTransform.transform.position - shootTransform.transform.right * 5;
        
        Gizmos.color = Color.red;
        //Gizmos.DrawRay(shootTransform.transform.position, hitPoint);
        Gizmos.DrawLine(shootTransform.transform.position, hitPoint);
    }*/
}
