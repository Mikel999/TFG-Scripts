using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DianaControl : MonoBehaviour
{
    private List<int> allAnswers; //Diana guztien erantzun lista.

    public GameObject[] dianas; //diana guztien objektu arraya.
    private TextMeshPro[] dianaTexts; //diana guztien textu konponenteen arraya.

    private int randomIndex; //indize aleatorioa egiteko, erantzunak aleatorioki jartzeko.

    private int rightAnswer; //erantzun zuzena gordetzeko.
    private int rightAnswerIndex; //erantzun zuzenaren indizea gordetzeko.
    public GameObject rightAnswerEffect; //txarto erantzuten bada, zein zen erantzun zuzena jakiteko efektua.

    public AudioSource correctAnswerSound;
    // Start is called before the first frame update
    void Awake()
    {
        //Awake erabili start ordez, awake lehenago exekutatzen delako, bestela errorea ematen du
        //putanswers funtzioan, exekuzio ordenagatik.
        //Diana guztien textu konponenteak lortu.
        dianaTexts = GetComponentsInChildren<TextMeshPro>();
    }

    public void PutAnswers(HashSet<int> answers)
    {
        //dianaTexts = GetComponentsInChildren<TextMeshPro>();
        //Debug.Log(dianaTexts.Length + "text length");
        //Erantzunen hashseta lista batean bihurtu, indize bidez balioak lortu ahal izateko.
        allAnswers = new List<int>(answers);
        rightAnswer = allAnswers[0]; //erantzun zuzena beti lehen posizioan dago, beste hiruak random dira.
        
        for (int i = 0; i < dianaTexts.Length; i++)
        {
            //Indize aleatorioa sortu, 0-3 artean, Range(min, max+1);
            randomIndex = Random.Range(0, allAnswers.Count);
            
            if (allAnswers[randomIndex] == rightAnswer) //erantzun zuzenaren indizea gordetzeko.
            {
                rightAnswerIndex = i; //uneko forreko indizea adierazten du ze diana tratatzen ari den.
            }
            //Diana testuan jarri indize aleatorioa duen erantzuna.
            dianaTexts[i].text = allAnswers[randomIndex].ToString();
            //Erantzunetatik unean hartutakoa kendu, hurrengo bueltan berriro ez begiratzeko zbki bera.
            allAnswers.RemoveAt(randomIndex);
        }


    }

    //Jakiteko zein den erantzun zuzena, DianaCollision scriptetik deitzeko funtzioa.
    public int GetRightAnswer()
    {
        return rightAnswer;
    }


    //Erantzun okerra kolpatzen bada balarekin, zein den 
    //diana zuzena jakiteko efektua sortu, DianaCollision scriptetik deitzen da.
    public void WrongCollision()
    {
        Vector3 instPosition = dianas[rightAnswerIndex].transform.position;
        //Lurretik ateratzeko, balio finkoa kendu x ardatzean.
        instPosition.y -= 3.92f;
        Instantiate(rightAnswerEffect, instPosition, rightAnswerEffect.transform.rotation);
        correctAnswerSound.Play();
    }
}
