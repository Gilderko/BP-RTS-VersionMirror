using UnityEngine;

public class FaceTheCamera : MonoBehaviour
{

    private void LateUpdate()
    {
        transform.forward = Camera.main.transform.forward;
    }

}
