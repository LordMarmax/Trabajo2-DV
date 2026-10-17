
using Godot;

public partial class BateriaUI : CanvasLayer
{
    [Export] public RunTimer Temporizador;
    [Export] public ProgressBar BarraBateria;

    public override void _Ready()
    {
        if (Temporizador == null || BarraBateria == null)
        {
            GD.PushError(
                "BateriaUI: Faltan referencias."
            );
            return;
        }

        BarraBateria.MinValue = 0;
        BarraBateria.MaxValue = 100;
        BarraBateria.Value = 100;

        Temporizador.BateriaActualizada +=
            ActualizarBateria;
    }

    private void ActualizarBateria(float porcentaje)
    {
        porcentaje = Mathf.Clamp(porcentaje, 0f, 1f);

        BarraBateria.Value = porcentaje * 100;

        Color color;

        if (porcentaje >= 0.5f)
        {
            color = new Color(1f, 1f, 0f)
                .Lerp(
                    new Color(0f, 1f, 0f),
                    (porcentaje - 0.5f) * 2f
                );
        }
        else
        {
            color = new Color(1f, 0f, 0f)
                .Lerp(
                    new Color(1f, 1f, 0f),
                    porcentaje * 2f
                );
        }

        StyleBoxFlat estilo =
            BarraBateria.GetThemeStylebox("fill")
            .Duplicate() as StyleBoxFlat;

        if (estilo != null)
        {
            estilo.BgColor = color;

            BarraBateria.AddThemeStyleboxOverride(
                "fill",
                estilo
            );
        }
    }

    public override void _ExitTree()
    {
        if (Temporizador != null &&
            GodotObject.IsInstanceValid(Temporizador))
        {
            Temporizador.BateriaActualizada -=
                ActualizarBateria;
        }
    }
}
