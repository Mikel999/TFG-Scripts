using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletForce;
    public float destroyCounter;
    public GameObject bullet;
    //public AudioSource impactSound;

    public AudioSource impactSound;

    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //Aurrera bultzatu bala.
        rb.AddForce(-transform.right * bulletForce); //Z ardatzean bultzatu behar denez, 
        //.right bektorea erabili, arma 90ºko desbiderapena duelako. Noranzko negatiboan mugitzeko, -right.
    }

    // Update is called once per frame
    void Update()
    {
        //Denbora bat pasa bada eta ez badu talka egin, desagertarazi. Objektu gehiegi ez akumulatzeko eszenan
        //Jokoa moteldu baidezake.
        destroyCounter -= Time.deltaTime;
        if (destroyCounter <= 0)
        {
            Destroy(bullet);
        }
    }

    //Zeozerren kontra talka egitean, apurtu.
    private void OnCollisionEnter(Collision collision)
    {
        //Talka soinua sortu, juartigaiari audiosource osagaia gehitu zaio, bestela soinua ez zuen funtzionatzen.
        impactSound.Play();
        //Jaurtigaia desagertzeko, marrazkia eta colliderrak desagertarazi.
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<SphereCollider>().enabled = false;
        //Jaurtigaia apurtu, soinua amaitu denean.
        Destroy(bullet, impactSound.clip.length);
    }

}
