using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Character
{
    private Job _myJob;
    private float _movementPercentage = 0; // Goes from 0 to 1 as we are transitioning from the current Tile to the destination Tile
    private float _speed = 2f; // Character movement speed in tiles per second

    public event EventHandler CharacterChanged;

    public float X => Mathf.Lerp(CurrTile.X, DestTile.X, _movementPercentage);
    public float Y => Mathf.Lerp(CurrTile.Y, DestTile.Y, _movementPercentage);

    public Tile CurrTile { get; protected set; }
    public Tile DestTile { get; protected set; }
    
    public Character(Tile tile)
    {
        CurrTile = DestTile = tile;
    }

    public void Update(float deltaTime)
    {
        if (_myJob == null)
        {
            _myJob = CurrTile.World.JobQueue.Dequeue();

            if (_myJob != null)
            {
                Debug.Log("Character has taken a job from the queue.");
                DestTile = _myJob.Tile;
                _myJob.JobCompleted += OnJobEnded;
                _myJob.JobCanceled += OnJobEnded;
            }
        }

        // Did we arrive at our destination tile
        if (CurrTile == DestTile) {
            _myJob?.DoWork(deltaTime);
        }
        // What is the total distance from point A to point B
        float distToTravel = Mathf.Sqrt(Mathf.Pow(CurrTile.X - DestTile.X, 2) + Mathf.Pow(CurrTile.Y - DestTile.Y, 2));
        // How much distance can be travel this Update cycle
        float distThisFrame = _speed * deltaTime;
        // How much is that in terms of percentage to our destination
        float percThisFrame = distThisFrame / distToTravel;

        _movementPercentage += percThisFrame;

        if (_movementPercentage >= 1)
        {
            // Character arrived at destination
            CurrTile = DestTile;
            _movementPercentage = 0;
        }

        OnCharacterChanged();
    }

    private void OnCharacterChanged() => CharacterChanged?.Invoke(this, EventArgs.Empty);

    public void SetDestination(Tile tile)
    {
        if (CurrTile.IsNeighbor(tile, true) == false)
            Debug.Log("Character::SetDestination -- Our destination tile is not a neighbor tile.");

        DestTile = tile;
    }

    private void OnJobEnded(object sender, EventArgs args)
    {
        var job = sender as Job;

        if (job != _myJob)
        {
            Debug.LogError("Character completed a job that is not his job. Eventually forgot to unregister something.");
        }

        _myJob = null;
    }
}
