using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MusicController : MonoBehaviour
{
    private static MusicController musicController; //kontrolatzeko objektua ez dela errepikatzen.

    public List<AudioSource> songList; //erabiliko diren abesti guztien lista, lehena menukoa izango da.

    private List<AudioSource> copyList; //abestien lista husten joateko.

    private AudioSource currentSong; //uneko abestia.
    private float songLength; //uneko abestiaren luzapena.
    private string currentScene; //uneko eszenaren izena.


    public AudioMixer audioMixer; //soinuak aldatzeko.


    void Awake()
    {
        //Kopia bat mantendu.
        copyList = new List<AudioSource>(songList);

        //Lehen abestia jo, menua kargatuta dagoen bitartean loop egingo duena.
        songLength = copyList[0].clip.length;

        //Uneko eszenaren izena gorde, jakiteko noiz aldatzen den eszenaz.
        currentScene = SceneManager.GetActiveScene().name;

        //Eszenen artean objektua mantentzeko, baina kopiarik ez sortzeko menura itzultzean.
        //Jokalariak menura itzultzen den bakoitzean, Music GameObjectaren kopia bat sortuko da, 
        //eta kopia hori ezabatu behar da.
        if (musicController == null) //Lehen music objektua bada
        {
            musicController = this; //Aldagaia eguneratu
            DontDestroyOnLoad(this); //Ez apurtzeko ezarri
        }
        else
        {
            //Bestela, suntsitu.
            Destroy(this);
        }
        //Funtzioen hasieraketak egin.
        VolumeChange(System.Convert.ToBoolean(PlayerPrefs.GetFloat("VolumeVR", 1)));
        BackgroundMusicChange(System.Convert.ToBoolean(PlayerPrefs.GetFloat("BackgroundMusicVR", 1)));
        SoundEffectsChange(System.Convert.ToBoolean(PlayerPrefs.GetFloat("SoundEffectsVR", 1)));
    }

    // Update is called once per frame
    void Update()
    {
        //Eszena aldatzen den bakoitzean, OnSceneLoaded funtzioa deitu.
        //SceneManager.sceneLoaded += OnSceneLoaded;
        if (SceneManager.GetActiveScene().name != currentScene)
        {
            OnSceneLoaded();
        }

        if (songLength <= 0) //Abestiaren denbora agortu bada, aldatu abestiz.
        {
            ChangeMusic(0, songList.Count);
        }
        //timescale / pitch
        songLength -= Time.unscaledDeltaTime; //Jokoa gelditzean kontatzen jarraitzeko.
    }

    //Abestiaz aldatu.
    private void ChangeMusic(int low, int high)
    {
        if (songList.Count == 0)
        {
            //Lista hustu bada, berriro sortu.
            songList = new List<AudioSource>(copyList);
        }

        //Zorizko zenbakia lortu.
        int randomSong = Random.Range(low, high);
        //Abestia lortu, eta abestiaren iraupena.
        currentSong = songList[randomSong];
        songLength = currentSong.clip.length;

        //Abestia ez errepikatzeko, listatik kendu.
        songList.RemoveAt(randomSong);

        //Abestia jo.
        currentSong.Play();
    }

    //private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    private void OnSceneLoaded()
    {
        //Balioa eguneratu
        currentScene = SceneManager.GetActiveScene().name;
        //Kargatu den eszena menu nagusia bada.
        if (currentScene.Equals("MenuScene"))
        {
            //Menu nagusia kargatzen bada, menuko abestia jo, loop aldagaia true jarriz.
            //songList aldagaia eraldatua egon daitekeenez, copylist aldagaia erabili.
            copyList[0].loop = true;

            copyList[0].Play();
            //Unean jotzen ari den abestia gelditu.
            currentSong.Stop();
        }
        else
        {
            //Behin jokoa hasi dela, menuko abestiaren loop ezaugarria desaktibatu.
            copyList[0].loop = false;
            //Menuko abestia gelditu.
            copyList[0].Stop();
            //Abestiaz aldatu eszena kargatzean, lehen aldian, menuko abestia berriz ez ateratzeko,
            //1 eta listaren luzera arteko balioa lortuko da random funtziaren bidez.
            ChangeMusic(1, songList.Count);
        }
    }

    //0.0001 eta 1 balioen artean egongo da bolumen balioa, dezibelioetara pasatzean,
    //Audio Mixerra 80 eta 0 balioen artean egoteko.
    public void VolumeChange(bool volume)
    {
        //Setfloat parametro bezala, Audio mixer taldeak daukan Exposed Parameters izena jarri, eta 
        //balioa dizebelioetan.
        //Bolumena bool izango denez VR kasuan, bolumena intera pasatu.
        //Volume==1 bada, 0 db behar dira, aldiz, Volume==0 bada, -80 db.
        audioMixer.SetFloat("Master", -80*System.Convert.ToInt32(!volume));

        
        //Textu fitxategian gorde bolumenaren balioa, hurrengo saioetan gordeta izateko.
        PlayerPrefs.SetFloat("VolumeVR", System.Convert.ToInt32(volume));
        //Aldaketak gorde.
        PlayerPrefs.Save();
    }

    public void BackgroundMusicChange(bool volume)
    {
        //Setfloat parametro bezala, Audio mixer taldeak daukan Exposed Parameters izena jarri, eta 
        //balioa dizebelioetan.
        audioMixer.SetFloat("BackgroundMusic", -80 * System.Convert.ToInt32(!volume));

        //Textu fitxategian gorde atzeko musikaren bolumenaren balioa, hurrengo saioetan gordeta izateko.
        PlayerPrefs.SetFloat("BackgroundMusicVR", System.Convert.ToInt32(volume));
        //Aldaketak gorde.
        PlayerPrefs.Save();
    }

    public void SoundEffectsChange(bool volume)
    {
        //Setfloat parametro bezala, Audio mixer taldeak daukan Exposed Parameters izena jarri, eta 
        //balioa dizebelioetan.
        audioMixer.SetFloat("SoundEffects", -80 * System.Convert.ToInt32(!volume));

        //Textu fitxategian gordesoinu efektuen bolumenaren balioa, hurrengo saioetan gordeta izateko.
        PlayerPrefs.SetFloat("SoundEffectsVR", System.Convert.ToInt32(volume));
        //Aldaketak gorde.
        PlayerPrefs.Save();
    }

}

