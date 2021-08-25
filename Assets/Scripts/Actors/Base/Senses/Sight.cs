using UnityEngine;

public class Sight : Sense
{
    public override void InitializeSense(float range, float checkInterval, float fov, Actor actor)
    {
        base.InitializeSense(range, checkInterval, fov, actor);


    }

    protected override EntityType.EntityID EntityFound()
    {
        for (float i = -fieldOfView; i <= fieldOfView; i += fovCheckInterval)
        {
            Ray fovPointer = new Ray(transform.position + new Vector3(0, 0.1f, 0), 
                transform.forward + new Vector3(i, MathFun.PointOnCircle(i)));
        }

        return base.EntityFound();
    }

    protected override void OnSenseTrigger(EntityType.EntityID entityFound)
    {
        
    }

    private void OnDrawGizmos()
    {
        for (float i = -fieldOfView; i <= fieldOfView; i += fovCheckInterval)
        {
            Debug.DrawRay(transform.position + new Vector3(0, 0.1f, 0),
                transform.forward - new Vector3(i, transform.position.y + 0.1f, MathFun.PointOnCircle(i)), Color.red);
        }
    }
}
