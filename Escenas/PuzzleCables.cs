using Godot;

public partial class PuzzleCables : Node3D
{
    [Export] public Camera3D CamaraPuzzle;

    public bool EstaAbierto { get; private set; } = false;

    private Camera3D camaraAnterior;

    public void AbrirPuzzle()
    {
        if (EstaAbierto)
            return;

        if (CamaraPuzzle == null)
        {
            GD.PushWarning("No se asignó CamaraPuzzle.");
            return;
        }

        camaraAnterior = GetViewport().GetCamera3D();

        EstaAbierto = true;

        CamaraPuzzle.MakeCurrent();

        Input.MouseMode = Input.MouseModeEnum.Visible;

        GD.Print("Puzzle de cables abierto.");
    }

    public void CerrarPuzzle()
    {
        if (!EstaAbierto)
            return;

        EstaAbierto = false;

        if (GodotObject.IsInstanceValid(camaraAnterior))
            camaraAnterior.MakeCurrent();

        Input.MouseMode = Input.MouseModeEnum.Captured;

        GD.Print("Puzzle de cables cerrado.");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!EstaAbierto)
            return;

        if (@event.IsActionPressed("ui_cancel"))
        {
            CerrarPuzzle();
            GetViewport().SetInputAsHandled();
        }
    }
}