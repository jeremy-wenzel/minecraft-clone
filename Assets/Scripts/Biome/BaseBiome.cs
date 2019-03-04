using UnityEngine;

public abstract class BaseBiome
{
    protected static int SnowStartHeight => 8;

    public abstract GameObject GetObjectForPosition(Vector3 position);
}