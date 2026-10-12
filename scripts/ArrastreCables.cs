using Godot;
using System;

public partial class ArrastreCables : Node3D
{
    [Export] public PuzzleCables Puzzle;
    [Export] public Camera3D Camara;
    [Export] public CableFlexible[] Cables;
    [Export] public ConectorCable[] Conectores;

    // Capa de colisión de las puntas.
    [Export(PropertyHint.Layers3DPhysics)]
    public uint MascaraPuntas = 4;

    private CableFlexible cableActivo;
    private Plane planoArrastre;
    private Vector3 desplazamientoMouse;

    public override void _Input(InputEvent @event)
    {
        if (Puzzle == null || !Puzzle.EstaAbierto)
            return;

        if (@event is InputEventMouseButton boton &&
            boton.ButtonIndex == MouseButton.Left)
        {
            if (boton.Pressed)
                IniciarArrastre();
            else
                FinalizarArrastre();
        }

        if (@event is InputEventMouseMotion &&
            cableActivo != null)
        {
            ActualizarArrastre();
        }
    }

    private void IniciarArrastre()
    {
        Vector2 mouse = GetViewport().GetMousePosition();

        Vector3 origen = Camara.ProjectRayOrigin(mouse);
        Vector3 direccion = Camara.ProjectRayNormal(mouse);

        var consulta = PhysicsRayQueryParameters3D.Create(
            origen,
            origen + direccion * 100.0f
        );

        consulta.CollisionMask = MascaraPuntas;
        consulta.CollideWithBodies = true;
        consulta.CollideWithAreas = false;

        var resultado = GetWorld3D()
            .DirectSpaceState
            .IntersectRay(consulta);

        if (resultado.Count == 0)
            return;

        StaticBody3D cuerpo =
            resultado["collider"].AsGodotObject()
            as StaticBody3D;

        if (cuerpo == null)
            return;

        foreach (CableFlexible cable in Cables)
        {
            if (cable == null || cable.EstaConectado)
                continue;

            if (cable.Punta == cuerpo)
            {
                cableActivo = cable;
                break;
            }
        }

        if (cableActivo == null)
            return;

        // Plano paralelo a la vista de la cámara,
        // pasando por el punto de unión de la punta.
        Vector3 normal = -Camara.GlobalBasis.Z;

        planoArrastre = new Plane(
            normal,
            cableActivo.Union.GlobalPosition
        );

        Vector3? puntoMouse =
            ObtenerPuntoMouseEnPlano();

        if (puntoMouse.HasValue)
        {
            // Evita que la punta salte al centro del cursor.
            desplazamientoMouse =
                cableActivo.Union.GlobalPosition -
                puntoMouse.Value;
        }
        else
        {
            cableActivo = null;
            return;
        }

        GD.Print("Arrastrando cable: " +
            cableActivo.Name);
    }

    private void ActualizarArrastre()
    {
        Vector3? punto = ObtenerPuntoMouseEnPlano();

        if (!punto.HasValue)
            return;

        Vector3 destino =
            punto.Value + desplazamientoMouse;

        cableActivo.MoverPunta(destino);
    }

    private Vector3? ObtenerPuntoMouseEnPlano()
    {
        Vector2 mouse = GetViewport().GetMousePosition();

        Vector3 origen = Camara.ProjectRayOrigin(mouse);
        Vector3 direccion = Camara.ProjectRayNormal(mouse);

        return planoArrastre.IntersectsRay(
            origen,
            direccion
        );
    }


    private void FinalizarArrastre()
    {
        if (cableActivo == null)
            return;

        CableFlexible cable = cableActivo;
        cableActivo = null;

        // Posición exacta del punto de unión
        // de la punta móvil.
        Vector3 posicionPunta = cable.Union.GlobalPosition;

        foreach (ConectorCable conector in Conectores)
        {
            if (conector == null)
                continue;


            bool dentro = conector.ContienePunto(posicionPunta);

            float distancia = posicionPunta.DistanceTo(
                conector.ObtenerPosicionConexion()
            );

            GD.Print(
                $"Cable: {cable.Name} | " +
                $"Conector: {conector.Name} | " +
                $"Distancia: {distancia:F3} | " +
                $"Dentro: {dentro} | " +
                $"Color cable: {cable.ColorCable} | " +
                $"Color conector: {conector.Color}"
            );

            if (!dentro)
                continue;


            // El cable está sobre un conector.
            if (conector.Color == cable.ColorCable)
            {
                cable.Conectar(conector);

                GD.Print(
                    $"Conexión correcta: {cable.Name}"
                );

                ComprobarPuzzleCompleto();
                return;
            }

            // Está sobre un conector incorrecto.
            GD.Print(
                $"Conexión incorrecta: {cable.Name}"
            );

            cable.Desconectar();
            return;
        }

        // La punta no está dentro de ningún conector.
        GD.Print(
            $"Cable {cable.Name} fuera de las áreas. Regresando."
        );

        cable.Desconectar();
    }


    private void ComprobarPuzzleCompleto()
    {
        if (Cables == null || Cables.Length == 0)
            return;

        foreach (CableFlexible cable in Cables)
        {
            if (cable == null || !cable.EstaConectado)
                return;
        }

        GD.Print("¡Puzzle de cables completado!");
    }


}