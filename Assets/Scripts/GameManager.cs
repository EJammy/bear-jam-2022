using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    AudioClip crashAudio;
    [SerializeField]
    AudioClip repAudio;
    [SerializeField]
    AudioClip titleMusic, introMusic, atkMusic, victorySFX, loseSFX;

    // Do we need this when we have crash audio?
    // [SerializeField]
    // AudioClip strikeAudio;

    AudioSource musicSrc, sfxSrc;

    // Singleton
    static public GameManager instance { get; private set; }

    MapGrid mapGrid;
    TrackPlacer trackPlacer;
    TrainSpawner trainSpawner;
    ObstacleSpawner obstacleSpawner;
    public VisualElement UI;
    public UIDocument gameHUD, winUI, loseUI, titleUI, creditsUI;
    public int mapHeight, mapWidth;
    int reputation, crashes;
    int curTrainArrivals, curStage;
    int[] stageTrainGoals = {0, 2, 4, 10, 15};
    int winRep = 4, loseCrashes = 5;
    VisualElement[] starsUI, strikesUI;
    public Sprite emptyStar, filledStar, emptyStrike, filledStrike;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        AudioSource[] srcs = GetComponents<AudioSource>();
        // wow such hack
        if (srcs[0].loop) {musicSrc = srcs[0]; sfxSrc = srcs[1];}
        else {musicSrc = srcs[1]; sfxSrc = srcs[0];}
        musicSrc.clip = titleMusic;
        musicSrc.Play();

        UI = gameHUD.rootVisualElement;
        starsUI = new VisualElement[winRep];
        for (int i = 0; i < winRep; i++) {
            starsUI[i] = UI.Q<VisualElement>("star" + (i+1).ToString());
        }
        strikesUI = new VisualElement[loseCrashes];
        for (int i = 0; i < loseCrashes; i++) {
            strikesUI[i] = UI.Q<VisualElement>("strike" + (i+1).ToString());
        }

        Grid grid = FindObjectOfType<Grid>();
        if (grid == null) {
            Debug.LogError("No grid found!");
        } else {
            mapGrid = grid.GetComponent<MapGrid>();
            trackPlacer = grid.GetComponent<TrackPlacer>();
            
            obstacleSpawner = grid.GetComponent<ObstacleSpawner>();
            trainSpawner = grid.GetComponent<TrainSpawner>();

            trackPlacer.enabled = false;
        }

        winUI.rootVisualElement.Q<Button>("playagain-button").SetEnabled(false);
        loseUI.rootVisualElement.Q<Button>("playagain-button").SetEnabled(false);

        winUI.rootVisualElement.Q<Button>("playagain-button").clicked += RestartGame;
        loseUI.rootVisualElement.Q<Button>("playagain-button").clicked += RestartGame;

        titleUI.rootVisualElement.Q<Button>("play-button").clicked += StartGame;
        titleUI.rootVisualElement.Q<Button>("credits-button").clicked += ShowCredits;

        creditsUI.rootVisualElement.Q<Button>("back-button").clicked += BackToMain;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleCrash() {
        PlaySFX(crashAudio);
        strikesUI[crashes].style.backgroundImage = new StyleBackground(filledStrike);
        crashes++;
        Debug.Log(string.Format("Crashed! Total crashes: {0}", crashes));
        if (crashes >= loseCrashes) {
            // sadness :( stop spawning trains, and stop moving trains
            trainSpawner.enabled = false;
            foreach (TrainController train in FindObjectsOfType<TrainController>()) {
                train.enabled = false;
            }
            loseUI.rootVisualElement.Q<VisualElement>("root").visible = true;
            loseUI.rootVisualElement.Q<Button>("playagain-button").SetEnabled(true);
            trackPlacer.enabled = false;
            musicSrc.Stop();
            PlaySFX(loseSFX);
            Debug.Log("GAME OVER");
        }
    }
    public void IncReputation() {
        starsUI[reputation].style.backgroundImage = new StyleBackground(filledStar);
        reputation++;
        PlaySFX(repAudio);
        Debug.Log(string.Format("Rep increased! Total rep: {0}", reputation));

        if (reputation == 1) {
            // start the attack!
            obstacleSpawner.SetSpawns(true);
            musicSrc.Stop();
            musicSrc.clip = atkMusic;
            musicSrc.Play();
        }

        if (reputation >= winRep) {
            // win! stop spawning obstacles, trains, and stop moving trains
            obstacleSpawner.SetSpawns(false);
            trainSpawner.enabled = false;
            foreach (TrainController train in FindObjectsOfType<TrainController>()) {
                train.enabled = false;
            }
            winUI.rootVisualElement.Q<VisualElement>("root").visible = true;
            winUI.rootVisualElement.Q<Button>("playagain-button").SetEnabled(true);
            trackPlacer.enabled = false;
            musicSrc.Stop();
            PlaySFX(victorySFX);
            Debug.Log("WIN");
        }
    }
    public void TrainArrived() {
        if (curStage < winRep) {
            curTrainArrivals++;
            if (curTrainArrivals >= stageTrainGoals[curStage]) {
                curTrainArrivals = 0;
                curStage++;
                trainSpawner.SpawnStation();
            }
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSrc.PlayOneShot(clip);
    }

    public void PlaySFX(AudioClip clip, float vol)
    {
        sfxSrc.PlayOneShot(clip, vol);
    }

    private void RestartGame() {
        SceneManager.LoadScene("GameScene");
    }

    private void StartGame() {
        titleUI.rootVisualElement.Q<VisualElement>("root").visible = false;

        trackPlacer.enabled = true;
        mapGrid.Initialize(mapHeight, mapWidth);
        trackPlacer.Initialize();
        obstacleSpawner.Initialize();
        reputation = 0;
        crashes = 0;
        curTrainArrivals = 0;
        curStage = 1;
        musicSrc.Stop();
        musicSrc.clip = introMusic;
        musicSrc.Play();

        obstacleSpawner.SetSpawns(false);
        trainSpawner.AddStation(new Coords(0, 4), new Coords(11, 4), Coords.RIGHT, Coords.LEFT);
        trainSpawner.stations[0].spawnDelay = 5.0f;
    }

    private void ShowCredits() {
        titleUI.rootVisualElement.Q<VisualElement>("root").visible = false;
        creditsUI.rootVisualElement.Q<VisualElement>("root").visible = true;
    }

    private void BackToMain() {
        titleUI.rootVisualElement.Q<VisualElement>("root").visible = true;
        creditsUI.rootVisualElement.Q<VisualElement>("root").visible = false;
        winUI.rootVisualElement.Q<VisualElement>("root").visible = false;
        loseUI.rootVisualElement.Q<VisualElement>("root").visible = false;
    }
}
