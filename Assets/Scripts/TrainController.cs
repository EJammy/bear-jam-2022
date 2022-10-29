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
            Pos = Pos.MoveDir(nextDir);

            prevDir = Coords.OppDir(nextDir);

            // TODO: set next dir
            moveCD = timeBetweenMove;
        }
    }
}
