using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Assets.Events;

public class FurnitureSpriteController : MonoBehaviour
{
    private World _world => WorldController.Instance.World;

    public Dictionary<Furniture, GameObject> FurnitureGameObjectMap = new();
    public Dictionary<string, Sprite> FurnitureSprites = new();

    void Start()
    {
        LoadFurnitureSprites();
        _world.FurnitureCreated += OnFurnitureCreated;
    }

    void LoadFurnitureSprites()
    {
        Sprite[] s = Resources.LoadAll<Sprite>("Walls/");

        foreach (var sprite in s)
        {
            FurnitureSprites.Add(sprite.name, sprite);
        }
    }

    public void OnFurnitureCreated(object sender, FurnitureCreatedEventArgs args)
    {
        GameObject furnitureGameObject = new GameObject();
        Furniture furniture = args.Furniture;

        FurnitureGameObjectMap.Add(furniture, furnitureGameObject);
        furnitureGameObject.name = furniture.ObjectType + "_" + furniture.Tile.X + "_" + furniture.Tile.Y;
        furnitureGameObject.transform.position = new Vector3(furniture.Tile.X, furniture.Tile.Y, -1);
        furnitureGameObject.transform.SetParent(this.transform, true);

        SpriteRenderer sr = furnitureGameObject.AddComponent<SpriteRenderer>();
        sr.sprite = GetFurnitureSprite(furniture, true);
        sr.sortingLayerName = "Furniture";

        furniture.FurnitureChanged += OnFurnitureChanged;
    }

    public void OnFurnitureRemoved(object sender, EventArgs args)
    {
        var furniture = sender as Furniture;
        GameObject furnitureGameObject = FurnitureGameObjectMap[furniture];

        Destroy(furnitureGameObject);
        furniture.Tile.RemoveFurniture();
        furniture.FurnitureChanged -= OnFurnitureChanged;
        FurnitureGameObjectMap.Remove(furniture);
    }

    public void UpdateFurnitureSprites(List<Tile> tiles)
    {
        HashSet<Tile> tilesToUpdate = new HashSet<Tile>();
        foreach (Tile tile in tiles)
        {
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    tilesToUpdate.Add(_world.GetTileAt(tile.X + x, tile.Y + y));
                }
            }
        }

        Debug.Log("Tiles to update:" + tilesToUpdate.Count);

        foreach (Tile tile in tiles)
            tilesToUpdate.Remove(tile);

        Debug.Log("Tiles to update:" + tilesToUpdate.Count);

        foreach (Tile tile in tilesToUpdate)
        {
            if (tile.Furniture == null)
                continue;
            var furnGo = FurnitureGameObjectMap[tile.Furniture];
            furnGo.GetComponent<SpriteRenderer>().sprite = GetFurnitureSprite(tile.Furniture, false);
        }
    }

    public Sprite GetFurnitureSprite(Furniture obj, bool updateNeighbors = false)
    {
        if (obj.linksToNeighbors == false)
        {
            Debug.Log("Set Sprite: " + FurnitureSprites[obj.ObjectType + "_0"]);
            return FurnitureSprites[obj.ObjectType + "_0"];
        }

        // Array of fixed defined direct neighbors -> North, East, South, West
        Vector2Int[] directNeighbors = new Vector2Int[] {
            new Vector2Int(0, 1),
            new Vector2Int(1, 0),
            new Vector2Int(0,-1),
            new Vector2Int(-1,0)
        };

        // Array of fixed defined diagonal neighbors -> NorthEast, SouthEast, SouthWest, NorthWest
        Vector2Int[] inDirectNeighbors = new Vector2Int[] {
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
            Tile neighborTile = _world.GetTileAt(obj.Tile.X + directNeighbors[i].x, obj.Tile.Y + directNeighbors[i].y);
            if (neighborTile != null && neighborTile.Furniture != null && obj.ObjectType == neighborTile.Furniture.ObjectType)
            {
                if (i == 0)
                    firstNeighborConnected = true;

                spriteNumber += bitValue;
                if (updateNeighbors == true)
                {
                    SpriteRenderer neighborSpriteRenderer = FurnitureGameObjectMap[neighborTile.Furniture].GetComponent<SpriteRenderer>();
                    neighborSpriteRenderer.sprite = GetFurnitureSprite(neighborTile.Furniture);
                }

                if (lastNeighborConnected == true)
                {
                    Tile neighborTileCorner = _world.GetTileAt(obj.Tile.X + inDirectNeighbors[i - 1].x, obj.Tile.Y + inDirectNeighbors[i - 1].y);
                    if (neighborTileCorner != null && neighborTileCorner.Furniture != null && obj.ObjectType == neighborTileCorner.Furniture.ObjectType)
                    {
                        spriteNumber += bitValue * 8;
                        if (updateNeighbors == true)
                        {
                            SpriteRenderer neighborSpriteRenCorner = FurnitureGameObjectMap[neighborTileCorner.Furniture].GetComponent<SpriteRenderer>();
                            neighborSpriteRenCorner.sprite = GetFurnitureSprite(neighborTileCorner.Furniture);
                        }
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
            Tile neighborTileCorner = _world.GetTileAt(obj.Tile.X + inDirectNeighbors[3].x, obj.Tile.Y + inDirectNeighbors[3].y);
            if (neighborTileCorner != null && neighborTileCorner.Furniture != null && obj.ObjectType == neighborTileCorner.Furniture.ObjectType)
            {
                spriteNumber += 128;
                if (updateNeighbors == true)
                {
                    SpriteRenderer neighborSpriteRenCorner = FurnitureGameObjectMap[neighborTileCorner.Furniture].GetComponent<SpriteRenderer>();
                    neighborSpriteRenCorner.sprite = GetFurnitureSprite(neighborTileCorner.Furniture);
                }
            }
        }

        // Check if the sprite can be found
        if (FurnitureSprites.ContainsKey(obj.ObjectType + "_" + spriteNumber) == false)
        {
            Debug.LogError("GetFurnitureSprite - - Sprite not found for: " + obj.ObjectType + "_" + spriteNumber);
            return null;
        }
        return FurnitureSprites[obj.ObjectType + "_" + spriteNumber];
    }

    void OnFurnitureChanged(object sender, EventArgs args)
    {
        //TODO: implement
    }

    public Sprite GetSpriteForFurniture(string objectType)
    {
        if (FurnitureSprites.ContainsKey(objectType))
            return FurnitureSprites[objectType];
        else if (FurnitureSprites.ContainsKey(objectType + "_0"))
            return FurnitureSprites[objectType + "_0"];
        else
        {
            Debug.LogError("GetSpriteForFurniture - - Sprite not found for: " + objectType);
            return null;
        }
    }
}

