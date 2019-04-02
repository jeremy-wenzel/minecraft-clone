using Assets.Scripts;

public class TreeCube : Cube
{
    public override void MineCube()
    {
        // Do nothing for right now. We should just remove the cube
        DeactivateCube();
    }
}