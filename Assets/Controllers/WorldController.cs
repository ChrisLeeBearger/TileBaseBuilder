using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

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
    [SerializeField] private Sprite wallSprite;

    public Dictionary<Tile, GameObject> tileGameObjectMap;
    public Dictionary<InstalledObject, GameObject> installedObjectGameObjectMap;
    public Dictionary<string, Sprite> installedObjectSprites;

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

        tileGameObjectMap = new Dictionary<Tile, GameObject>();
        installedObjectGameObjectMap = new Dictionary<InstalledObject, GameObject>();

        LoadInstalledObjectSprites();

        World = new World();

        // Create a GameObject for each of our tiles, so they show visually
        for (int x = 0; x < World.Width; x++)
            for (int y = 0; y < World.Height; y++)
            {
                Tile tileData = World.GetTileAt(x, y);
                GameObject tileGo = new GameObject();
                tileGameObjectMap.Add(tileData, tileGo);
                tileGo.name = "Tile_" + tileData.X + "_" + tileData.Y;
                tileGo.transform.SetParent(GameObject.Find("Tiles").transform, true);
                tileGo.transform.position = new Vector2(tileData.X, tileData.Y);
                SpriteRenderer tileSr = tileGo.AddComponent<SpriteRenderer>();

                tileData.RegisterTileTypeChangedCallback(OnTileTypeChanged);
                tileData.RegisterTileTypeChangedCallback(AnotherCallBackTest);
                // Previous integration of registrating a callback via lambda
                // tileData.RegisterTileTypeChangedCallback((tile) => { OnTileTypeChanged(tile, tileGo); });
            }
        World.RandomizeTiles();
    }

    void Update()
    {

    }

    private void AnotherCallBackTest(Tile tileData)
    {
        //Debug.Log("This callback was successfully called!");
    }

    private void OnTileTypeChanged(Tile tileData)
    {
        if (!tileGameObjectMap.ContainsKey(tileData))
        {
            Debug.LogError("OnTileTypeChanged - Tile could not be found in the tileGameObjectMap!");
            return;
        }

        GameObject tileGo = tileGameObjectMap[tileData];

        if (tileGo == null)
        {
            Debug.LogError("OnTileTypeChanged - GameObject has been destroyed already.");
            return;
        }

        SpriteRenderer tileSr = tileGo.GetComponent<SpriteRenderer>();
        if (tileSr == null)
            Debug.LogError("OnTileTypeChanged - Tile GameObject does not have a SpriteRenderer.");
        if (tileData.Type == TileType.Ground)
            tileSr.sprite = tileSprites[0];
        else if (tileData.Type == TileType.Gras)
            tileSr.sprite = tileSprites[1];
        else if (tileData.Type == TileType.Floor)
            tileSr.sprite = tileSprites[2];
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

    public void DestroyAllTileGameObjects()
    {
        while (tileGameObjectMap.Count > 0)
        {
            Tile tileData = tileGameObjectMap.Keys.First();
            GameObject tileGo = tileGameObjectMap[tileData];

            // Remove the current pair from the dictionary
            tileGameObjectMap.Remove(tileData);
            // Unregister the callback
            tileData.UnregisterTileTypeChangedCallback(OnTileTypeChanged);
            // Destroy the visual part of the Tile -> GameObject
            Destroy(tileGo);
        }
    }

    public void OnInstalledObjectCreated(InstalledObject obj)
    {
        GameObject go = new GameObject();

        installedObjectGameObjectMap.Add(obj, go);
        go.name = obj.ObjectType + "_" + obj.Tile.X + "_" + obj.Tile.Y;
        go.transform.position = new Vector3(obj.Tile.X, obj.Tile.Y, 0);
        go.transform.SetParent(this.transform, true);

        go.AddComponent<SpriteRenderer>().sprite = GetInstalledObjectSprite(obj);

        obj.RegisterOnChangedCallback(OnInstalledObjectChanged);

    }

    Sprite GetInstalledObjectSprite(InstalledObject obj)
    {
        if (obj.linksToNeighbors == false)
        {
            Debug.Log("Set Sprite: " + installedObjectSprites[obj.ObjectType + "_0"]);
            return installedObjectSprites[obj.ObjectType + "_0"];
        }

        Debug.Log("Set Sprite: " + installedObjectSprites[obj.ObjectType + "_0"]);
        return installedObjectSprites[obj.ObjectType + "_0"];
    }

    void OnInstalledObjectChanged(InstalledObject obj)
    {

    }

    void LoadInstalledObjectSprites()
    {
        installedObjectSprites = new Dictionary<string, Sprite>();
        Sprite[] s = Resources.LoadAll<Sprite>("Walls/");

        foreach (var sprite in s)
        {
            installedObjectSprites.Add(sprite.name, sprite);
        }

    }
}
