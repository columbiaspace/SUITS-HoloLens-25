using UnityEngine;

public class ArrowPointer : MonoBehaviour
{
    // Target to point towards
    public Transform target;

    void Update()
    {
        //print("update");
        // Ensure the target is set
        if (target != null)
        {
            // Calculate the direction vector from the arrow to the target
            Vector3 direction = target.position - transform.position;
            //direction.x = 90;
            //print(direction);

            // Use Quaternion.LookRotation to rotate the arrow towards the target
            Vector3 upwards = Vector3.forward;
            Quaternion rotation = Quaternion.LookRotation(direction, upwards);
            //print(rotation);

            // Apply the rotation, ensuring the arrow points correctly along its forward axis
            transform.rotation = rotation;
        }
    }
}
