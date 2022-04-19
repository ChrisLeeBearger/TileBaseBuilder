using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Assets.Events;

public class SoundController : MonoBehaviour
{
    float _soundCooldown = 0;

    // audioclip assigned via unity editor
    public AudioClip BuildFloor;
    public AudioClip BuildWall;

    // Use this for initialization
    void Start()
    {
        WorldController.Instance.World.FurnitureCreated += OnFurnitureCreated;
        WorldController.Instance.World.TileChanged += OnTileTypeChange;
    }

    // Update is called once per frame
    void Update()
    {
        _soundCooldown -= Time.deltaTime;
    }

    void OnTileTypeChange(object sender, TileChangedEventArgs args)
    {
        if (CanPlaySound())
            AudioSource.PlayClipAtPoint(BuildFloor, Camera.main.transform.position);
    }

    void OnFurnitureCreated(object sender, FurnitureCreatedEventArgs args)
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
