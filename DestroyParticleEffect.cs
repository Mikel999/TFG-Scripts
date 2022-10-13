using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticleEffect : MonoBehaviour
{
    public float timeToDestroy;
    public GameObject particleEffect;
    // Update is called once per frame
    void Update()
    {
        //Denbora kontagailua eguneratu
        timeToDestroy -= Time.deltaTime;
        if (timeToDestroy <= 0)
        {
            Destroy(particleEffect);//Denbora pasata, instantziatutako objektua suntsitu, memoria ez okupatzeko.
        }
    }
}
