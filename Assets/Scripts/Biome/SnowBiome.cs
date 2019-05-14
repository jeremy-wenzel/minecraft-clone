using UnityEngine;

public class SnowBiome : BaseBiome
{
    public override GameObject GetObjectForPosition(Vector3 position)
    {
        return PrefabManager.GetPrefab(PrefabType.Snow);
    }

    public override GameObject GetColumnCube()
    {
        return PrefabManager.GetPrefab(PrefabType.Dirt);
    }

    public override GameObject GetAdjacentCube()
    {
        return GetColumnCube();
    }
}