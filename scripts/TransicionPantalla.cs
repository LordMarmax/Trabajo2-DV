
using Godot;

public partial class TransicionPantalla : CanvasLayer
{
    [Signal]
    public delegate void DesvanecimientoTerminadoEventHandler();

    [Export] public ColorRect FondoNegro;

    [ExportGroup("Colores de transición")]
    [Export] public Color ColorCeleste = new Color("#7ccdff");
    [Export] public Color ColorNegro = new Color("#000000");

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
        FondoNegro.Color = ColorNegro;
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

    public void DesvanecerACeleste()
    {
        if (_transicionando || FondoNegro == null)
            return;

        _transicionando = true;

        GD.Print("Iniciando desvanecimiento a celeste...");

        // Cambiar el color del rectángulo.
        FondoNegro.Color = ColorCeleste;

        // Asegurar que comienza transparente.
        FondoNegro.Visible = true;
        FondoNegro.Modulate = new Color(1, 1, 1, 0);

        Tween tween = CreateTween();

        tween.TweenProperty(
            FondoNegro,
            "modulate:a",
            1.0f,
            Duracion
        ).SetTrans(Tween.TransitionType.Sine)
         .SetEase(Tween.EaseType.InOut);

        tween.Finished += () =>
        {
            GD.Print("Pantalla completamente celeste.");

            EmitSignal(
                SignalName.DesvanecimientoTerminado
            );
        };
    }


}
