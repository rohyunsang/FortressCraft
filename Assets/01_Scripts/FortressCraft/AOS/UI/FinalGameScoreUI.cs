using Agit.Utility;
using FusionHelpers;
using UnityEngine;
using TMPro;

namespace Agit.FortressCraft
{
	public class FinalGameScoreUI : MonoBehaviour
	{
		[SerializeField] private SpriteRenderer _crown;
		[SerializeField] private TextMeshPro _score;
		[SerializeField] private TextMeshPro _playerName;

		public void SetPlayerName(Player player)
		{
			_playerName.text = $"Player {player.PlayerIndex}";

		}

		public void SetScore(int newScore)
		{
			_score.text = newScore.ToString();
		}

		public void ToggleCrown(bool on)
		{
			_crown.enabled = on;
		}
	}
}