using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class World
{

    Tile[,] tiles;

    Dictionary<string, Furniture> _furniturePrototypes;
    public int Width { get; private set; }
    public int Height { get; private set; }
    Action<Furniture> _cbFurnitureCreated;
    Action<Tile> _cbTileChanged;
    public Queue<Job> JobQueue;
    public World(int width = 100, int height = 100)
    {
        JobQueue = new Queue<Job>();
        this.Width = width;
        this.Height = height;

        tiles = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x, y] = new Tile(this, x, y);
                tiles[x, y].RegisterTileTypeChangedCallback(OnTileChanged);

            }
        }

        Debug.Log("World created with " + (width * height) + " tiles.");

        CreateFurniturePrototypes();

    }

    void CreateFurniturePrototypes()
    {
        _furniturePrototypes = new Dictionary<string, Furniture>();
        Furniture wallPrototype = Furniture.CreatePrototype(
            "greyWall",
            0,      // Impassable
            1,      // Width
            1,      // Height
            true    // Links to neighbor Walls
        );

        _furniturePrototypes.Add("greyWall", wallPrototype);
        Debug.Log("Prototype has been created: " + wallPrototype.ObjectType);
    }

    public Tile GetTileAt(int x, int y)
    {
        if (x > Width || x < 0 || y > Height || y < 0)
            Debug.LogError("Requested tile is out of range.");

        Tile tile = tiles[x, y];
        if (tile == null)
            Debug.LogError("Missing tile at position (" + x + ", " + y + ")");

        return tile;
    }

    public void PlaceFurniture(string objectType, Tile tile)
    {
        // Check if we have a prototype for the given objectType string
        if (_furniturePrototypes.ContainsKey(objectType) == false)
        {
            Debug.LogError("FurniturePrototypes does not contain a prototype for key: " + objectType);
            return;
        }

        Furniture obj = Furniture.PlaceInstance(_furniturePrototypes[objectType], tile);

        // Create the visual GameObject if we placed the object successfully
        if (obj != null)
        {
            WorldController.Instance.OnFurnitureCreated(obj);
            if (_cbFurnitureCreated != null)
                _cbFurnitureCreated(obj);
        }

    }

    public void RandomizeTiles()
    {
        Debug.Log("World has been randomized.");
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (UnityEngine.Random.Range(0, 2) == 0)
                    tiles[x, y].Type = TileType.Ground;
                else
                    tiles[x, y].Type = TileType.Gras;
            }
        }
    }
    public void RegisterFurnitureCallback(Action<Furniture> callbackFunction) => _cbFurnitureCreated += callbackFunction;

    public void UnregisterFurnitureCallback(Action<Furniture> callbackFunction) => _cbFurnitureCreated -= callbackFunction;

    public void RegisterTileCallback(Action<Tile> callbackFunction) => _cbTileChanged += callbackFunction;

    public void UnregisterTileCallback(Action<Tile> callbackFunction) => _cbTileChanged -= callbackFunction;

    void OnTileChanged(Tile tile)
    {
        if (_cbTileChanged == null)
            return;
        _cbTileChanged(tile);
    }
}
