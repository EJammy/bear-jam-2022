using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TrainSpawner : MonoBehaviour
{
    [SerializeField]
    float spawnCD;

    [SerializeField]
    TrainController train;


    public class TrainStation
    {
        public Coords start;
        public Coords end;

        public int startDir, endDir;
        public float spawnDelay;
        public bool spawned;

        public TrainStation(Coords start, Coords end, int startDir, int endDir, float spawnDelay)
        {
            this.start = start;
            this.end = end;
            this.startDir = startDir;
            this.endDir = endDir;
            this.spawnDelay = spawnDelay;
            spawned = false;
        }

    }

    List<TrainStation> stations;

    // Start is called before the first frame update
    void Start()
    {
        stations = new List<TrainStation>();
        AddStation(new Coords(0, 0), new Coords(4, 4), Coords.RIGHT, Coords.DOWN);
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
        stations.Add(new TrainStation(start, end, startDir, endDir, spawnCD));
    }
}
