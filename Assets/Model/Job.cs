using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// This class holds info for a queued job, which can include things like
// placing a Furniture, moving stored inventory, crafting at a workbench, ..
public class Job
{

    public Tile Tile { get; protected set; }

    float _jobTime = 1f;

    Action<Job> _cbJobComplete;
    Action<Job> _cbJobCancel;

    public Job(Tile tile, Action<Job> cbJobComplete, float jobTime = 1f)
    {
        Tile = tile;
        _cbJobComplete += cbJobComplete;
        _jobTime = jobTime;
        Tile.PendingFurnitureJob = this;
    }

    public void RegisterJobCompleteCallback(Action<Job> cb) => _cbJobComplete += cb;
    public void RegisterJobCancelCallback(Action<Job> cb) => _cbJobCancel += cb;

    public void DoWork(float workTime)
    {
        _jobTime -= workTime;

        if (_jobTime <= 0 && _cbJobComplete != null)
            _cbJobComplete(this);
    }

    public void CancelJob()
    {
        if (_cbJobCancel != null)
            _cbJobCancel(this);
    }
}


