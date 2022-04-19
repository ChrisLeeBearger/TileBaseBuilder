using System;
using System.Collections.Generic;
using System.Linq;

public class JobQueue {
    protected Queue<Job> _jobQueue;

    private Action<Job> cbJobCreated;

    public JobQueue() {
        _jobQueue = new Queue<Job>();
    }

    public void Enqueue(Job job) {
        _jobQueue.Enqueue(job);
        cbJobCreated?.Invoke(job);
    }

    public Job Dequeue() {
        return _jobQueue.Any() ? _jobQueue.Dequeue() : null;
    }

    public void RegisterJobCreationCallback(Action<Job> cb) {
        cbJobCreated += cb;
    }

    public void UnregisterJobCreationCallback(Action<Job> cb) {
        cbJobCreated -= cb;
    }
}