// Este script va en los cuatro compartimientos de Fusibles.
using Godot;

public partial class CompFusible : Node3D
{
    [Export]
    public Area3D DetectionArea;

    [Export]
    public Node3D FusibleInstalado;

    private bool _ocupado = false;

    public override void _Ready()
    {
        FusibleInstalado.Hide();

        // Conectamos el método OnBodyEntered al evento BodyEntered
        DetectionArea.BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node3D body)
    {
        if (_ocupado)
            return;

        if (body is not FusiblePortable fusible)
            return;

        InstallFusible(fusible);
    }

    private void InstallFusible(FusiblePortable fusible)
    {
        _ocupado = true;

        FusibleInstalado.Show();

        fusible.QueueFree();

        GD.Print("Fusible instalado correctamente.");
    }
}
