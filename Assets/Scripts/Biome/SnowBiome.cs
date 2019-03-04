using UnityEngine;

public class SnowBiome : BaseBiome
{
    public override GameObject GetObjectForPosition(Vector3 position)
    {
        return PrefabManager.GetPrefab(PrefabType.Snow);
    }
}