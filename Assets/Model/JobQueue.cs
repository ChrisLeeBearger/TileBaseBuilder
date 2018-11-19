using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class JobQueue
{
    protected Queue<Job> _jobQueue;

    Action<Job> cbJobCreated;

    public JobQueue()
    {
        _jobQueue = new Queue<Job>();
    }

    public void Enqueue(Job job)
    {
        _jobQueue.Enqueue(job);

        if (cbJobCreated != null)
        {
            cbJobCreated(job);
        }
    }
    public Job Dequeue()
    {
        if (_jobQueue.Count == 0)
            return null;
        return _jobQueue.Dequeue();
    }

    public void RegisterJobCreationCallback(Action<Job> cb) => cbJobCreated += cb;

    public void UnregisterJobCreationCallback(Action<Job> cb) => cbJobCreated -= cb;

}
