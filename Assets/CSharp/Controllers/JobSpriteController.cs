using System;
using System.Collections.Generic;
using UnityEngine;

// This bare-bone controller is mostly just going to piggyback on FurnitureSpriteController as we do not yet
// fully know what our job system is going to look like in the end
public class JobSpriteController : MonoBehaviour
{
    private World _world => WorldController.Instance.World;
    private FurnitureSpriteController _furnitureSpriteController;
    private Dictionary<Job, GameObject> _jobGameObjectMap;

    void Start()
    {
        _jobGameObjectMap = new Dictionary<Job, GameObject>();
        _furnitureSpriteController = GameObject.FindObjectOfType<FurnitureSpriteController>();
        _world.JobQueue.RegisterJobCreationCallback(OnJobCreated);
    }

    private void OnJobCreated(Job job)
    {
        GameObject jobGameObject = new();
        _jobGameObjectMap.Add(job, jobGameObject);

        jobGameObject.name = "JOB_" + job.JobObjectType + " " + job.Tile.X + " " + job.Tile.Y;
        jobGameObject.transform.position = new Vector3(job.Tile.X, job.Tile.Y, 0);
        jobGameObject.transform.SetParent(this.transform, true);

        SpriteRenderer spriteRenderer = jobGameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = _furnitureSpriteController.GetSpriteForFurniture(job.JobObjectType);
        spriteRenderer.sortingLayerName = "Jobs";
        // Set the sprite transparent
        spriteRenderer.color = new Color(0.8f, 1f, 0.8f, 0.3f);

        // register event callback
        job.JobCompleted += OnJobEnded;
        job.JobCanceled += OnJobEnded;
    }

    // This executes whether a job was COMPLETED or CANCELED
    // It simply removes the preview GameObject of the job
    private void OnJobEnded(object sender, EventArgs args)
    {
        Job job = sender as Job;

        job.JobCompleted -= OnJobEnded;
        job.JobCanceled -= OnJobEnded;

        GameObject jobGameObject = _jobGameObjectMap[job];
        Destroy(jobGameObject);
    }
}
