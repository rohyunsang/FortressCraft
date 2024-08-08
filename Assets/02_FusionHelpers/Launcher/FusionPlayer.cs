using System;
using Fusion;
using UnityEngine;

namespace FusionHelpers
{
	/// <summary>
	/// Base class for an object that will exist in exactly one instance per player.
	/// This *could* be the players avatar (visual game object), but it need not be - it's perfectly valid to just treat this as a data placeholder for the player.
	/// Implement the abstract InitNetworkState to initialize networked data on the State Authority.
	/// </summary>

	public abstract class FusionPlayer : NetworkBehaviour
	{

		// This is set by FusionSession when the player spawns and should not be used by the application. Use PlayerIndex instead.
		[Networked] public int NetworkedPlayerIndex { private get; set; }

		// These are local properties so they remain valid when the network state goes away (also, they don't change during the life of the NO).
		public PlayerRef PlayerId { get; private set; } = PlayerRef.None;
		public int PlayerIndex { get; private set; } = -1;

		private TickAlignedEventRelay _eventStub;

		public override void Spawned()
		{
			// Getting this here because it will revert to -1 if the player disconnects, but we still want to remember the Id we were assigned for clean-up purposes
			PlayerId = Object.InputAuthority;
			PlayerIndex = NetworkedPlayerIndex;

			// The EventRelay only makes sense in shared mode, and most games don't support both hosted and shared,
			// so this particular check is very specific to Tanknarok.
			if (Runner.Topology == Topologies.Shared)
			{
				//if(HasStateAuthority)
				// _eventStub = eventRelay;
			}
			else
			{
				// For hosted mode, we avoid spawning the event relay since it doesn't really do anything except calling the local peer.
				// _eventStub = gameObject.AddComponent<TickAlignedEventRelay>();
			}

			Debug.Log($"Spawned Player with InputAuth {PlayerId}, Index {PlayerIndex}");

			Runner.WaitForSingleton<FusionSession>(session => session.AddPlayerAvatar(this));
		}

		public override void Despawned(NetworkRunner runner, bool hasState)
		{
			Debug.Log($"Despawned Player with InputAuth {PlayerId}, Index {PlayerIndex}");

			Debug.Log("Despawned is RUN?");
			Runner.WaitForSingleton<FusionSession>(session => session.RemovePlayerAvatar(this));
		}


		public abstract void InitNetworkState();
	}
}