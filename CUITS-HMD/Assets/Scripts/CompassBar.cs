using UnityEngine;

public class UICompass : MonoBehaviour
{
    [SerializeField] Transform viewDirection;
    [SerializeField] RectTransform compassElement;
    [SerializeField] float compassSize; /*NorthXposition - SouthXposition*/

    void LateUpdate()
    {
        Vector3 forwardVector = Vector3.ProjectOnPlane(viewDirection.forward, Vector3.up).normalized;
        float forwardSignedAngle = Vector3.SignedAngle(forwardVector, Vector3.forward, Vector3.up);
        float compassOffset = (forwardSignedAngle / 180) * compassSize;
        compassElement.anchoredPosition = new Vector3(compassOffset, 0);
    }
}