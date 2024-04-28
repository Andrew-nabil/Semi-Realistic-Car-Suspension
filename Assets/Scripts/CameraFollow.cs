
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform carTransform;
    private readonly Vector3 _offset = new Vector3(-.5f, 2.7f, -6);

    private void LateUpdate()
    {
        transform.position = carTransform.position + _offset;
    }
}
