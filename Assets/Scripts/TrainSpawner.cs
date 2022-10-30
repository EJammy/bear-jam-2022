using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainStation
{
    public Coords start;
    public Coords end;

    public int startDir, endDir;
    public float spawnDelay;
    public bool spawned, scored;
    public int lineNum;

    public TrainStation(Coords start, Coords end, int startDir, int endDir, float spawnDelay, int lineNum)
    {
        this.start = start;
        this.end = end;
        this.startDir = startDir;
        this.endDir = endDir;
        this.spawnDelay = spawnDelay;
        this.lineNum = lineNum;
        spawned = false;
        scored = false;
    }

}
public class TrainSpawner : MonoBehaviour
{
    [SerializeField]
    float spawnCD;

    [SerializeField]
    TrainController train;

    List<TrainStation> stations;
    int curLineCnt = 0;
    bool _init = false;

    // Start is called before the first frame update
    void Start()
    {
        if(!_init) Initialize();
    }

    void Initialize()
    {
        _init = true;
        stations = new List<TrainStation>();
        curLineCnt = 0;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var station in stations)
        {
            if (!station.spawned) 
            {
                station.spawnDelay -= Time.deltaTime;
                if (station.spawnDelay < 0)
                {
                    // Debug.Log(" New Train! ");
                    station.spawned = true;
                    station.spawnDelay = spawnCD;
                    var newTrain = Instantiate(train, MapGrid.instance.WorldPos(station.start), Quaternion.identity);
                    newTrain.Pos = station.start;
                    newTrain.Dir = station.startDir;
                    newTrain.parentStation = station;
                }
            }
        }
    }

    public void AddStation(Coords start, Coords end, int startDir, int endDir)
    {
        stations.Add(new TrainStation(start, end, startDir, endDir, spawnCD, curLineCnt));
        TrackPlacer.instance.PlaceTrack(start, (TrackType)((int)TrackType.STATIONT + startDir));
        MapGrid.instance.GetTile(start).stationNum = curLineCnt;
        TrackPlacer.instance.PlaceTrack(end, (TrackType)((int)TrackType.STATIONT + endDir));
        MapGrid.instance.GetTile(end).stationNum = curLineCnt;
        curLineCnt++;
    }
}
