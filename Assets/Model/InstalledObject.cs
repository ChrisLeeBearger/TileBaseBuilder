using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InstalledObject
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
    Action<InstalledObject> cbOnChanged;

    public bool linksToNeighbors { get; protected set; }

    protected InstalledObject()
    {
    }
    static public InstalledObject CreatePrototype(string ObjectType, float movementCost = 1f, int width = 1, int height = 1, bool linksToNeighbors = false)
    {
        InstalledObject obj = new InstalledObject();

        obj.ObjectType = ObjectType;
        obj.movementCost = movementCost;
        obj.width = width;
        obj.height = height;
        obj.linksToNeighbors = linksToNeighbors;

        return obj;
    }

    static public InstalledObject PlaceInstance(InstalledObject proto, Tile tile)
    {
        InstalledObject obj = new InstalledObject();
        obj.ObjectType = proto.ObjectType;
        obj.movementCost = proto.movementCost;
        obj.width = proto.width;
        obj.height = proto.height;
        obj.linksToNeighbors = proto.linksToNeighbors;

        obj.Tile = tile;

        if (tile.PlaceObject(obj) == false)
        {
            // If for some reason we were not able to place this object in this Tile.
            // (Probably it was already occupied)

            // Do NOT return our newly instantiated object.
            // (It will be garbage collected.)
            return null;
        }

        return obj;
    }
    public void RegisterOnChangedCallback(Action<InstalledObject> callbackFunction)
    {
        cbOnChanged += callbackFunction;
    }

    public void UnregisterOnChangedCallback(Action<InstalledObject> callbackFunction)
    {
        cbOnChanged -= callbackFunction;
    }
}
