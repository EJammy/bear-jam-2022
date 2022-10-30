using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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
        for (int i = 0; i < mapHeight; i++) {
            for (int j = 0; j < mapWidth; j++) {
                trackPlacer.PlaceTrack(new Coords(i, j), (TrackType)((i + j) % 9));
            }
        }
        trackPlacer.PlaceTrack(new Coords(1, 0), TrackType.HORI);
        trackPlacer.PlaceTrack(new Coords(2, 0), TrackType.CORNERTL);
        trackPlacer.PlaceTrack(new Coords(2, 1), TrackType.VERTI);
        trackPlacer.PlaceTrack(new Coords(2, 2), TrackType.CORNERBR);
        trackPlacer.PlaceTrack(new Coords(3, 2), TrackType.HORI);
        trackPlacer.PlaceTrack(new Coords(4, 2), TrackType.CROSS);
        trackPlacer.PlaceTrack(new Coords(5, 2), TrackType.CORNERTL);
        trackPlacer.PlaceTrack(new Coords(5, 3), TrackType.VERTI);
        trackPlacer.PlaceTrack(new Coords(5, 4), TrackType.VERTI);
        trackPlacer.PlaceTrack(new Coords(5, 5), TrackType.CORNERBL);
        trackPlacer.PlaceTrack(new Coords(4, 5), TrackType.CORNERBR);

        obstacleSpawner.SetSpawns(true);
        trainSpawner.AddStation(new Coords(0, 0), new Coords(4, 4), Coords.RIGHT, Coords.UP);
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
    public void IncReputation() {
        reputation++;
        Debug.Log(string.Format("Rep increased! Total rep: {0}", reputation));
    }
}
