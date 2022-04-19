using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class WorldController : MonoBehaviour
{
    public static WorldController Instance { get; protected set; }

    // Sprites array for our tiles, assignable in the inspector.
    [Header("Sprites")]
    public Sprite[] tileSprites;
    [SerializeField] private Sprite wallSprite;
    [SerializeField] private int worldHeight;
    [SerializeField] private int worldWidth;

    public Dictionary<Tile, GameObject> tileGameObjectMap;
    public Dictionary<Furniture, GameObject> FurnitureGameObjectMap;
    public Dictionary<string, Sprite> FurnitureSprites;

    public World World { get; protected set; }
    // Use this for initialization
    void Awake()
    {
        if (Instance != null)
            Debug.LogError("World, Start - There should only be one world.");
        Instance = this;

        World = new World(worldWidth,worldHeight);
        // Center the camera on game start
        Camera.main.transform.position = new Vector3(World.Width / 2, World.Height / 2, -15);
    }

    void Update()
    {
        // TODO: Should also implement ways to manipulate time (pause, slowdown, fastforward)
        World.Update(Time.deltaTime);
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
