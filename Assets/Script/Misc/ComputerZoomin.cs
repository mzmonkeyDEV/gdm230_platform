using UnityEngine;

public class ComputerZoomin : MonoBehaviour
{
    public GameObject vCam1, vCam2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            vCam1.SetActive(false);
            vCam2.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            vCam1.SetActive(true);
            vCam2.SetActive(false);
        }
    }
}
