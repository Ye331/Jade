using UnityEngine;

namespace Jade.World
{
    public class ShanhaiShardInventory2D : MonoBehaviour
    {
        [SerializeField] private int requiredShards = 4;
        [SerializeField] private int collectedShards;

        public int RequiredShards => requiredShards;
        public int CollectedShards => collectedShards;
        public bool HasAllShards => collectedShards >= requiredShards;

        public void Configure(int required)
        {
            requiredShards = Mathf.Max(1, required);
        }

        public void AddShard(string shardName)
        {
            collectedShards++;
            Debug.Log("Collected Shanhai shard " + collectedShards + "/" + requiredShards + ": " + shardName);
        }
    }
}
