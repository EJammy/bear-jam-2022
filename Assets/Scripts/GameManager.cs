using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton
    static public GameManager instance { get; private set; }

    MapGrid mapGrid;
    TrackPlacer trackPlacer;
    ObstacleSpawner obstacleSpawner;
    public int mapHeight, mapWidth;
    int reputation, crashes;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        Grid grid = FindObjectOfType<Grid>();
        if (grid == null) {
            Debug.LogError("No grid found!");
        } else {
            mapGrid = grid.GetComponent<MapGrid>();
            trackPlacer = grid.GetComponent<TrackPlacer>();
            obstacleSpawner = grid.GetComponent<ObstacleSpawner>();
        }
        mapGrid.Initialize(mapHeight, mapWidth);
        trackPlacer.Initialize();
        reputation = 0;
        crashes = 0;
        
        // testing
        // for (int i = 0; i < mapHeight; i++) {
        //     for (int j = 0; j < mapWidth; j++) {
        //         trackPlacer.PlaceTrack(new Coords(i, j), (TrackType)((i + j) % 9));
        //     }
        // }
        obstacleSpawner.SetSpawns(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleCrash() {
        crashes++;
        Debug.Log(string.Format("Crashed! Total crashes: {0}", crashes));
        if (crashes >= 3) {
            // TODO: sadness :(
        }
    }
    void IncReputation() {
        Debug.Log(string.Format("Rep increased! Total rep: {0}", reputation));
        reputation++;
    }
}
