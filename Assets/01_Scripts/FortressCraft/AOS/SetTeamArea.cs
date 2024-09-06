using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SearchService;


namespace Agit.FortressCraft
{
    public class SetTeamArea : MonoBehaviour
    {
        public Team team;

        private float timeBetweenCalls = 1.0f; // Time in seconds between function calls
        private Dictionary<Player, float> lastCallTime; // To track the last call time for each player

        private void Start()
        {
            lastCallTime = new Dictionary<Player, float>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.transform.parent != null)
            {
                Player player = other.transform.parent.GetComponent<Player>();
                if (player != null)
                {
                    player.RPC_ChangeHpBarColor(team);
                }
            }
        }
        public void OnTriggerStay2D(Collider2D other)
        {
            if (other.transform.parent != null)
            {
                Player player = other.transform.parent.GetComponent<Player>();
                if (player != null)
                {
                    // Check if the function can be called for this player
                    if (!lastCallTime.ContainsKey(player) || Time.time >= lastCallTime[player] + timeBetweenCalls)
                    {
                        player.RPC_ChangeHpBarColor(team);
                        // Update the time of the last call
                        lastCallTime[player] = Time.time;
                    }
                }
            }
        }
    }
}
