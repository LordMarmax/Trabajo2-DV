
using Godot;

public partial class ObjectGrabber : Node
{
    [Export] public RayCast3D InteractionRay;
    [Export] public Marker3D HoldPoint;

    [Export] public float FollowSpeed = 12.0f;
    [Export] public float MaxSpeed = 8.0f;
    [Export] public float DropDistance = 4.0f;

    private FusiblePortable _heldFuse;

    private float _originalGravity;
    private float _originalLinearDamp;
    private float _originalAngularDamp;

    public override void _PhysicsProcess(double delta)
    {
        if (_heldFuse == null)
            return;

        // Comprobar que el fusible todavía existe.
        if (!GodotObject.IsInstanceValid(_heldFuse) ||
            _heldFuse.IsQueuedForDeletion())
        {
            _heldFuse = null;
            return;
        }

        // Si ya fue instalado, dejamos de controlarlo.
        if (_heldFuse.IsInstalled)
        {
            ReleaseInstalledFuse();
            return;
        }

        Vector3 target = HoldPoint.GlobalPosition;
        Vector3 current = _heldFuse.GlobalPosition;

        Vector3 direction = target - current;

        _heldFuse.LinearVelocity =
            (direction * FollowSpeed).LimitLength(MaxSpeed);

        _heldFuse.AngularVelocity = Vector3.Zero;

        if (direction.Length() > DropDistance)
            Drop();
    }


    public void Interact()
    {
        GD.Print("ObjectGrabber: Interact ejecutado.");

        if (_heldFuse != null)
        {
            GD.Print("Soltando fusible.");
            Drop();
            return;
        }

        InteractionRay.ForceRaycastUpdate();

        if (!InteractionRay.IsColliding())
        {
            GD.Print("RayCast: No detecta ningún objeto.");
            return;
        }

        GodotObject collider = InteractionRay.GetCollider();

        if (collider is Node node)
        {
            GD.Print($"Nodo detectado: {node.Name}");
            GD.Print($"Ruta: {node.GetPath()}");
            GD.Print($"Tipo: {node.GetType().Name}");
        }
        GD.Print($"Tipo de objeto: {collider.GetType().Name}");

        if (collider is FusiblePortable fusible)
        {
            GD.Print("¡FusiblePortable detectado!");

            Grab(fusible);
        }
        else
        {
            GD.Print("El objeto detectado no es FusiblePortable.");
        }
    }


    private void Grab(FusiblePortable fuse)
    {
        if (fuse.IsHeld || fuse.IsInstalled)
            return;

        _heldFuse = fuse;

        // Guardar propiedades físicas originales.
        _originalGravity = fuse.GravityScale;
        _originalLinearDamp = fuse.LinearDamp;
        _originalAngularDamp = fuse.AngularDamp;

        fuse.IsHeld = true;

        fuse.GravityScale = 0.0f;
        fuse.LinearDamp = 8.0f;
        fuse.AngularDamp = 10.0f;

        fuse.Sleeping = false;

        // Escuchar cuándo se instala.
        fuse.Installed += ReleaseInstalledFuse;
    }

    public void Drop()
    {
        if (_heldFuse == null)
            return;

        if (!GodotObject.IsInstanceValid(_heldFuse) ||
            _heldFuse.IsQueuedForDeletion())
        {
            _heldFuse = null;
            return;
        }

        // Dejamos de escuchar la instalación.
        _heldFuse.Installed -= ReleaseInstalledFuse;

        _heldFuse.IsHeld = false;

        // Restaurar las propiedades originales.
        _heldFuse.GravityScale = _originalGravity;
        _heldFuse.LinearDamp = _originalLinearDamp;
        _heldFuse.AngularDamp = _originalAngularDamp;

        _heldFuse = null;
    }

    private void ReleaseInstalledFuse()
    {
        if (_heldFuse == null)
            return;

        if (GodotObject.IsInstanceValid(_heldFuse))
        {
            _heldFuse.Installed -= ReleaseInstalledFuse;
        }

        // No restauramos la física porque será eliminado.
        _heldFuse = null;

        GD.Print("Fusible liberado del sistema de agarre.");
    }
}
