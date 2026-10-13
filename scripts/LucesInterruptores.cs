
using Godot;

public partial class LucesInterruptores : Node3D
{
    [Signal]
    public delegate void TodosInterruptoresActivadosEventHandler();

    [Signal]
    public delegate void EstadoInterruptoresCambiadoEventHandler(bool todosActivados);

    private bool ultimoEstadoCompleto = false;

    [Export] public MeshInstance3D[] Luces;

    [Export] public Material MaterialApagado;
    [Export] public Material MaterialRojo;
    [Export] public Material MaterialVerde;
    [Export] public InterruptorElectrico[] Interruptores;
    private bool[] preparadas;
    private bool[] activadas;

    public override void _Ready()
    {
        preparadas = new bool[Luces.Length];
        activadas = new bool[Luces.Length];

        for (int i = 0; i < Luces.Length; i++)
            ActualizarLuz(i);

        foreach (InterruptorElectrico interruptor in Interruptores)
        {
            if (interruptor == null)
                continue;

            interruptor.ConfigurarLuces(this);
        }
    }

    public bool EstaPreparada(int indice)
    {
        return IndiceValido(indice) && preparadas[indice];
    }

    public void EncenderLuzRoja(int indice)
    {
        if (!IndiceValido(indice))
            return;

        preparadas[indice] = true;
        ActualizarLuz(indice);
    }

    public void CambiarLuzVerde(int indice, bool encendida)
    {
        if (!EstaPreparada(indice))
            return;

        activadas[indice] = encendida;

        ActualizarLuz(indice);

        ComprobarInterruptores();
    }

    private bool IndiceValido(int indice)
    {
        return Luces != null &&
               indice >= 0 &&
               indice < Luces.Length;
    }

    private void ActualizarLuz(int indice)
    {
        MeshInstance3D luz = Luces[indice];

        if (luz == null)
            return;

        if (activadas[indice])
            luz.MaterialOverride = MaterialVerde;
        else if (preparadas[indice])
            luz.MaterialOverride = MaterialRojo;
        else
            luz.MaterialOverride = MaterialApagado;
    }

    public bool EstanTodosActivados()
    {
        if (activadas == null || activadas.Length == 0)
            return false;

        foreach (bool activado in activadas)
        {
            if (!activado)
                return false;
        }

        return true;
    }

    private void ComprobarInterruptores()
    {
        bool todosActivados = EstanTodosActivados();

        // Evitar emitir señales repetidas sin cambios.
        if (todosActivados == ultimoEstadoCompleto)
            return;

        ultimoEstadoCompleto = todosActivados;

        EmitSignal(
            SignalName.EstadoInterruptoresCambiado,
            todosActivados
        );

        if (todosActivados)
        {
            GD.Print("¡Todos los interruptores activados!");

            EmitSignal(
                SignalName.TodosInterruptoresActivados
            );
        }
        else
        {
            GD.Print("Sistema de interruptores incompleto.");
        }
    }
}
