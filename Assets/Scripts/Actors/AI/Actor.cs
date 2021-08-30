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

    protected bool busy;
    public bool IsBusy { get { return busy; } }

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
        actorController = actorMain.AddComponent<Movement>();


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
        stateMachine.AddState(State.StateID.Idle, new Idle(this, actorController));
        stateMachine.AddState(State.StateID.Wander, new Wander(this, actorController));
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
        actorRender.material.color = actorStats.stats.race.colorTone;

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