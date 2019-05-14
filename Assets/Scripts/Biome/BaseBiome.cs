using Assets.Scripts;
using UnityEngine;

public abstract class BaseBiome
{
    protected Perlin PerlinInstance => Perlin.GetInstance();
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

    protected virtual float AdjustHeightIfNecessary(float newX, float newZ, float currentTotalY)
    {
        return currentTotalY;
    }

    public Vector3 GetHeightForPosition(int startX, int startZ, int xOffset, int zOffset)
    {
        Perlin perlin = Perlin.GetInstance();
        float newX = (startX + xOffset + WorldConstants.OFFSET) / WorldConstants.SCALE_FACTOR;
        float newZ = (startZ + zOffset + WorldConstants.OFFSET) / WorldConstants.SCALE_FACTOR;

        // this essentially allows us to generate the steepness. Dividing by _worldScale
        // allows us to have plains and montains because the steepness spans over a longer distance
        float steepnessY = perlin.DoPerlin(newX / WorldConstants.WORLD_SCALE, newZ / WorldConstants.WORLD_SCALE) * WorldConstants.STEEPNESS_SCALE;
        float totalY = perlin.DoPerlin(newX, newZ) * steepnessY;
        return new Vector3(startX + xOffset, (int)AdjustHeightIfNecessary(newX, newZ, totalY), startZ + zOffset);
    }

    public abstract GameObject GetColumnCube();
}