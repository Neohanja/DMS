using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smell : Sense
{
    public override void InitializeSense(float range, float checkInterval, float fov, Actor actor)
    {
        base.InitializeSense(range, checkInterval, fov, actor);


    }

    protected override EntityType.EntityID EntityFound()
    {
        return base.EntityFound();
    }

    protected override void OnSenseTrigger(EntityType.EntityID entityFound)
    {

    }
}
