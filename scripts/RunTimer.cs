using Godot;

public partial class RunTimer : Node
{
    private double _tiempoTranscurrido = 0.0;
    private bool _contando = false;

    public override void _Ready()
    {
        Iniciar();
    }

    public override void _Process(double delta)
    {
        if (!_contando)
            return;

        _tiempoTranscurrido += delta;
    }

    public void Iniciar()
    {
        _tiempoTranscurrido = 0.0;
        _contando = true;

        GD.Print("=== RUN INICIADA ===");
    }

    public void Detener()
    {
        if (!_contando)
            return;

        _contando = false;

        int minutos = (int)(_tiempoTranscurrido / 60);
        double segundos = _tiempoTranscurrido % 60;

        GD.Print("=== RUN COMPLETADA ===");
        GD.Print($"Tiempo total: {minutos:00}:{segundos:00.00}");
        GD.Print($"Segundos totales: {_tiempoTranscurrido:F2}");
    }
    public void AlActivarPalanca()
    {
        Detener();
    }
}