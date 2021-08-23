using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public static AIManager ActorManager { private set; get; }

    public Material baseMaterial;

    public List<Vector2Int> playerBaseFlag;

    public List<Actor> testSubjects;
    public int initialMonsters;
    public ActorStats defaultStats;
    protected RanGen aiRNG;

    public Racial[] races;

    void Awake()
    {
        if(ActorManager != null && ActorManager != this)
        {
            Destroy(gameObject);
        }

        ActorManager = this;

        testSubjects = new List<Actor>();
    }

    void Start()
    {
        aiRNG = new RanGen(World.WorldMap.worldSeed);

        for(int m = 0; m < initialMonsters;++m)
        {
            int mRace = aiRNG.Roll(0, races.Length - 1);

            testSubjects.Add(new Monster(CharacterBuilder.Monster(races[mRace], mRace), races[mRace]));
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool SpaceOccupied(int x, int y)
    {
        for(int m = 0; m < testSubjects.Count; ++m)
        {
            int mX = MathFun.Floor(testSubjects[m].actorMain.transform.position.x);
            int mY = MathFun.Floor(testSubjects[m].actorMain.transform.position.z);
            if (x == mX && y == mY) return true;
        }

        return World.WorldMap.LocationBlocked(x, 1, y);
    }
}
