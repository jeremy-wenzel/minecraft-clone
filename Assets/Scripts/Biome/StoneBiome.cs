using UnityEngine;

public class StoneBiome : BaseBiome
{
    public override GameObject GetObjectForPosition(Vector3 position)
    {
        return PrefabManager.GetPrefab(PrefabType.Stone);
    }

    protected override float AdjustHeightIfNecessary(float newX, float newZ, float currentTotalY)
    {
        return currentTotalY * 2;
    }

    public override GameObject GetColumnCube()
    {
        return PrefabManager.GetPrefab(PrefabType.Stone);
    }

    public override GameObject GetAdjacentCube()
    {
        return GetColumnCube();
    }
}
