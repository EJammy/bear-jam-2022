using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    AudioClip crashAudio;
    [SerializeField]
    AudioClip music;

    // Singleton
    static public GameManager instance { get; private set; }

    MapGrid mapGrid;
    TrackPlacer trackPlacer;
    TrainSpawner trainSpawner;
    ObstacleSpawner obstacleSpawner;
    public int mapHeight, mapWidth;
    int reputation, crashes;

    // Start is called before the first frame update
    void Start()
    {
        PlayAudio(music);

        instance = this;

        Grid grid = FindObjectOfType<Grid>();
        if (grid == null) {
            Debug.LogError("No grid found!");
        } else {
            mapGrid = grid.GetComponent<MapGrid>();
            trackPlacer = grid.GetComponent<TrackPlacer>();
            obstacleSpawner = grid.GetComponent<ObstacleSpawner>();
            trainSpawner = grid.GetComponent<TrainSpawner>();
        }
        mapGrid.Initialize(mapHeight, mapWidth);
        trackPlacer.Initialize();
        obstacleSpawner.Initialize();
        reputation = 0;
        crashes = 0;
        
        // testing
        // for (int i = 0; i < mapHeight; i++) {
        //     for (int j = 0; j < mapWidth; j++) {
        //         trackPlacer.PlaceTrack(new Coords(i, j), (TrackType)((i + j) % 9));
        //     }
        // }

        obstacleSpawner.SetSpawns(true);
        trainSpawner.AddStation(new Coords(0, 0), new Coords(4, 4), Coords.RIGHT, Coords.UP);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleCrash() {
        PlayAudio(crashAudio);

        crashes++;
        Debug.Log(string.Format("Crashed! Total crashes: {0}", crashes));
        if (crashes >= 3) {
            // TODO: sadness :(
        }
    }
    public void IncReputation() {
        reputation++;
        Debug.Log(string.Format("Rep increased! Total rep: {0}", reputation));
    }

    // TODO: Manage music
    public void PlayAudio(AudioClip clip)
    {
        GetComponent<AudioSource>().PlayOneShot(clip);
    }
}
