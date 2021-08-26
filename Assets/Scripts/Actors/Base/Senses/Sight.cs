using UnityEngine;

public class Sight : Sense
{
    Vector3 offset = new Vector3(0, 0.1f, 0);

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == "Chunk") return;

        Vector3 rayDirection = other.transform.position - transform.position;
        if(Vector3.Angle(rayDirection, transform.parent.transform.forward) < fieldOfView)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position + offset, rayDirection + offset, out hit, radius))
            {
                if (hit.collider.gameObject.name == "Chunk") return;
            }

            EntityType otherActor = other.gameObject.GetComponent<EntityType>();

            if (otherActor != null)
            {
                Debug.Log(transform.parent.gameObject.name + " sees " + other.gameObject.name + "!");
            }
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 frontRay = transform.forward * radius + offset;

        for (float i = -fieldOfView; i <= fieldOfView; i += fovCheckInterval)
        {
            Debug.DrawRay(transform.position + offset, Quaternion.Euler(0, i, 0) * frontRay, Color.red);
        }
    }
}
