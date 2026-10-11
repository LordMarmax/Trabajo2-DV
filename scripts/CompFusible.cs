using Godot;

public partial class CompFusible : Node3D
{
    [Signal]
    public delegate void FuseInstalledEventHandler();

    [Export] public Area3D DetectionArea;
    [Export] public Node3D FusibleInstalado;

    private bool _ocupado = false;

    public override void _Ready()
    {
        // El fusible instalado comienza invisible.
        FusibleInstalado.Hide();

        // Detectamos cuando un cuerpo entra al compartimento.
        DetectionArea.BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node3D body)
    {
        // Si el compartimento está ocupado, no hacemos nada.
        if (_ocupado)
            return;

        // Verificamos que el cuerpo sea un fusible portable.
        if (body is not FusiblePortable fusible)
            return;

        // Solo aceptamos fusibles transportados y no instalados.
        if (!fusible.IsHeld || fusible.IsInstalled)
            return;

        // Instalamos el fusible.
        InstallFusible(fusible);
    }

    private void InstallFusible(FusiblePortable fusible)
    {
        // Marcamos el compartimento como ocupado.
        _ocupado = true;

        // Marcamos el fusible como instalado.
        fusible.MarkInstalled();

        // Mostramos el fusible fijo del compartimento.
        FusibleInstalado.Show();

        // Eliminamos el fusible transportado.
        fusible.QueueFree();

        // Notificamos la instalación.
        EmitSignal(SignalName.FuseInstalled);

        GD.Print("Fusible instalado correctamente.");
    }
}
