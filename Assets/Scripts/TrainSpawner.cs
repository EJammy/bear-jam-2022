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
        MapGrid.instance.GetTile(start.MoveDir(startDir)).isTargetted = true; // prevent targetting by fireballs
        TrackPlacer.instance.PlaceTrack(end, (TrackType)((int)TrackType.STATIONT + endDir));
        MapGrid.instance.GetTile(end).stationNum = curLineCnt;
        MapGrid.instance.GetTile(end.MoveDir(endDir)).isTargetted = true;
        curLineCnt++;
    }

    public void SpawnStation()
    {
        bool ok = true;
        // spawn start
        int sx, sy, sside;
        Coords sc;
        do {
            ok = true;
            sside = UnityEngine.Random.Range(0, 4);
            int edgeDist = UnityEngine.Random.Range(0, 3);
            switch (sside) {
                case Coords.UP:
                    sy = MapGrid.instance.height - 1 - edgeDist;
                    sx = UnityEngine.Random.Range(0, MapGrid.instance.width);
                    break;
                case Coords.DOWN:
                    sy = edgeDist;
                    sx = UnityEngine.Random.Range(0, MapGrid.instance.width);
                    break;
                case Coords.LEFT:
                    sx = edgeDist;
                    sy = UnityEngine.Random.Range(0, MapGrid.instance.height);
                    break;
                case Coords.RIGHT:
                default:
                    sx = MapGrid.instance.width - 1 - edgeDist;
                    sy = UnityEngine.Random.Range(0, MapGrid.instance.height);
                    break;
            }
            sc = new Coords(sx, sy);
            if (MapGrid.instance.GetTile(sc).isTargetted || MapGrid.instance.GetTile(sc).isBlocked) ok = false;
            List<int> sDirs = new();
            for (int i = 0; i < 4; i++) {
                if (MapGrid.instance.InBound(sc.MoveDir(i)) && 
                    !MapGrid.instance.GetTile(sc.MoveDir(i)).isTargetted &&
                    !MapGrid.instance.GetTile(sc.MoveDir(i)).isBlocked) {
                        sDirs.Add(i);
                    }
            }
            if (sDirs.Count == 0) ok = false;
            else sside = sDirs[UnityEngine.Random.Range(0, sDirs.Count)];
        } while (!ok);
        MapGrid.instance.GetTile(sc).isBlocked = true;
        // spawn end
        int ex, ey, eside;
        Coords ec;
        do {
            ok = true;
            eside = Coords.OppDir(sside); // always spawn on opposite sides; change this later?
            int edgeDist = UnityEngine.Random.Range(0, 3);
            switch (eside) {
                case Coords.UP:
                    ey = MapGrid.instance.height - 1 - edgeDist;
                    ex = UnityEngine.Random.Range(0, MapGrid.instance.width);
                    break;
                case Coords.DOWN:
                    ey = edgeDist;
                    ex = UnityEngine.Random.Range(0, MapGrid.instance.width);
                    break;
                case Coords.LEFT:
                    ex = edgeDist;
                    ey = UnityEngine.Random.Range(0, MapGrid.instance.height);
                    break;
                case Coords.RIGHT:
                default:
                    ex = MapGrid.instance.width - 1 - edgeDist;
                    ey = UnityEngine.Random.Range(0, MapGrid.instance.height);
                    break;
            }
            ec = new Coords(ex, ey);
            if (MapGrid.instance.GetTile(ec).isTargetted || MapGrid.instance.GetTile(ec).isBlocked) ok = false;
            List<int> eDirs = new();
            for (int i = 0; i < 4; i++) {
                if (MapGrid.instance.InBound(ec.MoveDir(i)) && 
                    !MapGrid.instance.GetTile(ec.MoveDir(i)).isTargetted &&
                    !MapGrid.instance.GetTile(ec.MoveDir(i)).isBlocked) {
                        eDirs.Add(i);
                    }
            }
            if (eDirs.Count == 0) ok = false;
            else eside = eDirs[UnityEngine.Random.Range(0, eDirs.Count)];
        } while (!ok);
        AddStation(sc, ec, sside, eside);
    }
}
