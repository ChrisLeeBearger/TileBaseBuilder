using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SoundController : MonoBehaviour
{
    float _soundCooldown = 0;
    public AudioClip BuildFloor;
    public AudioClip BuildWall;

    // Use this for initialization
    void Start()
    {
        WorldController.Instance.World.RegisterFurnitureCallback(OnFurnitureCreated);
        WorldController.Instance.World.RegisterTileCallback(OnTileTypeChange);
    }

    // Update is called once per frame
    void Update()
    {
        _soundCooldown -= Time.deltaTime;
    }

    void OnTileTypeChange(Tile tile)
    {
        if (CanPlaySound())
            AudioSource.PlayClipAtPoint(BuildFloor, Camera.main.transform.position);
    }

    void OnFurnitureCreated(Furniture furniture)
    {
        if (CanPlaySound())
            AudioSource.PlayClipAtPoint(BuildWall, Camera.main.transform.position);
    }

    Boolean CanPlaySound()
    {
        if (_soundCooldown <= 0)
        {
            _soundCooldown = 0.10f;
            return true;
        }
        return false;
    }

}
