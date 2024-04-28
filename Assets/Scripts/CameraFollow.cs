
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform carTransform;
    private readonly Vector3 _offset = new Vector3(-11f, 18f, 0);

    private void LateUpdate()
    {
        transform.position = carTransform.position + _offset;
    }
}
