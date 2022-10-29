using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TrackPlacer : MonoBehaviour
{
    // Singleton
    static public TrackPlacer instance { get; private set; }
    public Tile[] tileDict;
    MapGrid grid;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        grid = GetComponent<MapGrid>();
    }

    TrackType selected = TrackType.NONE;
    Coords lastSelectedCell;

    readonly TrackType[] selections =
    {
        TrackType.HORI,
        TrackType.VERTI,
        TrackType.CORNERTL,
        TrackType.CORNERTR,
        TrackType.CORNERBL,
        TrackType.CORNERBR,
        TrackType.CROSS,
    };

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < selections.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                selected = selections[i];
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape))
        {
            selected = TrackType.NONE;
        }

        // Might need to check if lastSelectedCell is set in the future
        if (grid.GetTile(lastSelectedCell).trackType == TrackType.NONE)
        {
            grid.SetTile(lastSelectedCell, tileDict[(int)TrackType.NONE]);
        }

        var selectedCell = grid.GridCoords(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        if (grid.InBound(selectedCell))
        {
            if (grid.GetTile(selectedCell).trackType == TrackType.NONE)
            {
                var preview = Instantiate(tileDict[(int)selected]);

                // set alpha
                var color = preview.color;
                color.a = 0.5f;
                preview.color = color;

                grid.SetTile(selectedCell, preview);

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    PlaceTrack(selectedCell, selected);
                }
                lastSelectedCell = selectedCell;
            }

        }
    }

    public void Initialize() {
        grid = GetComponent<MapGrid>();
    }

    public void PlaceTrack(Coords pos, TrackType type) {
        grid.tiles[pos.x, pos.y].trackType = type;
        grid.tiles[pos.x, pos.y].isBlocked = (type == TrackType.OBSTACLE || (int)type >= (int)TrackType.STATIONT);
        grid.SetTile(pos, tileDict[(int)type]);
    }
}
