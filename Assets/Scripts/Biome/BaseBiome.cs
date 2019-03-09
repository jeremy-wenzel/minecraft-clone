using UnityEngine;

public abstract class BaseBiome
{
    /// <summary>
    /// The Snow starting height for non snow biomes
    /// </summary>
    protected static int SnowStartHeight => 8;

    /// <summary>
    /// Gets the Prefab for the position.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public abstract GameObject GetObjectForPosition(Vector3 position);
}