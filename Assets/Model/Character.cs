using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Character
{
    public float X { get { return Mathf.Lerp(CurrTile.X, DestTile.X, movementPercentage); } }
    public float Y { get { return Mathf.Lerp(CurrTile.Y, DestTile.Y, movementPercentage); } }
    public Tile CurrTile { get; protected set; }
    public Tile DestTile { get; protected set; }
    private Job _myJob;
    private float movementPercentage = 0; // Goes from 0 to 1 as we are transitioning from the current Tile to the destination Tile
    private float speed = 2f; // Character movement speed in tiles per second
    private Action<Character> _cbCharacterChanged;
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
                _myJob.RegisterJobCancelCallback(OnJobEnded);
                _myJob.RegisterJobCompleteCallback(OnJobEnded);
            }
        }

        // Did we arrive at our destination tile
        if (CurrTile == DestTile)
        {
            if (_myJob != null)
            {
                _myJob.DoWork(deltaTime);
            }
        }
        // What is the total distance from point A to point B
        float distToTravel = Mathf.Sqrt(Mathf.Pow(CurrTile.X - DestTile.X, 2) + Mathf.Pow(CurrTile.Y - DestTile.Y, 2));
        // How much distance can be travel this Update cycle
        float distThisFrame = speed * deltaTime;
        // How much is that in terms of percentage to our destination
        float percThisFrame = distThisFrame / distToTravel;

        movementPercentage += percThisFrame;

        if (movementPercentage >= 1)
        {
            // Character arrived at destination
            CurrTile = DestTile;
            movementPercentage = 0;
        }

        if (_cbCharacterChanged != null)
            _cbCharacterChanged(this);
    }
    public void SetDestination(Tile tile)
    {
        if (CurrTile.IsNeighbor(tile, true) == false)
            Debug.Log("Character::SetDestination -- Our destination tile is not a neighbor tile.");

        DestTile = tile;
    }
    public void RegisterCharacterChangedCallback(Action<Character> cb) => _cbCharacterChanged += cb;
    public void UnregisterCharacterChangedCallback(Action<Character> cb) => _cbCharacterChanged -= cb;

    private void OnJobEnded(Job job)
    {
        if (job != _myJob)
        {
            Debug.LogError("Character ended a job that is not his job. Eventually forgot to unregister something.");
            return;
        }
        _myJob = null;
    }
}
