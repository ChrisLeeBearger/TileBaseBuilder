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
    public Dictionary<Furniture, GameObject> FurnitureGameObjectMap;
    public Dictionary<string, Sprite> FurnitureSprites;

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
        FurnitureGameObjectMap = new Dictionary<Furniture, GameObject>();

        LoadFurnitureSprites();

        World = new World();

        // Create a GameObject for each of our tiles, so they show visually
        for (int x = 0; x < World.Width; x++)
            for (int y = 0; y < World.Height; y++)
            {
                Tile tileData = World.GetTileAt(x, y);
                GameObject tileGo = new GameObject();
                // Add the current Tile/GameObject to our dictionary
                // This is needed so we can relate between them
                tileGameObjectMap.Add(tileData, tileGo);
                // Set the name of the tile within the unity editor 
                tileGo.name = "Tile_" + tileData.X + "_" + tileData.Y;
                // Find the GameObject "Tiles" in the editor and set it as parent for all our Tiles
                tileGo.transform.SetParent(GameObject.Find("Tiles").transform, true);
                tileGo.transform.position = new Vector2(tileData.X, tileData.Y);

                // Add a SpriteRender so that we can define a Sprite
                // Add a default sprite for empty tiles.
                tileGo.AddComponent<SpriteRenderer>().sprite = tileSprites[0];

                tileData.RegisterTileTypeChangedCallback(OnTileTypeChanged);
                tileData.RegisterTileTypeChangedCallback(AnotherCallBackTest);
                // Previous integration of registrating a callback via lambda
                // tileData.RegisterTileTypeChangedCallback((tile) => { OnTileTypeChanged(tile, tileGo); });
            }
        // World.RandomizeTiles();

        // Center the camera on game start
        Camera.main.transform.position = new Vector3(World.Width / 2, World.Height / 2, -15);
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
    public Vector2Int GetStartAndEndCoordinates(int start, int end) => start <= end ? new Vector2Int(start, end) : new Vector2Int(end, start);
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

        public int Length => End - Start + 1;

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

    public void OnFurnitureCreated(Furniture obj)
    {
        GameObject furnGo = new GameObject();

        FurnitureGameObjectMap.Add(obj, furnGo);
        furnGo.name = obj.ObjectType + "_" + obj.Tile.X + "_" + obj.Tile.Y;
        furnGo.transform.position = new Vector3(obj.Tile.X, obj.Tile.Y, -1);
        furnGo.transform.SetParent(this.transform, true);

        furnGo.AddComponent<SpriteRenderer>().sprite = GetFurnitureSprite(obj, true);

        obj.RegisterOnChangedCallback(OnFurnitureChanged);

    }

    Sprite GetFurnitureSprite(Furniture obj, bool updateNeighbors = false)
    {
        if (obj.linksToNeighbors == false)
        {
            Debug.Log("Set Sprite: " + FurnitureSprites[obj.ObjectType + "_0"]);
            return FurnitureSprites[obj.ObjectType + "_0"];
        }

        // Array of fixed defined direct neighbors -> North, East, South, West
        Vector2Int[] dirctNeighbors = new Vector2Int[] {
            new Vector2Int(0, 1),
            new Vector2Int(1, 0),
            new Vector2Int(0,-1),
            new Vector2Int(-1,0)
        };

        // Array of fixed defined diagonal neighbors -> NorthEast, SouthEast, SouthWest, NorthWest
        Vector2Int[] inDirctNeighbors = new Vector2Int[] {
            new Vector2Int(1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1,-1),
            new Vector2Int(-1,1)
        };

        // spriteNumber will define which of the Sprites will be taken
        int spriteNumber = 0;
        int bitValue = 1;
        bool firstNeighborConnected = false;
        bool lastNeighborConnected = false;

        for (int i = 0; i < 4; i++)
        {
            Tile neighborTile = World.GetTileAt(obj.Tile.X + dirctNeighbors[i].x, obj.Tile.Y + dirctNeighbors[i].y);
            if (neighborTile != null && neighborTile.Furniture != null && obj.ObjectType == neighborTile.Furniture.ObjectType)
            {
                if (i == 0)
                    firstNeighborConnected = true;

                spriteNumber += bitValue;
                SetNeighborSprite(neighborTile.Furniture);

                if (lastNeighborConnected == true)
                {
                    Tile neighborTileCorner = World.GetTileAt(obj.Tile.X + inDirctNeighbors[i - 1].x, obj.Tile.Y + inDirctNeighbors[i - 1].y);
                    if (neighborTileCorner != null && neighborTileCorner.Furniture != null && obj.ObjectType == neighborTileCorner.Furniture.ObjectType)
                    {
                        spriteNumber += bitValue * 8;
                        SetNeighborSprite(neighborTileCorner.Furniture);
                    }
                }
                lastNeighborConnected = true;
            }
            else
                lastNeighborConnected = false;
            // Increase the bit representation for the next loop    
            bitValue *= 2;
        }

        if (firstNeighborConnected && lastNeighborConnected)
        {
            Tile neighborTileCorner = World.GetTileAt(obj.Tile.X + inDirctNeighbors[3].x, obj.Tile.Y + inDirctNeighbors[3].y);
            if (neighborTileCorner != null && neighborTileCorner.Furniture != null && obj.ObjectType == neighborTileCorner.Furniture.ObjectType)
            {
                spriteNumber += 128;
                SetNeighborSprite(neighborTileCorner.Furniture);
            }
        }
        return FurnitureSprites[obj.ObjectType + "_" + spriteNumber];

        void SetNeighborSprite(Furniture furniture)
        {
            if (updateNeighbors == true)
            {
                SpriteRenderer neighborSpriteRenCorner = FurnitureGameObjectMap[furniture].GetComponent<SpriteRenderer>();
                neighborSpriteRenCorner.sprite = GetFurnitureSprite(furniture);
            }
        }
    }

    void OnFurnitureChanged(Furniture obj)
    {

    }

    void LoadFurnitureSprites()
    {
        FurnitureSprites = new Dictionary<string, Sprite>();
        Sprite[] s = Resources.LoadAll<Sprite>("Walls/");

        foreach (var sprite in s)
        {
            FurnitureSprites.Add(sprite.name, sprite);
        }

    }
}
