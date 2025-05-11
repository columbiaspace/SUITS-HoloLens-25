using UnityEngine;

public class UICompass : MonoBehaviour
{

    public Transform viewDirection;
    public RectTransform compassElement;
    public float compassSize;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void LateUpdate()
    {
        Vector3 forwardVector = Vector3.ProjectOnPlane(viewDirection.forward, Vector3.up).normalized;
        float forwardSignedAngle = Vector3.SignedAngle(forwardVector, Vector3.forward, Vector3.up);
        float compassOffset = (forwardSignedAngle / 360f) * compassSize;
        float wrappedOffset = Mathf.Repeat(compassOffset + (compassSize / 2f), compassSize) - (compassSize / 2f);
        compassElement.anchoredPosition = new Vector3(wrappedOffset, 0);
        //compassElement.anchoredPosition = new Vector3(compassOffset, 0);
    }
}