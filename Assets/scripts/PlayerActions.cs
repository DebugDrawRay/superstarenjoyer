using UnityEngine;
using System.Collections;
using InControl;

public class PlayerActions : PlayerActionSet
{
    public PlayerTwoAxisAction Move;

    public PlayerAction Up;
    public PlayerAction Down;
    public PlayerAction Left;
    public PlayerAction Right;

    public PlayerAction PrimaryAction;
    public PlayerAction Pause;

    public PlayerActions()
    {
        Up = CreatePlayerAction("Up");
        Down = CreatePlayerAction("Down");
        Left = CreatePlayerAction("Left");
        Right = CreatePlayerAction("Right");

        Move = CreateTwoAxisPlayerAction(Left, Right, Down, Up);

        PrimaryAction = CreatePlayerAction("Primary Action");
        Pause = CreatePlayerAction("Pause");
    }

    public static PlayerActions BindAll()
    {
        PlayerActions actions = new PlayerActions();
        actions.Up.AddDefaultBinding(Key.W);
        actions.Down.AddDefaultBinding(Key.S);
        actions.Left.AddDefaultBinding(Key.A);
        actions.Right.AddDefaultBinding(Key.D);

        actions.Up.AddDefaultBinding(InputControlType.LeftStickUp);
        actions.Down.AddDefaultBinding(InputControlType.LeftStickDown);
        actions.Left.AddDefaultBinding(InputControlType.LeftStickLeft);
        actions.Right.AddDefaultBinding(InputControlType.LeftStickRight);

        actions.PrimaryAction.AddDefaultBinding(InputControlType.Action1);
        actions.PrimaryAction.AddDefaultBinding(Key.Space);
        actions.Pause.AddDefaultBinding(Key.Escape);
        actions.Pause.AddDefaultBinding(InputControlType.Start);
        actions.Pause.AddDefaultBinding(InputControlType.Pause);
        actions.Pause.AddDefaultBinding(InputControlType.Menu);
        actions.Pause.AddDefaultBinding(InputControlType.Home);
        return actions;
    }
}
