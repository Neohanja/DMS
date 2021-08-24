using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sense : MonoBehaviour
{
    protected static float fovCheckInterval = 0.05f;
    protected Actor identity;

    protected float radius;
    protected float elapsedTime;
    protected float checkTimer;
    protected float fieldOfView;

    public SensoryTrigger SensoryTriggered;

    public virtual void InitializeSense(float range, float checkInterval, float fov, Actor actor)
    {
        radius = range;
        checkTimer = checkInterval;
        fieldOfView = fov / 90f;
        identity = actor;
    }

    void Update()
    {
        UpdateSense();
    }

    public virtual void UpdateSense()
    {
        elapsedTime += Time.deltaTime;
        
        if(elapsedTime >= checkTimer)
        {
            elapsedTime -= checkTimer;

            EntityType.EntityID seekEntityID;

            if(CheckSensory(out seekEntityID))
            {
                OnSenseTrigger(seekEntityID);
            }
        }
    }

    protected virtual bool CheckSensory(out EntityType.EntityID checkEntity)
    {
        checkEntity = EntityFound();

        return checkEntity == EntityType.EntityID.None;
    }

    protected virtual EntityType.EntityID EntityFound()
    {
        return EntityType.EntityID.None;
    }

    protected virtual void OnSenseTrigger(EntityType.EntityID entityFound)
    {

    }
}

[System.Serializable]
public class SensoryTrigger
{
    public string entityName;
    public int entityID;
    public bool isTriggered;
    public Vector3 triggerLocation;
    public EntityType triggerEntity;
}