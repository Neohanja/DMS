using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor
{
    //Manditory Game Objexts and controllers
    public GameObject actorMain;
    protected StateManager stateMachine;
    protected Movement actorController;
    protected List<GameObject> sensory;

    //Model and Physics
    protected GameObject actorModel;
    protected CapsuleCollider actorCollision;
    protected Rigidbody actorRigidbody;
    protected MeshFilter actorMesh;
    protected MeshRenderer actorRender;

    //Individual Actor data
    public EntityType actorStats;
    protected RanGen actorRNG;
    protected ActorType actorType;

    public Actor(ActorStats characterSheet)
    {
        //stats = characterSheet;        

        actorRNG = new RanGen(characterSheet.seed);
        actorType = (ActorType)characterSheet.actorType;
        actorMain = new GameObject(characterSheet.actorName);

        //Add the various components
        actorStats = actorMain.AddComponent<EntityType>();
        actorStats.stats = characterSheet;
        stateMachine = actorMain.AddComponent<StateManager>();

        //Add the Switch here for "Actor Type"
        switch(actorType)
        {
            case ActorType.Monster:
                actorController = actorMain.AddComponent<MonMovement>();
                actorStats.entityType = EntityType.EntityID.Monster;
                break;
            default:
                actorController = actorMain.AddComponent<Movement>();
                break;
        }


        SetupMesh();

        actorController.SetSpawn(actorStats.stats.SpawnPoint);
        actorController.SetStats(this, actorStats.stats);        

        AddColliders();
        BuildStateMachine();
        BuildSensoryData();
    }

    /// <summary>
    /// Where the individual actor types add their state machine libraries
    /// </summary>
    protected virtual void BuildStateMachine()
    {
        
    }

    protected virtual void BuildSensoryData()
    {
        sensory = new List<GameObject>();

        if(actorStats.stats.race.sight > 0)
        {
            GameObject sightObj = new GameObject("Sight");
            Sight sight = sightObj.AddComponent<Sight>();
            sight.InitializeSense(actorStats.stats.race.sight, 45, this);
            sightObj.transform.SetParent(actorMain.transform);
            sightObj.transform.position = actorMain.transform.position;
            sensory.Add(sightObj);
        }

        if(actorStats.stats.race.hearing > 0)
        {
            GameObject hearingObj = new GameObject("Hearing");
            Hear hearing = hearingObj.AddComponent<Hear>();
            hearing.InitializeSense(actorStats.stats.race.hearing, 45, this);
            hearingObj.transform.SetParent(actorMain.transform);
            hearingObj.transform.position = actorMain.transform.position;
            sensory.Add(hearingObj);
        }

        if(actorStats.stats.race.smell > 0)
        {
            GameObject smellObj = new GameObject("Smell");
            Smell smell = smellObj.AddComponent<Smell>();
            smell.InitializeSense(actorStats.stats.race.smell, 45, this);
            smellObj.transform.SetParent(actorMain.transform);
            smellObj.transform.position = actorMain.transform.position;
            sensory.Add(smellObj);
        }
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

        actorMesh.mesh = actorStats.stats.race.baseMesh;
        actorRender.material = AIManager.ActorManager.baseMaterial;
        actorRender.material.mainTexture = actorStats.stats.race.skins[0];

        //Temp stuff, until actual models are put in.
        actorModel.transform.position += Vector3.up * actorStats.stats.race.height;
        actorModel.transform.localScale *= actorStats.stats.race.height;
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