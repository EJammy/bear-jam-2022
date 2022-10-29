using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGrid : MonoBehaviour
{
    public Tile defaultTile;
    Tilemap tilemap;
    Grid grid;
    public MapTile[,] tiles;
    [System.NonSerialized]
    public int height, width;

    public class MapTile
    {
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

    public void Initialize(int height, int width)
    {
        tilemap = GetComponentInChildren<Tilemap>();
        grid = GetComponent<Grid>();
        this.height = height;
        this.width = width;
        tiles = new MapTile[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                tiles[i, j] = new MapTile();
                tilemap.SetTile(new Vector3Int(i, j), defaultTile);
            }
        }
    }

    public void SetTile(Coords pos, Tile tile)
    {
        if (tile == null) tilemap.SetTile(new Vector3Int(pos.x, pos.y), defaultTile);
        tilemap.SetTile(new Vector3Int(pos.x, pos.y), tile);
    }

    public Coords GridCoords(Vector3 pos)
    {
        Vector3Int v = grid.LocalToCell(pos);
        return new Coords(v.x, v.y);
    }
}