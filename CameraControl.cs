using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float sensitivityXaxis;
    public float sensitivityYaxis;
    float rotationY=0f;//Akumulatutako balioa, transform.rotation.x balioa gordetzen joateko

    // Start is called before the first frame update
    void Start()
    {
        //Kurtsorea erdian blokeatu, pantailatik ez ateratzeko.
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //Sagua ezkerrera/eskuinera mugitzen bada, pertsonaia norabide berdinean mugitu.
        float axisX = Input.GetAxis("Mouse X")*sensitivityXaxis* Time.deltaTime;

        //Pertsonaia errotatu: Y ardatzarekiko errotazioa.

        transform.Rotate(new Vector3(0, axisX, 0), Space.Self);

        //Sagua gora/behere mugitzen bada, kamara norabide berdinean mugitu.
        float axisY = Input.GetAxis("Mouse Y") * sensitivityYaxis * Time.deltaTime;
        
        //Gora mugitu nahi bada, negatiboki errotatu beharko da X ardatzean, beraz, Input norabidea alderantziz dago.
        rotationY -= axisY;//uneko transform.rotation.x-ren balioa gordeko da.

        //Camara errotatu: X ardatzarekiko errotazioa.
        //Errotazioa Y ardatzean 180ºkoa izango da gehienez jota.
        //Clamp funtzioarekin, angelua -90º eta 90º artean egongo da.
        rotationY=Mathf.Clamp(rotationY, -90f, 90f);

        Camera.main.transform.localRotation = Quaternion.Euler(rotationY,0f,0f);

    }

}

