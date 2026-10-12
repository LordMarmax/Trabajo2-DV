
// Este script va en el jugador
using Godot;

public partial class ArrastrePalanca : Node
{
    [Export] public RayCast3D RayCast;

    private PalancaEnergia _palancaActual;

    public bool EstaArrastrando => _palancaActual != null;

    public bool IntentarIniciarArrastre()
    {
        if (RayCast == null)
        {
            GD.PushWarning("ArrastrePalanca: falta asignar el RayCast.");
            return false;
        }

        RayCast.ForceRaycastUpdate();

        if (!RayCast.IsColliding())
            return false;

        Node objeto = RayCast.GetCollider() as Node;

        if (objeto == null)
            return false;

        // Busca el controlador en los padres de la manija.
        PalancaEnergia palanca = BuscarPalanca(objeto);

        if (palanca == null || palanca.EstaBloqueada)
            return false;

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
