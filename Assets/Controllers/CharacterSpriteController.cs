using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpriteController : MonoBehaviour
{

    public Dictionary<Character, GameObject> CharacterGameObjectMap;
    public Dictionary<string, Sprite> CharacterSprites;
    private World _world { get { return WorldController.Instance.World; } }
    // Use this for initialization
    void Start()
    {
        LoadCharacterSprites();
        CharacterGameObjectMap = new Dictionary<Character, GameObject>();
        _world.RegisterCharacterCallback(OnCharacterCreated);
        // TODO: This should be moved somewhere else in the future
        Character c = _world.CreateCharacter(_world.GetTileAt(_world.Width / 2, _world.Height / 2));
        // c.SetDestination(_world.GetTileAt(_world.Width / 2 + 5, _world.Height / 2 + 5));
    }

    void LoadCharacterSprites()
    {
        CharacterSprites = new Dictionary<string, Sprite>();
        Sprite[] s = Resources.LoadAll<Sprite>("Characters/");

        foreach (var sprite in s)
        {
            CharacterSprites.Add(sprite.name, sprite);
        }
    }
    public void OnCharacterCreated(Character c)
    {
        GameObject charGo = new GameObject();

        CharacterGameObjectMap.Add(c, charGo);
        charGo.name = "Character";
        charGo.transform.position = new Vector2(c.X, c.Y);
        charGo.transform.SetParent(this.transform, true);
        SpriteRenderer charSr = charGo.AddComponent<SpriteRenderer>();
        charSr.sprite = CharacterSprites["player"];
        charSr.sortingLayerName = "Characters";
        Debug.Log("Created character: " + charGo.name);
        c.RegisterCharacterChangedCallback(OnCharacterChanged);
    }

    public void OnCharacterRemoved(Character c)
    {
        GameObject charGo = CharacterGameObjectMap[c];
        Destroy(charGo);
        CharacterGameObjectMap.Remove(c);
    }

    // ! Currently not useable as it was just taken over from FurnitureSpriteController
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

    void OnCharacterChanged(Character c)
    {
        if (CharacterGameObjectMap.ContainsKey(c) == false)
        {
            Debug.LogError("CharacterSpriteController::OnCharacterChanged -- trying to change visuals for a character not in CharacterGameObjectMap.");
            return;
        }
        GameObject charGo = CharacterGameObjectMap[c];
        charGo.transform.position = new Vector2(c.X, c.Y);
    }
}
