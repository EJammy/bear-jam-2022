using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGrid : MonoBehaviour
{
    Tilemap tilemap;
    Grid grid;
    Tile[,] tiles;
    int height, width;

    public class Tile
    {
        public TileBase tile = null; // sprite to display; CHANGE ME to default
        public TrackType trackType = TrackType.NONE; // has tracks?
        public bool isBlocked = false; // blocked by obstacle?
    }

    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        grid = GetComponent<Grid>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void Initialize(int height, int width)
    {
        tiles = new Tile[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                tilemap.SetTile(new Vector3Int(i, j), tiles[i, j].tile);
            }
        }
    }

    public Coords GridCoords(Vector3 pos)
    {
        Vector3Int v = grid.LocalToCell(pos);
        return new Coords(v.x, v.y);
    }
}