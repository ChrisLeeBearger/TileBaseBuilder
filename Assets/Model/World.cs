﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class World
{

    Tile[,] tiles;

    Dictionary<string, InstalledObject> installedObjectPrototypes;
    public int Width
    {
        get;
        private set;
    }
    public int Height
    {
        get;
        private set;
    }
    public World(int width = 100, int height = 100)
    {
        this.Width = width;
        this.Height = height;

        tiles = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x, y] = new Tile(this, x, y);
            }
        }

        Debug.Log("World created with " + (width * height) + " tiles.");

        CreateInstalledObjectsPrototypes();

    }

    void CreateInstalledObjectsPrototypes()
    {
        installedObjectPrototypes = new Dictionary<string, InstalledObject>();
        InstalledObject wallPrototype = InstalledObject.CreatePrototype(
            "greyWall",
            0,      // Impassable
            1,      // Width
            1,      // Height
            true    // Links to neighbor Walls
        );

        installedObjectPrototypes.Add("greyWall", wallPrototype);
        Debug.Log("Prototype has been created: " + wallPrototype.ObjectType);
    }

    public Tile GetTileAt(int x, int y)
    {
        if (x > Width || x < 0 || y > Height || y < 0)
            Debug.LogError("Requested tile is out of range.");

        var tile = tiles[x, y];
        if (tile == null)
            Debug.LogError("Missing tile at position (" + x + ", " + y + ")");

        return tile;
    }

    public void PlaceInstalledObject(string objectType, Tile tile)
    {
        // Check if we have a prototype for the given objectType string
        if (installedObjectPrototypes.ContainsKey(objectType) == false)
        {
            Debug.LogError("installedObjectPrototypes does not contain a prototype for key: " + objectType);
            return;
        }

        InstalledObject obj = InstalledObject.PlaceInstance(installedObjectPrototypes[objectType], tile);

        // Create the visual GameObject if we placed the object successfully
        if (obj != null)
            WorldController.Instance.OnInstalledObjectCreated(obj);
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
}