
// Este script va en el jugador
using Godot;

public partial class ArrastrePalanca : Node
{
    [Export] public RayCast3D RayCast;

    private PalancaEnergia _palancaActual;

    public bool EstaArrastrando => _palancaActual != null;


    public bool IntentarIniciarArrastre()
    {
        GD.Print("=== INICIO DE DETECCIÓN ===");

        if (RayCast == null)
        {
            GD.PrintErr("ERROR: RayCast no asignado.");
            return false;
        }

        GD.Print("RayCast encontrado: ", RayCast.Name);

        RayCast.ForceRaycastUpdate();

        if (!RayCast.IsColliding())
        {
            GD.Print("RayCast no detecta ningún objeto.");
            return false;
        }

        Node objeto = RayCast.GetCollider() as Node;

        if (objeto == null)
        {
            GD.Print("El collider no es un nodo.");
            return false;
        }

        GD.Print("Objeto detectado: ", objeto.Name);
        GD.Print("Tipo: ", objeto.GetType().Name);
        GD.Print("Ruta: ", objeto.GetPath());

        PalancaEnergia palanca = BuscarPalanca(objeto);

        if (palanca == null)
        {
            GD.Print("No se encontró PalancaEnergia en los padres.");
            return false;
        }

        GD.Print("Palanca encontrada: ", palanca.Name);

        if (palanca.EstaBloqueada)
        {
            GD.Print("La palanca está bloqueada.");

            if (palanca.Manager != null)
            {
                if (!palanca.Manager.CablesConectados)
                {
                    palanca.Manager.MostrarFeedback("Primero debes conectar los cables.");
                }
                else if (!palanca.Manager.FusiblesCompletados)
                {
                    palanca.Manager.MostrarFeedback("Primero debes instalar los fusibles");
                }
                else if (!palanca.Manager.InterruptoresActivados)
                {
                    palanca.Manager.MostrarFeedback("Debes activar todos los interruptores.");
                }
            }
            return false;
        }

        _palancaActual = palanca;

        GD.Print("Arrastre de palanca iniciado.");
        return true;
    }


    public void Arrastrar(Vector2 movimientoMouse)
    {
        if (_palancaActual == null)
            return;

        _palancaActual.Arrastrar(movimientoMouse.Y);
    }

    public void FinalizarArrastre()
    {
        if (_palancaActual == null)
            return;

        _palancaActual = null;
        GD.Print("Arrastre de palanca finalizado.");
    }

    private PalancaEnergia BuscarPalanca(Node nodo)
    {
        while (nodo != null)
        {
            if (nodo is PalancaEnergia palanca)
                return palanca;

            nodo = nodo.GetParent();
        }

        return null;
    }
}
