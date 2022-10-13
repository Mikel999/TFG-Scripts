using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; //UI elementuak erabiltzeko beharrezkoa

public class HealthBar : MonoBehaviour
{
    public int MaxHealth; //Jokalariak izango duen bizitza kopuru maximoa
    public Image hpBar; //Sprite irudia eguneratzeko
    public TextMeshProUGUI hpNumber; //Bizitza barraren zenbakiak eguneratzeko
    private int CurrentHealth; //Uneko bizitzak

    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = MaxHealth;
        hpNumber.SetText(CurrentHealth + " / " + MaxHealth);
        hpBar.fillAmount = 1;
    }

    public int GetHp()
    {
        return CurrentHealth; //konprobatzeko ea jokoa amaitu den (if CurrentHp==0) end game
    }

    public void TakeDamage(int dmg)
    {
        CurrentHealth -= dmg; //erantzun oker bakoitzeko, bizitza kendu
        hpNumber.SetText(CurrentHealth + " / " + MaxHealth); //bizitzako barraren zenbakiak eguneratu
        hpBar.fillAmount = (float) CurrentHealth / MaxHealth; //[0,1] tarteko balioa izan behar da
    }


}
