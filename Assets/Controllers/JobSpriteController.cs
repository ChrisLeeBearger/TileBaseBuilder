using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// This bare-bone controller is mostly just going to piggyback on FurnitureSpriteController as we do not yet
// fully know what our job system is going to look like in the end
public class JobSpriteController : MonoBehaviour
{
    private World _world { get { return WorldController.Instance.World; } }
    FurnitureSpriteController _furnitureSpriteController;
    private Dictionary<Job, GameObject> _jobGameObjectMap;

    void Start()
    {
        _jobGameObjectMap = new Dictionary<Job, GameObject>();
        _furnitureSpriteController = GameObject.FindObjectOfType<FurnitureSpriteController>();
        WorldController.Instance.World.JobQueue.RegisterJobCreationCallback(OnJobCreated);
    }

    private void OnJobCreated(Job job)
    {
        GameObject jobGo = new GameObject();
        _jobGameObjectMap.Add(job, jobGo);

        jobGo.name = "JOB_" + job.JobObjectType + " " + job.Tile.X + " " + job.Tile.Y;
        jobGo.transform.position = new Vector3(job.Tile.X, job.Tile.Y, 0);
        jobGo.transform.SetParent(this.transform, true);

        SpriteRenderer sr = jobGo.AddComponent<SpriteRenderer>();
        sr.sprite = _furnitureSpriteController.GetSpriteForFurniture(job.JobObjectType);
        sr.sortingLayerName = "Jobs";
        // Set the sprite transparent
        sr.color = new Color(0.8f, 1f, 0.8f, 0.3f);

        job.RegisterJobCompleteCallback(OnJobEnded);
        job.RegisterJobCancelCallback(OnJobEnded);
    }

    // This executes whether a job was COMPLETED or CANCELED
    // It simply removes the preview GameObject of the job
    private void OnJobEnded(Job job)
    {
        GameObject jobGo = _jobGameObjectMap[job];
        job.UnregisterJobCancelCallback(OnJobEnded);
        job.UnregisterJobCompleteCallback(OnJobEnded);
        Destroy(jobGo);
    }
}
