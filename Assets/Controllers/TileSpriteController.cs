using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class TileSpriteController : MonoBehaviour
{

    // Sprites array for our tiles, assignable in the inspector.
    [Header("Sprites")]
    public Sprite[] tileSprites;
    public Dictionary<Tile, GameObject> tileGameObjectMap;
    private World _world { get { return WorldController.Instance.World; } }

    void Start()
    {
        tileGameObjectMap = new Dictionary<Tile, GameObject>();
        VisualizeWorld();
        _world.RegisterTileCallback(OnTileTypeChanged);
    }

    private void VisualizeWorld()
    {
        // Create a GameObject for each of our tiles, so they show visually
        for (int x = 0; x < _world.Width; x++)
            for (int y = 0; y < _world.Height; y++)
            {
                Tile tileData = _world.GetTileAt(x, y);
                GameObject tileGo = new GameObject();
                // Add the current Tile/GameObject to our dictionary
                // This is needed so we can relate between them
                tileGameObjectMap.Add(tileData, tileGo);
                // Set the name of the tile within the unity editor 
                tileGo.name = "Tile_" + tileData.X + "_" + tileData.Y;
                // Find the GameObject "Tiles" in the editor and set it as parent for all our Tiles
                tileGo.transform.SetParent(GameObject.Find("Tiles").transform, true);
                tileGo.transform.position = new Vector2(tileData.X, tileData.Y);

                // Add a SpriteRender so that we can define a Sprite
                // Add a default sprite for empty tiles.
                tileGo.AddComponent<SpriteRenderer>().sprite = tileSprites[0];

                tileData.RegisterTileTypeChangedCallback(OnTileTypeChanged);
            }
    }


    private void OnTileTypeChanged(Tile tileData)
    {
        if (!tileGameObjectMap.ContainsKey(tileData))
        {
            Debug.LogError("OnTileTypeChanged - Tile could not be found in the tileGameObjectMap!");
            return;
        }

        GameObject tileGo = tileGameObjectMap[tileData];

        if (tileGo == null)
        {
            Debug.LogError("OnTileTypeChanged - GameObject has been destroyed already.");
            return;
        }

        SpriteRenderer tileSr = tileGo.GetComponent<SpriteRenderer>();
        if (tileSr == null)
            Debug.LogError("OnTileTypeChanged - Tile GameObject does not have a SpriteRenderer.");
        if (tileData.Type == TileType.Ground)
            tileSr.sprite = tileSprites[0];
        else if (tileData.Type == TileType.Gras)
            tileSr.sprite = tileSprites[1];
        else if (tileData.Type == TileType.Floor)
            tileSr.sprite = tileSprites[2];
        else
            Debug.LogError("OnTileTypeChanged - Tile sprite out of range.");
    }

    public void DestroyAllTileGameObjects()
    {
        while (tileGameObjectMap.Count > 0)
        {
            Tile tileData = tileGameObjectMap.Keys.First();
            GameObject tileGo = tileGameObjectMap[tileData];

            // Remove the current pair from the dictionary
            tileGameObjectMap.Remove(tileData);
            // Unregister the callback
            tileData.UnregisterTileTypeChangedCallback(OnTileTypeChanged);
            // Destroy the visual part of the Tile -> GameObject
            Destroy(tileGo);
        }
    }

}

