using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World
{

    Tile[,] tiles;
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

    public void RandomizeTiles()
    {
        Debug.Log("World has been randomized.");
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (Random.Range(0, 2) == 0)
                    tiles[x, y].Type = Tile.TileType.Floor;
                else
                    tiles[x, y].Type = Tile.TileType.Gras;
            }
        }
    }
}
