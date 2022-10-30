using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject targetObj;
    public GameObject fireBall;
    // public GameObject obstObj;
    public float spawnTime = 20.0f;
    public float warnDur = 5.0f;
    public float aliveDur = 20.0f;
    public AudioClip fireSpawn, fireFall, fireHit, fireBurn;
    TrackPlacer trackPlacer;
    MapGrid mapGrid;
    bool isSpawning;
    float curTime = 0.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        trackPlacer = GetComponent<TrackPlacer>();
        mapGrid = GetComponent<MapGrid>();
    }

    public void Initialize()
    {
        trackPlacer = GetComponent<TrackPlacer>();
        mapGrid = GetComponent<MapGrid>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isSpawning) {
            curTime += Time.deltaTime;
            if (curTime >= spawnTime) {
                curTime = 0f;
                List<Coords> spawnLoc = RandomizeLoc(3);
                foreach (Coords c in spawnLoc) {
                    StartCoroutine(SpawnSingle(c));
                }
            }
        }
    }

    public void SetSpawns(bool enable) {
        isSpawning = enable;
        curTime = spawnTime;
    }

    private List<Coords> RandomizeLoc(int size) {
        List<Coords> ret = new();
        for (int i = 0; i < size; i++) {
            int rx, ry;
            do {
                rx = UnityEngine.Random.Range(0, mapGrid.width);
                ry = UnityEngine.Random.Range(0, mapGrid.height);
            } while (mapGrid.tiles[rx, ry].isTargetted || mapGrid.tiles[rx, ry].isBlocked);
            ret.Add(new Coords(rx, ry));
        }
        return ret;
    }

    private IEnumerator SpawnSingle(Coords c) {
        mapGrid.tiles[c.x, c.y].isTargetted = true;
        GameManager.instance.PlayAudio(fireSpawn);
        // spawn target
        GameObject curTarget = Instantiate(targetObj, mapGrid.WorldPos(c), Quaternion.identity);

        int frames = 20;
        float spawnLoc = 4;
        var fStart = mapGrid.WorldPos(c) + Vector2.up * spawnLoc;
        var fallingObj = Instantiate(fireBall, fStart, Quaternion.identity);
        for (int i = 0; i < frames; i++)
        {
            fallingObj.transform.position = Vector2.Lerp(fStart, mapGrid.WorldPos(c), ((float) i) / 20);
            yield return new WaitForSeconds(warnDur / frames);
        }
        Destroy(curTarget);
        Destroy(fallingObj);

        // spawn obstacle & destroy tracks
        GameManager.instance.PlayAudio(fireHit);
        trackPlacer.PlaceTrack(c, TrackType.OBSTACLE);
        yield return new WaitForSeconds(0.5f);
        GameManager.instance.PlayAudio(fireBurn);
        yield return new WaitForSeconds(aliveDur - 0.5f);
        trackPlacer.PlaceTrack(c, TrackType.NONE);
        mapGrid.SetGroundTile(c, true);
        mapGrid.tiles[c.x, c.y].isTargetted = false;
    }
}
