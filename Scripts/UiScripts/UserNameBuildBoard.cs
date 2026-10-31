using UnityEngine;

public class UserNameBuildBoard : MonoBehaviour
{
   Camera mainCam;
    private void Update()
    {
        if(mainCam == null)
        {
            mainCam = FindObjectOfType<Camera>();
        }

        if (mainCam == null) return;

        transform.LookAt(mainCam.transform);
        transform.Rotate(Vector3.up * 180);

    }
}
