using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// This class holds info for a queued job, which can include things like
// placing a Furniture, moving stored inventory, crafting at a workbench, ..
public class Job
{
    float _jobTime = 1f;

    public Tile Tile { get; protected set; }

    public string JobObjectType { get; protected set; }

    public event EventHandler JobCompleted;
    public event EventHandler JobCanceled;

    public Job(Tile tile, string jobObjectType, EventHandler cbJobComplete, float jobTime = 1f)
    {
        Tile = tile;
        JobCompleted += cbJobComplete;
        _jobTime = jobTime;
        JobObjectType = jobObjectType;
        Tile.PendingFurnitureJob = this;
    }

    private void OnJobCompleted() => JobCompleted?.Invoke(this, EventArgs.Empty);
    private void OnJobCanceled() => JobCanceled?.Invoke(this, EventArgs.Empty);

    public void DoWork(float workTime)
    {
        _jobTime -= workTime;

        if (_jobTime <= 0)
            OnJobCompleted();
    }

    public void CancelJob() {
        OnJobCanceled();
    }
}