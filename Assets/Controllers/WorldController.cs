using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WorldController : MonoBehaviour
{

    public static WorldController Instance
    {
        get;
        protected set;
    }

    // Sprites array for our tiles, assignable in the inspector.
    [Header("Sprites")]
    public Sprite[] tileSprites;

    public World World
    {
        get;
        protected set;
    }
    // Use this for initialization
    void Start()
    {
        if (Instance != null)
            Debug.LogError("World, Start - There should only be one world.");
        Instance = this;

        World = new World();

        // Create a GameObject for each of our tiles, so they show visually
        for (int x = 0; x < World.Width; x++)
            for (int y = 0; y < World.Height; y++)
            {
                Tile tileData = World.GetTileAt(x, y);
                GameObject tileGo = new GameObject();
                tileGo.name = "Tile_" + tileData.X + "_" + tileData.Y;
                tileGo.transform.SetParent(GameObject.Find("Tiles").transform, true);
                tileGo.transform.position = new Vector2(tileData.X, tileData.Y);
                tileData.RegisterTileTypeChangedCallback((tile) => { OnTileTypeChanged(tile, tileGo); });
                SpriteRenderer tileSr = tileGo.AddComponent<SpriteRenderer>();
            }
        World.RandomizeTiles();
    }

    void Update()
    {

    }

    private void OnTileTypeChanged(Tile tileData, GameObject tileGo)
    {
        SpriteRenderer tileSr = tileGo.GetComponent<SpriteRenderer>();
        if (tileSr == null)
            Debug.LogError("OnTileTypeChanged - Tile GameObject does not have a SpriteRenderer.");
        if (tileData.Type == Tile.TileType.Floor)
            tileSr.sprite = tileSprites[0];
        else if (tileData.Type == Tile.TileType.Gras)
            tileSr.sprite = tileSprites[1];
        else
            Debug.LogError("OnTileTypeChanged - Tile sprite out of range.");
    }

    public Tile GetTileAtCoordinates(Vector3 coord)
    {
        int x = Mathf.RoundToInt(coord.x);
        int y = Mathf.RoundToInt(coord.y);
        return World.GetTileAt(x, y);
    }

    public Tile[] GetTilesAtCoordinates(Vector3 startCoordinates, Vector3 endCoordinates)
    {
        var xAxis = new SortedAxisPoints(Mathf.RoundToInt(startCoordinates.x), Mathf.RoundToInt(endCoordinates.x));
        var yAxis = new SortedAxisPoints(Mathf.RoundToInt(startCoordinates.y), Mathf.RoundToInt(endCoordinates.y));

        var tileCount = xAxis.Length * yAxis.Length;
        var tiles = new Tile[tileCount];

        int i = 0;
        for (int x = xAxis.Start; x <= xAxis.End; x++)
        {
            for (int y = yAxis.Start; y <= yAxis.End; y++)
            {
                tiles[i] = World.GetTileAt(x, y);
                i++;
            }
        }
        return tiles;
    }
    // Takes two coordinates from a single axis and orders them 
    public Vector2Int GetStartAndEndCoordinates(int start, int end)
    {
        if (start <= end)
            return new Vector2Int(start, end);
        else
            return new Vector2Int(end, start);
    }

    public class SortedAxisPoints
    {
        public int Start
        {
            get;
            private set;
        }
        public int End
        {
            get;
            private set;
        }

        public int Length
        {
            get
            {
                return (End - Start + 1);
            }
        }

        public SortedAxisPoints(int pointA, int pointB)
        {
            if (pointA < pointB)
            {
                Start = pointA;
                End = pointB;
            }
            else
            {
                Start = pointB;
                End = pointA;
            }
        }

    }
}
