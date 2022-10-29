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

        public int dir;
        public float nextSpawnCD;

        public TrainStation(Coords start, Coords end, int dir, float nextSpawnCD)
        {
            this.start = start;
            this.end = end;
            this.dir = dir;
            this.nextSpawnCD = nextSpawnCD;
        }

    }

    List<TrainStation> stations;

    // Start is called before the first frame update
    void Start()
    {
        stations = new List<TrainStation>();
        AddStation(new Coords(0, 0), new Coords(0, 0), Coords.RIGHT);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var station in stations)
        {
            station.nextSpawnCD -= Time.deltaTime;
            if (station.nextSpawnCD < 0)
            {
                // Debug.Log(" New Train! ");
                station.nextSpawnCD = spawnCD;
                var newTrain = Instantiate(train, MapGrid.instance.WorldPos(station.start), Quaternion.identity);
                newTrain.Pos = station.start;
                newTrain.Dir = station.dir;
            }
        }
    }

    public void AddStation(Coords start, Coords end, int dir)
    {
        stations.Add(new TrainStation(start, end, dir, spawnCD));
    }
}
