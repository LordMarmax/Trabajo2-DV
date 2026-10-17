
using Godot;

public partial class TransicionPantalla : CanvasLayer
{
    [Signal]
    public delegate void DesvanecimientoTerminadoEventHandler();

    [Export] public ColorRect FondoNegro;

    [Export] public float Duracion = 1.5f;

    private bool _transicionando = false;

    public override void _Ready()
    {
        if (FondoNegro == null)
        {
            GD.PushError("TransicionPantalla: Falta FondoNegro.");
            return;
        }

        FondoNegro.Visible = true;
        FondoNegro.Modulate = new Color(1, 1, 1, 0);
        FondoNegro.MouseFilter = Control.MouseFilterEnum.Ignore;
    }
    public void DesvanecerANegro()
    {
        if (_transicionando || FondoNegro == null)
            return;

        _transicionando = true;

        GD.Print("Iniciando desvanecimiento a negro...");

        Tween tween = CreateTween();

        tween.TweenProperty(
            FondoNegro,
            "modulate:a",
            1.0f,
            Duracion
        );

        tween.Finished += () =>
        {
            GD.Print("Pantalla completamente negra.");

            EmitSignal(
                SignalName.DesvanecimientoTerminado
            );
        };
    }

}
