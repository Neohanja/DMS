using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hear : Sense
{
    private void OnTriggerEnter(Collider other)
    {
        EntityType otherActor = other.gameObject.GetComponent<EntityType>();

        if(otherActor != null)
        {
            //Debug.Log(transform.parent.gameObject.name + " hears " + other.gameObject.name + "!");
        }
    }
}
