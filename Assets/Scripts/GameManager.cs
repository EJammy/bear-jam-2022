using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    AudioClip crashAudio;
    AudioSource audioSrc;

    // Singleton
    static public GameManager instance { get; private set; }

    MapGrid mapGrid;
    TrackPlacer trackPlacer;
    TrainSpawner trainSpawner;
    ObstacleSpawner obstacleSpawner;
    public VisualElement UI;
    public int mapHeight, mapWidth;
    int reputation, crashes;
    int curTrainArrivals, curStage;
    int[] stageTrainGoals = {0, 2, 4, 10, 15};
    int winRep = 5, loseCrashes = 3;
    VisualElement[] starsUI, strikesUI;
    public Sprite emptyStar, filledStar, emptyStrike, filledStrike;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        audioSrc = GetComponent<AudioSource>();
        audioSrc.playOnAwake = true;

        Grid grid = FindObjectOfType<Grid>();
        if (grid == null) {
            Debug.LogError("No grid found!");
        } else {
            mapGrid = grid.GetComponent<MapGrid>();
            trackPlacer = grid.GetComponent<TrackPlacer>();
            obstacleSpawner = grid.GetComponent<ObstacleSpawner>();
            trainSpawner = grid.GetComponent<TrainSpawner>();
        }

        UI = FindObjectOfType<UIDocument>().GetComponent<UIDocument>().rootVisualElement;
        starsUI = new VisualElement[winRep];
        for (int i = 0; i < winRep; i++) {
            starsUI[i] = UI.Q<VisualElement>("star" + (i+1).ToString());
        }
        strikesUI = new VisualElement[loseCrashes];
        for (int i = 0; i < loseCrashes; i++) {
            strikesUI[i] = UI.Q<VisualElement>("strike" + (i+1).ToString());
        }

        mapGrid.Initialize(mapHeight, mapWidth);
        trackPlacer.Initialize();
        obstacleSpawner.Initialize();
        reputation = 0;
        crashes = 0;
        curTrainArrivals = 0;
        curStage = 1;

        obstacleSpawner.SetSpawns(false);
        trainSpawner.AddStation(new Coords(0, 4), new Coords(14, 4), Coords.RIGHT, Coords.LEFT);
        trainSpawner.stations[0].spawnDelay = 5.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleCrash() {
        PlayAudio(crashAudio);
        strikesUI[crashes].style.backgroundImage = new StyleBackground(filledStrike);
        crashes++;
        Debug.Log(string.Format("Crashed! Total crashes: {0}", crashes));
        if (crashes >= 3) {
            // sadness :( stop spawning trains, and stop moving trains
            trainSpawner.enabled = false;
            foreach (TrainController train in FindObjectsOfType<TrainController>()) {
                train.enabled = false;
            }
            Debug.Log("GAME OVER");
        }
    }
    public void IncReputation() {
        starsUI[reputation].style.backgroundImage = new StyleBackground(filledStar);
        reputation++;
        Debug.Log(string.Format("Rep increased! Total rep: {0}", reputation));
        if (reputation == 1) {
            // start the attack!
            obstacleSpawner.SetSpawns(true);
        }
        if (reputation >= winRep) {
            // win! stop spawning obstacles, trains, and stop moving trains
            obstacleSpawner.SetSpawns(false);
            trainSpawner.enabled = false;
            foreach (TrainController train in FindObjectsOfType<TrainController>()) {
                train.enabled = false;
            }
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

    // TODO: Manage music
    public void PlayAudio(AudioClip clip)
    {
        audioSrc.PlayOneShot(clip);
    }
}
