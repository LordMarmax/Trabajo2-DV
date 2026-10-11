
using Godot;

public partial class FusiblePortable : RigidBody3D
{
	[Signal]
	public delegate void InstalledEventHandler();

	public bool IsHeld { get; set; } = false;
	public bool IsInstalled { get; private set; } = false;

	public override void _Ready()
	{
		AddToGroup("fusibles_portables");
	}

	public void MarkInstalled()
	{
		if (IsInstalled)
			return;

		IsInstalled = true;
		IsHeld = false;

		// Avisar al ObjectGrabber.
		EmitSignal(SignalName.Installed);
	}
}
