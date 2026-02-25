using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

public class ZoomwithLight : MonoBehaviour
{
    [System.Serializable]
    public class LightSettings
    {
        public Light2D lightSource; // Use Light2D if in URP 2D
        public float enterIntensity;
        public float exitIntensity;
    }

    [Header("Cameras")]
    public GameObject vCam1;
    public GameObject vCam2;

    [Header("Light Configuration")]
    public List<LightSettings> lightProfiles = new List<LightSettings>();
    public float transitionDuration = 0.5f;

    private Coroutine lightCoroutine;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("get");
        if (other.CompareTag("Player"))
        {
            ToggleCameras(false, true);
            StartLightTransition(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ToggleCameras(true, false);
            StartLightTransition(false);
        }
    }

    private void ToggleCameras(bool cam1State, bool cam2State)
    {
        if (vCam1) vCam1.SetActive(cam1State);
        if (vCam2) vCam2.SetActive(cam2State);
    }

    private void StartLightTransition(bool entering)
    {
        if (lightCoroutine != null) StopCoroutine(lightCoroutine);
        lightCoroutine = StartCoroutine(FadeLights(entering));
    }

    private IEnumerator FadeLights(bool entering)
    {
        float elapsed = 0f;
        int count = lightProfiles.Count;

        // Store the starting intensity of every light at the moment of trigger
        float[] startIntensities = new float[count];
        for (int i = 0; i < count; i++)
        {
            startIntensities[i] = lightProfiles[i].lightSource.intensity;
        }

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / transitionDuration);

            for (int i = 0; i < count; i++)
            {
                var profile = lightProfiles[i];
                float target = entering ? profile.enterIntensity : profile.exitIntensity;
                profile.lightSource.intensity = Mathf.Lerp(startIntensities[i], target, t);
            }
            yield return null;
        }

        // Finalize values
        foreach (var profile in lightProfiles)
        {
            profile.lightSource.intensity = entering ? profile.enterIntensity : profile.exitIntensity;
        }
    }
}