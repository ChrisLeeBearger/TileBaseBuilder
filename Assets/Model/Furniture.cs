using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Furniture
{

    public Tile Tile
    {
        get;
        protected set;
    }

    public string ObjectType
    {
        get;
        protected set;
    }

    float movementCost;

    int width;
    int height;
    Action<Furniture> cbOnChanged;

    Func<Tile, bool> funcPositionValidation;

    public bool linksToNeighbors { get; protected set; }

    protected Furniture()
    {
    }
    static public Furniture CreatePrototype(string ObjectType, float movementCost = 1f, int width = 1, int height = 1, bool linksToNeighbors = false)
    {
        Furniture obj = new Furniture();

        obj.ObjectType = ObjectType;
        obj.movementCost = movementCost;
        obj.width = width;
        obj.height = height;
        obj.linksToNeighbors = linksToNeighbors;
        obj.funcPositionValidation = obj.IsValidPosition;
        return obj;
    }

    static public Furniture PlaceInstance(Furniture proto, Tile tile)
    {
        if (proto.funcPositionValidation(tile) == false)
        {
            Debug.LogError("PlaceInstance --- Position Validity Function returned FALSE.");
            return null;
        }
        Furniture obj = new Furniture();
        obj.ObjectType = proto.ObjectType;
        obj.movementCost = proto.movementCost;
        obj.width = proto.width;
        obj.height = proto.height;
        obj.linksToNeighbors = proto.linksToNeighbors;

        obj.Tile = tile;

        if (tile.PlaceFurniture(obj) == false)
        {
            // If for some reason we were not able to place this object in this Tile.
            // (Probably it was already occupied)

            // Do NOT return our newly instantiated object.
            // (It will be garbage collected.)
            return null;
        }

        return obj;
    }
    public void RegisterOnChangedCallback(Action<Furniture> callbackFunction) => cbOnChanged += callbackFunction;

    public void UnregisterOnChangedCallback(Action<Furniture> callbackFunction) => cbOnChanged -= callbackFunction;

    // Make sure the Tile below is, is of Type Floor and no Furniture is assigned to the Tile
    public bool IsValidPosition(Tile tile) => !(tile.Type != TileType.Floor || tile.Furniture != null);

    public bool IsValidPosition_Door(Tile tile)
    {
        // Make sure we have a pair of East/West or North/West Walls next to us
        Debug.LogError("IsValidPosition_Door - - - Implement me.");
        return false;
    }
}
