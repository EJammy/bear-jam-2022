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
    public Tile[] fireballAnim;
    public AudioClip invalid, placeTile, replaceTile, selTile;
    MapGrid grid;
    GroupBox[] tileBoxes;
    int burntIndex = 0, burntDirection = 1;
    const float burntFrameTime = 0.2f;
    float curBurntTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
    }
    public void Initialize() {
        instance = this;
        grid = GetComponent<MapGrid>();
        tileBoxes = new GroupBox[7];
        for(int i = 0; i < 7; i++) {
            tileBoxes[i] = GameManager.instance.UI.Q<GroupBox>("Tile" + (i + 1).ToString());
            tileBoxes[i].style.borderBottomWidth = new StyleFloat(2.0f);
            tileBoxes[i].style.borderTopWidth = new StyleFloat(2.0f);
            tileBoxes[i].style.borderLeftWidth = new StyleFloat(2.0f);
            tileBoxes[i].style.borderRightWidth = new StyleFloat(2.0f);
        }
        GameManager.instance.UI.Q<GroupBox>("Tile1").RegisterCallback<ClickEvent>(_ => {selected = selections[0]; GameManager.instance.PlaySFX(selTile);});
        GameManager.instance.UI.Q<GroupBox>("Tile2").RegisterCallback<ClickEvent>(_ => {selected = selections[1]; GameManager.instance.PlaySFX(selTile);});
        GameManager.instance.UI.Q<GroupBox>("Tile3").RegisterCallback<ClickEvent>(_ => {selected = selections[2]; GameManager.instance.PlaySFX(selTile);});
        GameManager.instance.UI.Q<GroupBox>("Tile4").RegisterCallback<ClickEvent>(_ => {selected = selections[3]; GameManager.instance.PlaySFX(selTile);});
        GameManager.instance.UI.Q<GroupBox>("Tile5").RegisterCallback<ClickEvent>(_ => {selected = selections[4]; GameManager.instance.PlaySFX(selTile);});
        GameManager.instance.UI.Q<GroupBox>("Tile6").RegisterCallback<ClickEvent>(_ => {selected = selections[5]; GameManager.instance.PlaySFX(selTile);});
        GameManager.instance.UI.Q<GroupBox>("Tile7").RegisterCallback<ClickEvent>(_ => {selected = selections[6]; GameManager.instance.PlaySFX(selTile);});
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
        // input stuff
        bool didSelect = false;
        for (int i = 0; i < selections.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                selected = selections[i];
                didSelect = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            selected = TrackType.CORNERBR;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            selected = TrackType.CORNERTR;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            selected = TrackType.CORNERBL;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            selected = TrackType.CORNERTL;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            selected = TrackType.CROSS;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape))
        {
            selected = TrackType.NONE;
            didSelect = false;
        }

        if (didSelect) GameManager.instance.PlaySFX(selTile);

        for (int i = 0; i < 7; i++) {
            if (selections[i] == selected) {
                tileBoxes[i].style.borderBottomColor = new StyleColor(Color.red);
                tileBoxes[i].style.borderTopColor = new StyleColor(Color.red);
                tileBoxes[i].style.borderLeftColor = new StyleColor(Color.red);
                tileBoxes[i].style.borderRightColor = new StyleColor(Color.red);
            } else {
                tileBoxes[i].style.borderBottomColor = new StyleColor(Color.clear);
                tileBoxes[i].style.borderTopColor = new StyleColor(Color.clear);
                tileBoxes[i].style.borderLeftColor = new StyleColor(Color.clear);
                tileBoxes[i].style.borderRightColor = new StyleColor(Color.clear);
            }
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

            if (Input.GetKeyDown(KeyCode.Mouse0) && selected != TrackType.NONE)
            {
                if (grid.GetTile(selectedCell).isBlocked) {
                    GameManager.instance.PlaySFX(invalid);
                } else {
                    GameManager.instance.PlaySFX(grid.GetTile(selectedCell).trackType == TrackType.NONE ? placeTile : replaceTile);
                    PlaceTrack(selectedCell, selected);
                }
            }
            if (Input.GetKey(KeyCode.Mouse0)
                && !grid.GetTile(selectedCell).isBlocked
                && grid.GetTile(selectedCell).trackType != selected )
            {
                GameManager.instance.PlayAudio(grid.GetTile(selectedCell).trackType == TrackType.NONE ? placeTile : replaceTile);
                PlaceTrack(selectedCell, selected);
            }
            if (Input.GetKey(KeyCode.Mouse1)
                && !grid.GetTile(selectedCell).isBlocked
                && grid.GetTile(selectedCell).trackType != TrackType.NONE )
            {
                GameManager.instance.PlayAudio(replaceTile);
                PlaceTrack(selectedCell, TrackType.NONE);
            }
        }

        // update fireball tiles
        curBurntTime += Time.deltaTime;
        if (curBurntTime >= burntFrameTime) {
            curBurntTime = 0f;
            burntIndex += burntDirection;
            if (burntIndex == 0 || burntIndex == fireballAnim.Length - 1) burntDirection = -burntDirection;
            for (int i = 0; i < grid.width; i++) {
                for (int j = 0; j < grid.height; j++) {
                    if (grid.tiles[i, j].trackType == TrackType.OBSTACLE) {
                        grid.SetTile(new Coords(i, j), fireballAnim[burntIndex]);
                    }
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
