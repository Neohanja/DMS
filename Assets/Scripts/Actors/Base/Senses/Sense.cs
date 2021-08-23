using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sense : MonoBehaviour
{
    protected static float fovCheckInterval = 0.05f; 

    protected int radius;
    protected float elapsedTime;
    protected float checkTimer;
    protected float fieldOfView; 

    public virtual void InitializeSense(int range, float checkInterval, float fov)
    {
        radius = range;
        checkTimer = checkInterval;
        fieldOfView = fov / 2f;
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
