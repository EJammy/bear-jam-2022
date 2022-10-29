using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO Animate train movement
public class TrainController : MonoBehaviour
{
    int prevDir;
    int nextDir;

    public Coords Pos { get => _pos;
        set
        {
            _pos = value;
            transform.position = MapGrid.instance.WorldPos(value);
        }
    }

    public int Dir {
        get { return nextDir; }
        set { nextDir = value; }
    }

    Coords _pos;

    float moveCD;

    [SerializeField]
    readonly float timeBetweenMove = 1.2f;

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
        Pos = Pos.MoveDir(nextDir);

        prevDir = Coords.OppDir(nextDir);

        // TODO: set next dir
        moveCD = timeBetweenMove;
        int opening1 = -1;
        int opening2 = -1;
        switch(MapGrid.instance.GetTile(Pos).trackType)
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
                return;
        }


        if (prevDir == opening1)
        {
            nextDir = opening2;
        }
        else if (prevDir == opening2)
        {
            nextDir = opening1;
        }
        else
        {
            // Destroy(gameObject);
            GameManager.instance.HandleCrash();
        }
    }
}
