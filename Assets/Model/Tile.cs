using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tile
{

    public enum TileType { Empty, Floor, Gras, Dirt, Water, Stone };

    Action<Tile> cbTileTypeChanged;

    TileType type = TileType.Empty;
    public TileType Type
    {
        get
        {
            return type;
        }
        set
        {
            if (type != value)
            {
                type = value;
                if (cbTileTypeChanged != null)
                    cbTileTypeChanged(this);
            }
        }
    }

    LooseObject looseObject;
    InstalledObject installedObject;

    World world;
    public int X
    {
        get;
        private set;
    }
    public int Y
    {
        get;
        private set;
    }

    public Tile(World world, int x, int y)
    {
        this.world = world;
        this.X = x;
        this.Y = y;
    }

    public void RegisterTileTypeChangedCallback(Action<Tile> callback)
    {
        cbTileTypeChanged += callback;
    }

    public void UnregisterTileTypeChangedCallback(Action<Tile> callback)
    {
        cbTileTypeChanged -= callback;
    }
}