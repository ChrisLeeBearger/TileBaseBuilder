﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum TileType { Empty, Ground, Gras, Floor, Water, Stone };

public class Tile
{


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

    public bool PlaceObject(InstalledObject objInstance)
    {
        if (objInstance == null)
        {
            installedObject = null;
            return true;
        }

        if (installedObject != null)
        {
            Debug.LogError("Trying to assign an installed object to a tile that already has one!");
            return false;
        }

        if (Type != TileType.Floor)
        {
            Debug.LogError("Walls can only be built on Tiles of Type Floor.");
            return false;
        }

        installedObject = objInstance;
        return true;
    }
}