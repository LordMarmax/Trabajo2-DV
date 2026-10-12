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
}