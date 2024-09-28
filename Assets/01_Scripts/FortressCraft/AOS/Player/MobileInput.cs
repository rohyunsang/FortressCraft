using Fusion;
using Agit.FortressCraft;
using UnityEngine;

public class MobileInput : MonoBehaviour
{
	[SerializeField] private RectTransform _leftJoy;
	[SerializeField] private RectTransform _leftKnob;
	private Transform _canvas;

	private void Awake()
	{
		_canvas = GetComponentInParent<Canvas>().transform;
	}

	public void OnToggleReady()
	{
		foreach (InputController ic in FindObjectsOfType<InputController>())
		{
			//if(ic.Object.HasInputAuthority)
				//ic.ToggleReady();
		}
	}

	public void OnDisconnect()
	{
		NetworkRunner runner = FindObjectOfType<NetworkRunner>();
		if (runner != null && !runner.IsShutdown)
		{
			// Calling with destroyGameObject false because we do this in the OnShutdown callback on FusionLauncher
			runner.Shutdown(false);
		}
	}

	private void SetJoy(RectTransform joy, RectTransform knob, bool active, Vector2 center, Vector2 current)
	{
		center /= _canvas.localScale.x;
		current /= _canvas.localScale.x;

		joy.gameObject.SetActive(active);
		joy.anchoredPosition = center;

		current -= center;
        // 노브가 조이스틱의 반지름을 넘어 30 더 갈 수 있도록 조정
        float maxRadius = knob.rect.width / 2 + 30;
        if (current.magnitude > maxRadius)
            current = current.normalized * maxRadius;

        knob.anchoredPosition = current;
	}

	public void SetLeft(bool active, Vector2 down, Vector2 current)
	{
		SetJoy(_leftJoy, _leftKnob, active, down, current);
	}
}
