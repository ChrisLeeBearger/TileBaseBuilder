using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Furniture
{
    private float movementCost;

    private int width;
    private int height;

    private Func<Tile, bool> _funcPositionValidation;

    public event EventHandler FurnitureChanged;

    public bool linksToNeighbors { get; protected set; }

    public Tile Tile { get; protected set; }

    public string ObjectType { get; protected set; }

    protected Furniture()
    {

    }

    public static Furniture CreatePrototype(
        string ObjectType,
        float movementCost = 1f,
        int width = 1,
        int height = 1,
        bool linksToNeighbors = false)
    {
        Furniture obj = new Furniture();

        obj.ObjectType = ObjectType;
        obj.movementCost = movementCost;
        obj.width = width;
        obj.height = height;
        obj.linksToNeighbors = linksToNeighbors;
        obj._funcPositionValidation = obj.IsValidPosition;

        return obj;
    }

    public static Furniture PlaceInstance(Furniture proto, Tile tile)
    {
        if (proto.IsValidPosition(tile) == false)
        {
            Debug.LogError("PlaceInstance --- Invalid position.");
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

    public bool IsValidPosition(Tile tile) => tile.Type == TileType.Floor && tile.Furniture == null;

    public bool IsValidPositionDoor(Tile tile)
    {
        // Make sure we have a pair of East/West or North/West Walls next to us
        Debug.LogError("IsValidPositionDoor - - - Implement me.");
        return false;
    }
}
