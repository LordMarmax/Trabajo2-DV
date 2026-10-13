
using Godot;

public partial class CompFusible : Node3D
{
    [Signal]
    public delegate void FuseInstalledEventHandler();

    [Export] public Area3D DetectionArea;
    [Export] public Node3D FusibleInstalado;

    // Referencia al controlador eléctrico.
    [Export] public CentralElectricaManager Manager;

    private bool _ocupado = false;

    public bool EstaOcupado => _ocupado;

    public override void _Ready()
    {
        FusibleInstalado.Hide();

        DetectionArea.BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node3D body)
    {
        if (_ocupado)
            return;

        if (body is not FusiblePortable fusible)
            return;

        if (!fusible.IsHeld || fusible.IsInstalled)
            return;

        // No permitir la instalación sin los cables.
        if (Manager == null || !Manager.CablesConectados)
        {
            GD.Print(
                "Fusible rechazado: los cables aún no están conectados."
            );
            return;
        }

        InstallFusible(fusible);
    }

    private void InstallFusible(FusiblePortable fusible)
    {
        _ocupado = true;

        fusible.MarkInstalled();

        FusibleInstalado.Show();

        fusible.QueueFree();

        EmitSignal(SignalName.FuseInstalled);

        GD.Print("Fusible instalado correctamente.");
    }
}
