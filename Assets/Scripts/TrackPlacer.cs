using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TrackPlacer : MonoBehaviour
{
    public Tile[] tileDict;
    MapGrid grid;

    // Start is called before the first frame update
    void Start()
    {
        grid = GetComponent<MapGrid>();
    }

    // Update is called once per frame
    void Update()
    {
        PlaceTrack(new Coords(3, 3), TrackType.HORI);
    }

    public void PlaceTrack(Coords pos, TrackType type) {
        grid.tiles[pos.x, pos.y].trackType = type;
        grid.tiles[pos.x, pos.y].isBlocked = (type == TrackType.OBSTACLE);
        grid.SetTile(pos, tileDict[(int)type]);
    }
}
