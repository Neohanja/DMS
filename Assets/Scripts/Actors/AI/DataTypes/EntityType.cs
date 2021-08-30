using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityType : MonoBehaviour
{
    public EntityID entityType;
    public ActorStats stats;

    public enum EntityID
    {
        None,
        Player,
        Mob,
        Monster,
        Villager,
    }
}
