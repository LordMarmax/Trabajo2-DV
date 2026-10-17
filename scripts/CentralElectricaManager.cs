
using Godot;

public partial class CentralElectricaManager : Node
{
    [Export] public PuzzleCables CajaCables;
    [Export] public CajaFusibles CajaFusibles;
    [Export] public LucesInterruptores SistemaLuces;
    [Export] public PalancaEnergia PalancaPrincipal;
    private ArrastreCables SistemaCables;

    public bool CablesConectados { get; private set; }
    public bool InterruptoresActivados { get; private set; }

    [Signal]
    public delegate void EstadoElectricoCambiadoEventHandler();
    [Signal]
    public delegate void FeedbackSolicitadoEventHandler(string mensaje);

    public bool PalancaActivada { get; private set; } = false;

    public bool FusiblesCompletados =>
        CajaFusibles != null && CajaFusibles.EstanTodosInstalados();

    public override void _Ready()
    {
        GD.Print("Manager: Iniciando...");

        if (CajaCables == null)
        {
            GD.PushWarning("Manager: CajaCables no asignada.");
            return;
        }

        GD.Print("Manager: CajaCables encontrada.");

        SistemaCables = CajaCables.ControladorCables;

        if (SistemaCables == null)
        {
            GD.PushWarning("Manager: No se encontró ArrastreCables.");
            return;
        }

        GD.Print("Manager: ArrastreCables encontrado.");

        SistemaCables.EstadoCablesCambiado +=
            OnEstadoCablesCambiado;

        GD.Print("Manager: Señal conectada correctamente.");

        OnEstadoCablesCambiado(
            SistemaCables.TodosConectados()
        );

        if (CajaFusibles != null)
        {
            CajaFusibles.ConfigurarManager(this);

            CajaFusibles.TodosFusiblesInstalados +=
                OnTodosFusiblesInstalados;

            GD.Print("Manager: Caja de fusibles configurada.");
        }
        else
        {
            GD.PushWarning("Manager: CajaFusibles no asignada.");
        }

        if (SistemaLuces == null)
        {
            GD.PushWarning("Manager: SistemaLuces no asignado.");
        }
        else
        {
            CajaFusibles.FusibleIndividualInstalado +=
                OnFusibleIndividualInstalado;

            GD.Print("Manager: Sistema de luces configurado.");
        }

        if (SistemaLuces != null)
        {
            SistemaLuces.EstadoInterruptoresCambiado +=
                OnEstadoInterruptoresCambiado;

            InterruptoresActivados =
                SistemaLuces.EstanTodosActivados();
        }
        if (PalancaPrincipal != null)
        {
            PalancaPrincipal.Activada += OnPalancaActivada;
        }
    }

    private void OnEstadoCablesCambiado(bool completos)
    {
        if (CablesConectados == completos)
            return;

        CablesConectados = completos;

        GD.Print(
            $"Manager: Cables conectados = {CablesConectados}"
        );

        ActualizarEstadoElectrico();
    }

    private void ActualizarEstadoElectrico()
    {


        EmitSignal(
            SignalName.EstadoElectricoCambiado
        );
    }

    public override void _ExitTree()
    {
        if (SistemaCables != null &&
            GodotObject.IsInstanceValid(SistemaCables))
        {
            SistemaCables.EstadoCablesCambiado -=
                OnEstadoCablesCambiado;
        }
        if (GodotObject.IsInstanceValid(CajaFusibles))
        {
            CajaFusibles.TodosFusiblesInstalados -=
                OnTodosFusiblesInstalados;
        }
        if (SistemaLuces != null)
        {
            SistemaLuces.EstadoInterruptoresCambiado -=
                OnEstadoInterruptoresCambiado;
        }

        if (PalancaPrincipal != null)
        {
            PalancaPrincipal.Activada -= OnPalancaActivada;
        }
    }

    private void OnTodosFusiblesInstalados()
    {
        if (!CablesConectados)
            return;


        ActualizarEstadoElectrico();
    }

    private void OnFusibleIndividualInstalado(int indice)
    {
        if (!CablesConectados || SistemaLuces == null)
            return;

        SistemaLuces.EncenderLuzRoja(indice);
    }

    private void OnEstadoInterruptoresCambiado(bool todosActivados)
    {
        InterruptoresActivados = todosActivados;

        MostrarFeedback("¡Interruptores activados!");

        ActualizarEstadoElectrico();

        ActualizarEstadoPalanca();
    }
    public bool PuedeActivarPalanca()
    {
        return CablesConectados &&
               CajaFusibles != null &&
               CajaFusibles.EstanTodosInstalados() &&
               InterruptoresActivados;
    }
    private void ActualizarEstadoPalanca()
    {
        if (PalancaPrincipal == null)
            return;

        if (PuedeActivarPalanca())
        {
            PalancaPrincipal.Desbloquear();
        }
        else
        {
            PalancaPrincipal.Bloquear();
        }
    }

    private void OnPalancaActivada()
    {
        PalancaActivada = true;

        MostrarFeedback("¡Central eléctrica activada!");

        ActualizarEstadoElectrico();
    }

    public void MostrarFeedback(string mensaje)
    {
        EmitSignal(SignalName.FeedbackSolicitado, mensaje);
    }
}
