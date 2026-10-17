
using Godot;

public partial class Ui : Control
{
    [Export] public CentralElectricaManager Manager;
    [Export] public Label LabelTareas;
    [Export] public Label LabelFeedback;

    [Export] public float DuracionMensaje = 2.5f;
    [Export] public float DuracionDesvanecimiento = 0.8f;

    private bool mouseCap = true;
    private Tween tweenFeedback;

    public override void _Ready()
    {
        LabelFeedback.Modulate = new Color(1, 1, 1, 0);

        if (Manager == null)
        {
            GD.PushWarning("UI: Manager no asignado.");
            return;
        }

        Manager.EstadoElectricoCambiado += ActualizarTareas;
        Manager.FeedbackSolicitado += MostrarFeedback;

        ActualizarTareas();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_cancel") && !@event.IsEcho())
        {
            mouseCap = !mouseCap;

            Input.MouseMode = mouseCap
                ? Input.MouseModeEnum.Captured
                : Input.MouseModeEnum.Visible;
        }
        if (Input.IsActionJustPressed("Reset"))
        {
            GetTree().ReloadCurrentScene();
        }
    }

    private void ActualizarTareas()
    {
        LabelTareas.Text =
            $"{Marca(Manager.CablesConectados)} Conectar cables\n" +
            $"{Marca(Manager.FusiblesCompletados)} Instalar fusibles\n" +
            $"{Marca(Manager.InterruptoresActivados)} Activar interruptores\n" +
            $"{Marca(Manager.PalancaActivada)} Bajar palanca";
    }

    private string Marca(bool completado)
    {
        return completado ? "✓" : "□";
    }

    public void MostrarFeedback(string mensaje)
    {
        // Cancelar animación anterior
        if (tweenFeedback != null &&
            tweenFeedback.IsRunning())
        {
            tweenFeedback.Kill();
        }

        LabelFeedback.Text = mensaje;

        // Mostrar inmediatamente
        LabelFeedback.Modulate = new Color(1, 1, 1, 1);

        tweenFeedback = CreateTween();

        // Mantener visible
        tweenFeedback.TweenInterval(DuracionMensaje);

        // Desvanecer suavemente
        tweenFeedback.TweenProperty(
            LabelFeedback,
            "modulate:a",
            0.0f,
            DuracionDesvanecimiento
        );
    }

    public override void _ExitTree()
    {
        if (Manager != null &&
            GodotObject.IsInstanceValid(Manager))
        {
            Manager.EstadoElectricoCambiado -= ActualizarTareas;
            Manager.FeedbackSolicitado -= MostrarFeedback;
        }

        if (tweenFeedback != null)
            tweenFeedback.Kill();
    }
}
