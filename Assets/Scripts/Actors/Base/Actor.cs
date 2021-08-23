using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor
{
    //Manditory Game Objexts and controllers
    public GameObject actorMain;
    protected StateManager stateMachine;
    protected Movement actorController;
    protected GameObject actorModel;
    protected CapsuleCollider actorCollision;
    protected Rigidbody actorRigidbody;
    protected MeshFilter actorMesh;
    protected MeshRenderer actorRender;

    //Individual Actor data
    protected ActorStats stats;
    protected Racial race;
    protected RanGen actorRNG;
    protected ActorType actorType;

    public Actor(ActorStats characterSheet, Racial baseRace)
    {
        stats = characterSheet;
        race = baseRace;

        actorRNG = new RanGen(stats.seed);
        actorType = (ActorType)stats.actorType;
        actorMain = new GameObject(stats.actorName);

        //Add the various components
        stateMachine = actorMain.AddComponent<StateManager>();

        //Add the Switch here for "Actor Type"
        switch(actorType)
        {
            case ActorType.Monster:
                actorController = actorMain.AddComponent<MonMovement>();
                break;
            default:
                actorController = actorMain.AddComponent<Movement>();
                break;
        }


        SetupMesh();
        
        actorController.SetSpawn(stats.SpawnPoint);
        actorController.SetStats(this, stats);

        AddColliders();
        BuildStateMachine();
    }

    /// <summary>
    /// Where the individual actor types add their state machine libraries
    /// </summary>
    protected virtual void BuildStateMachine()
    {
        
    }

    protected virtual void AddColliders()
    {
        actorCollision = actorMain.AddComponent<CapsuleCollider>();
        actorRigidbody = actorMain.AddComponent<Rigidbody>();

        actorRigidbody.constraints = (RigidbodyConstraints)116;
        actorCollision.radius = 0.25f;
        actorCollision.center = new Vector3(0f, 0.5f, 0f);
    }

    protected virtual void SetupMesh()
    {
        actorModel = new GameObject("Model");
        actorModel.transform.SetParent(actorMain.transform);
        actorMesh = actorModel.AddComponent<MeshFilter>();
        actorRender = actorModel.AddComponent<MeshRenderer>();

        actorMesh.mesh = race.baseMesh;
        actorRender.material = AIManager.ActorManager.baseMaterial;
        actorRender.material.mainTexture = race.skins[0];

        //Temp stuff, until actual models are put in.
        actorModel.transform.position += Vector3.up * race.height;
        actorModel.transform.localScale *= race.height;
        //End Temp Stuff
    }

    public virtual int Roll(int min, int max)
    {
        return actorRNG.Roll(min, max);
    }

    public virtual float Percent()
    {
        return actorRNG.Percentage();
    }

    public Vector2Int CurrentLocation()
    {
        return new Vector2Int(MathFun.Floor(actorMain.transform.position.x), MathFun.Floor(actorMain.transform.position.z));
    }

    public enum ActorType
    {
        Monster,
        Villager,
        Mob
    }
}