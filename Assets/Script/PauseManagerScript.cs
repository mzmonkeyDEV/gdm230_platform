using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using static UnityEngine.AudioSettings;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Canvas))]
public class PauseManagerScript : MonoBehaviour
{
	public Canvas canvas;
    public Canvas canvas2;
    public Canvas canvas3;
    private Mobile m_Mobile;
	private bool m_IsPlaying;
	
    private void OnEnable()
    {
        m_Mobile.Enable();
        m_Mobile.Player.Pause.performed += OnPausePerformed;

    }
    private void OnDisable()
    {
        m_Mobile.Disable();
        m_Mobile.Player.Pause.performed -= OnPausePerformed;
    }
    void Awake()
	{
		m_IsPlaying = true;
		m_Mobile = new Mobile();
		//canvas = GetComponent<Canvas>();
	}

	void Update()
	{
		// if (Input.GetKeyDown(KeyCode.Escape))
		// {
		// 	canvas.enabled = !canvas.enabled;
		// 	Pause();
		// }
		// if (Input.GetKeyDown(KeyCode.Q))
		// {
		// 	QuitGame();
		// }
	}

	private void QuitGame()
	{
#if UNITY_EDITOR
		EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
	
	public void OnPausePerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        m_IsPlaying = !m_IsPlaying;
		GameManager.I?.SetPlayerControl(m_IsPlaying);
		Time.timeScale = Time.timeScale == 0 ? 1 : 0;
		canvas.enabled = !canvas.enabled;
        canvas2.enabled = !canvas2.enabled;
        canvas3.enabled = !canvas3.enabled;
    }
}
