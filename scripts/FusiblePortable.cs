// Este script va en el fusible Portable
using Godot;

public partial class FusiblePortable : RigidBody3D
{
    public bool IsHeld { get; set; } = false;
    public bool IsInstalled { get; private set; } = false;
    public override void _Ready()
    {
        AddToGroup("fusibles_portables");
    }

    public void MarInstalled()
    {
        IsInstalled = true;
        IsHeld = false;
    }
}