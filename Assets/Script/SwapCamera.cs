using UnityEngine;

public class SwapCamera : MonoBehaviour
{
    public GameObject vCam1, vCam2;

    void OnPrevious()
    {
        vCam1.SetActive(true);
        vCam2.SetActive(false);
    }
    void OnNext()
    {
        vCam1.SetActive(false);
        vCam2.SetActive(true);
    }
}
