using Fusion;
using FusionHelpers;
using TMPro;
using UnityEngine;

public class DebugUI : NetworkBehaviour
{
	[SerializeField] private TMP_Text _text;

	private int lastRTT = -1;

	void Update()
	{
		if (Runner)
		{
			if (lastRTT < 0 || Runner.Tick % 20 == 0)
				lastRTT = Mathf.RoundToInt(1000f * (float)Runner.GetPlayerRtt(PlayerRef.None));
			Runner.WaitForSingleton<FusionSession>(session => { _text.text = $"Me:{Runner.LocalPlayer} Tick:{Runner.Tick} Region:{Runner.SessionInfo.Region} RTT:{lastRTT}"; });
		}
	}
}
