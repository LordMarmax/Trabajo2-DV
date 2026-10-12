
using Godot;

public partial class ConectorCable : Area3D
{
    public enum ColorCable
    {
        Verde,
        Rojo,
        Amarillo,
        Azul
    }

    [Export] public ColorCable Color;
    [Export] public CollisionShape3D ZonaConexion;

    public Vector3 ObtenerPosicionConexion()
    {
        return ZonaConexion.GlobalPosition;
    }

    public bool ContienePunto(Vector3 puntoGlobal)
    {
        if (ZonaConexion == null || ZonaConexion.Shape == null)
            return false;

        // Convertir el punto global a coordenadas
        // locales del CollisionShape3D.
        Vector3 puntoLocal =
            ZonaConexion.ToLocal(puntoGlobal);

        Shape3D forma = ZonaConexion.Shape;

        // Conectores esféricos.
        if (forma is SphereShape3D esfera)
        {
            return puntoLocal.LengthSquared() <=
                esfera.Radius * esfera.Radius;
        }

        // Conectores con forma de caja.
        if (forma is BoxShape3D caja)
        {
            Vector3 mitad = caja.Size * 0.5f;

            return Mathf.Abs(puntoLocal.X) <= mitad.X &&
                   Mathf.Abs(puntoLocal.Y) <= mitad.Y &&
                   Mathf.Abs(puntoLocal.Z) <= mitad.Z;
        }

        GD.PushWarning(
            $"Forma de colisión no soportada: {forma.GetType().Name}"
        );

        return false;
    }
}
