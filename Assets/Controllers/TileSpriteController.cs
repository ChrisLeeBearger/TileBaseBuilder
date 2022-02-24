using Assets.Events;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileSpriteController : MonoBehaviour
{

    private World _world => WorldController.Instance.World;

    // Sprites array for our tiles, assignable in the inspector.
    [Header("Sprites")]
    public Sprite[] tileSprites;

    public Dictionary<Tile, GameObject> tileGameObjectMap = new();

    private void Start()
    {
        VisualizeWorld();
        _world.TileChanged += OnTileTypeChanged;
    }

    private void VisualizeWorld()
    {
        // Create a GameObject for each of our tiles, so they show visually
        for (var x = 0; x < _world.Width; x++)
            for (var y = 0; y < _world.Height; y++)
            {
                var tileData = _world.GetTileAt(x, y);
                var tileGameObject = new GameObject();
                // Add the current Tile/GameObject to our dictionary
                // This is needed so we can relate between them
                tileGameObjectMap.Add(tileData, tileGameObject);
                // Set the name of the tile within the unity editor 
                tileGameObject.name = "Tile_" + tileData.X + "_" + tileData.Y;
                // Find the GameObject "Tiles" in the editor and set it as parent for all our Tiles
                tileGameObject.transform.SetParent(GameObject.Find("Tiles").transform, true);
                tileGameObject.transform.position = new Vector2(tileData.X, tileData.Y);

                // Add a SpriteRender so that we can define a Sprite
                // Add a default sprite for empty tiles.
                var sr = tileGameObject.AddComponent<SpriteRenderer>();
                sr.sprite = tileSprites[0];
                sr.sortingLayerName = "Tiles";

                tileData.TileChanged += OnTileTypeChanged;
            }
    }


    private void OnTileTypeChanged(object sender, TileChangedEventArgs args)
    {
        if (!tileGameObjectMap.ContainsKey(args.Tile))
        {
            Debug.LogError("OnTileTypeChanged - Tile could not be found in the tileGameObjectMap!");
            return;
        }

        var tileGameObject = tileGameObjectMap[args.Tile];

        if (tileGameObject == null)
        {
            Debug.LogError("OnTileTypeChanged - GameObject has been destroyed already.");
            return;
        }

        var tileSr = tileGameObject.GetComponent<SpriteRenderer>();
        if (tileSr == null)
            Debug.LogError("OnTileTypeChanged - Tile GameObject does not have a SpriteRenderer.");
        if (args.Tile.Type == TileType.Ground)
            tileSr.sprite = tileSprites[0];
        else if (args.Tile.Type == TileType.Gras)
            tileSr.sprite = tileSprites[1];
        else if (args.Tile.Type == TileType.Floor)
            tileSr.sprite = tileSprites[2];
        else
            Debug.LogError("OnTileTypeChanged - Tile sprite out of range.");
    }

    public void DestroyAllTileGameObjects()
    {
        while (tileGameObjectMap.Count > 0)
        {
            var tileData = tileGameObjectMap.Keys.First();
            var tileGo = tileGameObjectMap[tileData];

            // Remove the current pair from the dictionary
            tileGameObjectMap.Remove(tileData);
            // Unregister the callback
            tileData.TileChanged -= OnTileTypeChanged;
            // Destroy the visual part of the Tile -> GameObject
            Destroy(tileGo);
        }
    }
}