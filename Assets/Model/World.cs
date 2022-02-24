using Assets.Events;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class World
{
    private readonly List<Character> _characters;
    private readonly Tile[,] _tiles;

    public event EventHandler<CharacterCreatedEventArgs> CharacterCreated;
    public event EventHandler<FurnitureCreatedEventArgs> FurnitureCreated;
    public event EventHandler<TileChangedEventArgs> TileChanged; 

    private Dictionary<string, Furniture> _furniturePrototypes;

    public JobQueue JobQueue;
    public int Width { get; }
    public int Height { get; }

    public World(int width = 100, int height = 100)
    {
        JobQueue = new JobQueue();
        _characters = new List<Character>();

        _tiles = new Tile[width, height];
        Width = width;
        Height = height;

        for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
            {
                _tiles[x, y] = new Tile(this, x, y);
                _tiles[x, y].TileChanged += OnTileChanged;
            }

        Debug.Log("World created with " + width * height + " tiles.");
        CreateFurniturePrototypes();
    }

    private void OnCharacterCreated(Character character) => CharacterCreated.Invoke(this, new CharacterCreatedEventArgs { Character = character });
    private void OnFurnitureCreated(Furniture furniture) => FurnitureCreated.Invoke(this, new FurnitureCreatedEventArgs { Furniture = furniture });
    private void OnTileChanged(object sender, TileChangedEventArgs args) => TileChanged.Invoke(sender, args);

    public void Update(float deltaTime)
    {
        foreach (var c in _characters)
            c.Update(deltaTime);
    }

    public Character CreateCharacter(Tile tile)
    {
        var character = new Character(tile);

        _characters.Add(character);
        OnCharacterCreated(character);

        return character;
    }

    private void CreateFurniturePrototypes()
    {
        _furniturePrototypes = new Dictionary<string, Furniture>();
        var wallPrototype = Furniture.CreatePrototype(
            "greyWall",
            0, // Impassable
            1, // Width
            1, // Height
            true // Links to neighbor walls
        );
        _furniturePrototypes.Add("greyWall", wallPrototype);
        Debug.Log("Prototype has been created: " + wallPrototype.ObjectType);
    }

    public Tile GetTileAt(int x, int y)
    {
        // if requested tile is out of world coordinates
        if (x > Width || x < 0 || y > Height || y < 0)
        {
            Debug.LogError("Requested tile is out of range.");
            return null;
        }

        var tile = _tiles[x, y];
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

        var furniture = Furniture.PlaceInstance(_furniturePrototypes[objectType], tile);

        // Create the visual GameObject if we placed the object successfully
        if (furniture != null)
            OnFurnitureCreated(furniture);
    }

    public void RandomizeTiles()
    {
        for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                if (Random.Range(0, 2) == 0)
                    _tiles[x, y].Type = TileType.Ground;
                else
                    _tiles[x, y].Type = TileType.Gras;
        Debug.Log("World has been randomized.");
    }


    public bool IsFurniturePlacementValid(string furnitureType, Tile tile)
    {
        return _furniturePrototypes[furnitureType].IsValidPosition(tile);
    }

    public Furniture GetFurniturePrototype(string objectType)
    {
        if (_furniturePrototypes.ContainsKey(objectType) == false)
            Debug.LogError("No furniture with type: " + objectType);

        return _furniturePrototypes[objectType];
    }
}