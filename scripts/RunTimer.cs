
using Godot;

public partial class RunTimer : Node
{
    [Signal]
    public delegate void BateriaActualizadaEventHandler(
        float porcentaje
    );

    [Signal]
    public delegate void BateriaAgotadaEventHandler();

    [Export] public double DuracionBateria = 90.0;

    private double _tiempoTranscurrido = 0.0;
    private bool _contando = false;
    private bool _bateriaAgotada = false;

    [Signal]
    public delegate void RunCompletadaEventHandler();

    public override void _Ready()
    {
        Iniciar();
    }

    public override void _Process(double delta)
    {
        if (!_contando)
            return;

        _tiempoTranscurrido += delta;

        double restante = Mathf.Max(
            0.0,
            DuracionBateria - _tiempoTranscurrido
        );

        float porcentaje = DuracionBateria > 0
            ? (float)(restante / DuracionBateria)
            : 0f;

        EmitSignal(
            SignalName.BateriaActualizada,
            porcentaje
        );

        if (restante <= 0 && !_bateriaAgotada)
        {
            _bateriaAgotada = true;

            GD.Print("=== BATERÍA AGOTADA ===");

            EmitSignal(SignalName.BateriaAgotada);
        }
    }

    public void Iniciar()
    {
        _tiempoTranscurrido = 0.0;
        _contando = true;
        _bateriaAgotada = false;

        EmitSignal(
            SignalName.BateriaActualizada,
            1f
        );

        GD.Print("=== RUN INICIADA ===");
        GD.Print("Batería inicial: 90 segundos");
    }

    public void Detener()
    {
        if (!_contando)
            return;

        _contando = false;

        int minutos = (int)(_tiempoTranscurrido / 60);
        double segundos = _tiempoTranscurrido % 60;

        GD.Print("=== RUN COMPLETADA ===");
        GD.Print(
            $"Tiempo total: {minutos:00}:{segundos:00.00}"
        );
        GD.Print(
            $"Segundos totales: {_tiempoTranscurrido:F2}"
        );
    }

    public void AlActivarPalanca()
    {
        if (!_contando)
            return;
        Detener();

        EmitSignal(SignalName.RunCompletada);
    }
}
