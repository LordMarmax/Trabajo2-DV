using Godot;
using System;

public partial class Ui : Control
{
    private bool mouseCap = true;

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            if (mouseCap)
            {
                Input.MouseMode = Input.MouseModeEnum.Visible;
                mouseCap = false;
            }
            else
            {
                Input.MouseMode = Input.MouseModeEnum.Captured;
                mouseCap = true;
            }
        }
    }    
}
