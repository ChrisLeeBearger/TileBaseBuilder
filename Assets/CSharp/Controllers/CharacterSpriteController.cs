using Assets.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpriteController : MonoBehaviour
{
    [SerializeField]
    private int _startBuilders = 10;

    private World _world => WorldController.Instance.World;

    public Dictionary<Character, GameObject> CharacterGameObjectMap { get; private set; } = new();

    public Dictionary<string, Sprite> CharacterSprites { get; private set; } = new();

    // Use this for initialization
    void Start()
    {
        LoadCharacterSprites();
        _world.CharacterCreated += OnCharacterCreated;

        int spawnOffset = _startBuilders / 2;
        // TODO: This should be moved somewhere else in the future
        for (int i = 0; i < _startBuilders; i++) {
            
            Character c = _world.CreateCharacter(_world.GetTileAt(_world.Width / 2 + i - spawnOffset, _world.Height / 2));
        }
        // c.SetDestination(_world.GetTileAt(_world.Width / 2 + 5, _world.Height / 2 + 5));
    }

    // load sprites from Resources folder
    void LoadCharacterSprites()
    {
        Sprite[] s = Resources.LoadAll<Sprite>("Characters/");

        foreach (var sprite in s)
        {
            CharacterSprites.Add(sprite.name, sprite);
        }
    }

    public void OnCharacterCreated(object sender, CharacterCreatedEventArgs args)
    {
        // create the game object for our character, position it in the world set name and parent in the editor
        GameObject charGameObject = new GameObject();
        charGameObject.name = "Character";
        charGameObject.transform.position = new Vector2(args.Character.X, args.Character.Y);
        charGameObject.transform.SetParent(this.transform, true);

        // add sprite render, select sprite and set sorting layer
        SpriteRenderer charSr = charGameObject.AddComponent<SpriteRenderer>();
        charSr.sprite = CharacterSprites["robot_0"];
        charSr.sortingLayerName = "Characters";

        CharacterGameObjectMap.Add(args.Character, charGameObject);
        Debug.Log("Created character: " + charGameObject.name);

        args.Character.CharacterChanged += OnCharacterChanged;
    }

    public void OnCharacterRemoved(Character character)
    {
        GameObject charGameObject = CharacterGameObjectMap[character];

        Destroy(charGameObject);
        CharacterGameObjectMap.Remove(character);

        character.CharacterChanged -= OnCharacterChanged;
    }

    // ! Currently not useable as it was just copy pasted from FurnitureSpriteController
    public Sprite GetSpriteForCharacter(string objectType)
    {
        if (CharacterSprites.ContainsKey(objectType))
            return CharacterSprites[objectType];
        else if (CharacterSprites.ContainsKey(objectType + "_0"))
            return CharacterSprites[objectType + "_0"];
        else
        {
            Debug.LogError("GetSpriteForCharacter - - Sprite not found for: " + objectType);
            return null;
        }
    }

    void OnCharacterChanged(object sender, EventArgs args)
    {
        Character character = (Character)sender;

        if (CharacterGameObjectMap.ContainsKey(character) == false)
        {
            Debug.LogError("CharacterSpriteController::OnCharacterChanged -- trying to change visuals for a character not in CharacterGameObjectMap.");
            return;
        }

        GameObject charGo = CharacterGameObjectMap[character];
        charGo.transform.position = new Vector2(character.X, character.Y);
    }
}
