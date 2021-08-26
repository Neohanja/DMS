using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sense : MonoBehaviour
{
    protected float fovCheckInterval;
    protected Actor identity;
    protected CapsuleCollider myBubble;

    protected float radius;
    protected float fieldOfView;

    public SensoryTrigger SensoryTriggered;

    public virtual void InitializeSense(float range, float fov, Actor actor)
    {
        radius = range;
        fieldOfView = fov;
        fovCheckInterval = fov / 3f;
        identity = actor;

        myBubble = gameObject.AddComponent<CapsuleCollider>();

        myBubble.radius = range;
        myBubble.height = range;
        myBubble.isTrigger = true;
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