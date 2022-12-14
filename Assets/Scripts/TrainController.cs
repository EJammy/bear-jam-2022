using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO Animate train movement
public class TrainController : MonoBehaviour
{
    int prevDir;
    int nextDir;
    public TrainStation parentStation;
    public Sprite[] trainSprite;
    public Sprite[] dieAnim;
    const float dieAnimFrameTime = 1.5f;

    public Coords Pos { get => _pos;
        set
        {
            _pos = value;
            transform.position = MapGrid.instance.WorldPos(value);
        }
    }

    public int Dir {
        get { return nextDir; }
        set
        {
            nextDir = value;
            GetComponent<SpriteRenderer>().sprite = trainSprite[nextDir];
        }
    }

    Coords _pos;

    float moveCD;

    [SerializeField]
    float timeBetweenMove = 1.5f;
    float timeAtStation = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        moveCD = timeBetweenMove;
    }

    // Update is called once per frame
    void Update()
    {
        moveCD -= Time.deltaTime;

        if (moveCD < 0)
        {
            Move();
        }
    }

    void Move()
    {
        // print(": " + this + " moved to " + nextDir);
        Coords nextPos = Pos.MoveDir(nextDir);

        prevDir = Coords.OppDir(nextDir);
        nextDir = -1;

        moveCD = timeBetweenMove;
        bool normalTrack = true;
        int opening1 = -1;
        int opening2 = -1;
        switch(MapGrid.instance.GetTile(nextPos).trackType)
        {
            case TrackType.HORI:
                opening1 = Coords.LEFT;
                opening2 = Coords.RIGHT;
                break;
            case TrackType.VERTI:
                opening1 = Coords.UP;
                opening2 = Coords.DOWN;
                break;
            case TrackType.CORNERTL:
                opening1 = Coords.LEFT;
                opening2 = Coords.UP;
                break;
            case TrackType.CORNERTR:
                opening1 = Coords.RIGHT;
                opening2 = Coords.UP;
                break;
            case TrackType.CORNERBL:
                opening1 = Coords.LEFT;
                opening2 = Coords.DOWN;
                break;
            case TrackType.CORNERBR:
                opening1 = Coords.DOWN;
                opening2 = Coords.RIGHT;
                break;
            case TrackType.CROSS:
                nextDir = Coords.OppDir(prevDir);
                normalTrack = false;
                break;
            case TrackType.STATIONT:
            case TrackType.STATIONB:
            case TrackType.STATIONL:
            case TrackType.STATIONR:
                normalTrack = false;
                break;
        }

        if (normalTrack) {
            if (prevDir == opening1)
            {
                Dir = opening2;
            }
            else if (prevDir == opening2)
            {
                Dir = opening1;
            }
        }

        int stationType = TrackUtils.stationType(MapGrid.instance.GetTile(nextPos).trackType);
        if (stationType != -1) {
            if (prevDir == stationType) {
                // reached station from correct direction
                if (parentStation.lineNum != MapGrid.instance.GetTile(nextPos).stationNum) {
                    // wrong station - may want special handling for animation purposes
                    StartCoroutine(DieAnim());
                    Destroy(gameObject);
                    GameManager.instance.HandleCrash();
                    parentStation.spawned = false;
                    return;
                } else {
                    // correct station! go back and score reputation if first time
                    Dir = prevDir; 
                    if (!parentStation.scored) GameManager.instance.IncReputation();
                    parentStation.scored = true;
                    GameManager.instance.TrainArrived();
                    moveCD = timeAtStation;
                }
            }
        }

        if (Dir == -1)
        {
            StartCoroutine(DieAnim());
            GameManager.instance.HandleCrash();
            parentStation.spawned = false;
        }
        else
        {
            Pos = nextPos;
        }
    }

    IEnumerator DieAnim() {
        enabled = false;
        foreach (Sprite sp in dieAnim) {
            GetComponent<SpriteRenderer>().sprite = sp;
            yield return new WaitForSeconds(dieAnimFrameTime);
        }
        Destroy(gameObject);
    }
}
