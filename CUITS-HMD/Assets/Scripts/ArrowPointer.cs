using UnityEngine;

public class ArrowPointer : MonoBehaviour
{
    // Target to point towards
    public Transform target;
    public Vector3 targetPos;
    public Vector3 myPos;

    private TSSCommunicator TSS;

    void Start(){
        TSS = FindObjectOfType(typeof(TSSCommunicator)) as TSSCommunicator;
    }

    void Update()
    {
        targetPos = target.position;
        //myPos = transform.position;

        float x = 0;
        float y = 0;
        
        TSS.SendCommand(0, 17, 0);
        x = TSS.LastOutputData;

        TSS.SendCommand(0, 18, 0);
        y = TSS.LastOutputData;

        myPos = new Vector3(x, y, transform.position.z);

        

        // Ensure the target is set
        if (target != null)
        {
            // Calculate the direction vector from the arrow to the target
            Vector3 direction = targetPos - myPos;

            // Use Quaternion.LookRotation to rotate the arrow towards the target
            Vector3 upwards = Vector3.forward;
            Quaternion rotation = Quaternion.LookRotation(direction, upwards);

            // Apply the rotation, ensuring the arrow points correctly along its forward axis
            transform.rotation = rotation;
        }
    }
}
