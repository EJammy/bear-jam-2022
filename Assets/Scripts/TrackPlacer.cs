using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class TrackPlacer : MonoBehaviour
{
    // Singleton
    static public TrackPlacer instance { get; private set; }
    public Tile[] tileDict;
    public AudioClip invalid, placeTile, replaceTile;
    MapGrid grid;
    bool _init = false;

    // Start is called before the first frame update
    void Start()
    {
    }
    public void Initialize() {
        _init = true;
        instance = this;
        grid = GetComponent<MapGrid>();
        GameManager.instance.UI.Q<GroupBox>("Tile1").RegisterCallback<ClickEvent>(_ => {selected = selections[0];});
        GameManager.instance.UI.Q<GroupBox>("Tile2").RegisterCallback<ClickEvent>(_ => {selected = selections[1];});
        GameManager.instance.UI.Q<GroupBox>("Tile3").RegisterCallback<ClickEvent>(_ => {selected = selections[2];});
        GameManager.instance.UI.Q<GroupBox>("Tile4").RegisterCallback<ClickEvent>(_ => {selected = selections[3];});
        GameManager.instance.UI.Q<GroupBox>("Tile5").RegisterCallback<ClickEvent>(_ => {selected = selections[4];});
        GameManager.instance.UI.Q<GroupBox>("Tile6").RegisterCallback<ClickEvent>(_ => {selected = selections[5];});
        GameManager.instance.UI.Q<GroupBox>("Tile7").RegisterCallback<ClickEvent>(_ => {selected = selections[6];});
        /* why does this not work???
        for (int i = 0; i < 6; i++) {
            Debug.Log(string.Format("Registered box {0} to i={1}", "Tile" + (i + 1).ToString(), i));
            GroupBox box = GameManager.instance.UI.Q<GroupBox>("Tile" + (i + 1).ToString());
            box.RegisterCallback<ClickEvent>(
                ev => {
                    Debug.Log(string.Format("Clicked Tile {0}", (i+1)));
                    selected = selections[i];
                }
            );
        }
        */
    }

    TrackType selected = TrackType.NONE;
    Coords lastSelectedCell;

    readonly TrackType[] selections =
    {
        TrackType.VERTI,
        TrackType.HORI,
        TrackType.CORNERBR,
        TrackType.CORNERBL,
        TrackType.CORNERTR,
        TrackType.CORNERTL,
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

                lastSelectedCell = selectedCell;
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (grid.GetTile(selectedCell).isBlocked) {
                    GameManager.instance.PlayAudio(invalid);
                } else {
                    GameManager.instance.PlayAudio(grid.GetTile(selectedCell).trackType == TrackType.NONE ? placeTile : replaceTile);
                    PlaceTrack(selectedCell, selected);
                }
            }
        }
    }

    public void PlaceTrack(Coords pos, TrackType type) {
        grid.tiles[pos.x, pos.y].trackType = type;
        grid.tiles[pos.x, pos.y].isBlocked = (type == TrackType.OBSTACLE || TrackUtils.stationType(type) != -1);
        grid.SetTile(pos, tileDict[(int)type]);
    }
}
