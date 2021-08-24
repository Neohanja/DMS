using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smell : Sense
{
    public override void UpdateSense()
    {
        //base.UpdateSense();
    }

    public override void InitializeSense(float range, float checkInterval, float fov, Actor actor)
    {
        base.InitializeSense(range, checkInterval, fov, actor);

        myBubble = gameObject.AddComponent<CapsuleCollider>();

        myBubble.radius = range;
        myBubble.height = range;
        myBubble.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
}
