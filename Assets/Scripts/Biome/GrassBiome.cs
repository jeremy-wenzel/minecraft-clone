using UnityEngine;

public class GrassBiome : BaseBiome
{
    public override GameObject GetObjectForPosition(Vector3 position)
    {
        if (position.y > SnowStartHeight)
        {
            return PrefabManager.GetPrefab(PrefabType.Snow);
        }
        //int x = Random.Range(0, 500);
        //if (x == 1)
        //{
        //    return PrefabManager.GetPrefab(PrefabType.Tree);
        //}

        return PrefabManager.GetPrefab(PrefabType.Grass);
    }
}