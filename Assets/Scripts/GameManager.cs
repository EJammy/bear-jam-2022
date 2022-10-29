using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    MapGrid mapGrid;
    TrackPlacer trackPlacer;
    public int mapHeight, mapWidth;
    int reputation, crashes;

    // Start is called before the first frame update
    void Start()
    {
        Grid grid = FindObjectOfType<Grid>();
        if (grid == null) {
            Debug.LogError("No grid found!");
        } else {
            mapGrid = grid.GetComponent<MapGrid>();
            trackPlacer = grid.GetComponent<TrackPlacer>();
        }
        mapGrid.Initialize(mapHeight, mapWidth);
        trackPlacer.Initialize();
        reputation = 0;
        crashes = 0;
        
        // testing
        for (int i = 0; i < mapHeight; i++) {
            for (int j = 0; j < mapWidth; j++) {
                trackPlacer.PlaceTrack(new Coords(i, j), (TrackType)((i + j) % 9));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void HandleCrash() {
        crashes++;
    }
    void IncReputation() {
        reputation++;
    }
}
