using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Input;

namespace DungeonSlime;

public static class GameController
{
    private static KeyboardInfo Keyboard => Core.Input.Keyboard;
    private static GamePadInfo GamePad => Core.Input.GamePads[(int)PlayerIndex.One];

    public static bool MoveUp()
    {
        return Keyboard.WasKeyJustPressed(Keys.Up) ||
               Keyboard.WasKeyJustPressed(Keys.W) ||
               GamePad.WasButtonJustPressed(Buttons.DPadUp) ||
               GamePad.WasButtonJustPressed(Buttons.LeftThumbstickUp);
    }

    public static bool MoveDown()
    {
        return Keyboard.WasKeyJustPressed(Keys.Down) ||
               Keyboard.WasKeyJustPressed(Keys.S) ||
               GamePad.WasButtonJustPressed(Buttons.DPadDown) ||
               GamePad.WasButtonJustPressed(Buttons.LeftThumbstickDown);
    }

    /// <summary>
    /// Returns true if the player has triggered the "move left" action.
    /// </summary>
    public static bool MoveLeft()
    {
        return Keyboard.WasKeyJustPressed(Keys.Left) ||
               Keyboard.WasKeyJustPressed(Keys.A) ||
               GamePad.WasButtonJustPressed(Buttons.DPadLeft) ||
               GamePad.WasButtonJustPressed(Buttons.LeftThumbstickLeft);
    }

    public static bool MoveRight()
    {
        return Keyboard.WasKeyJustPressed(Keys.Right) ||
               Keyboard.WasKeyJustPressed(Keys.D) ||
               GamePad.WasButtonJustPressed(Buttons.DPadRight) ||
               GamePad.WasButtonJustPressed(Buttons.LeftThumbstickRight);
    }

    public static bool Pause()
    {
        return Keyboard.WasKeyJustPressed(Keys.Escape) ||
               GamePad.WasButtonJustPressed(Buttons.Start);
    }

    public static bool Action()
    {
        return Keyboard.WasKeyJustPressed(Keys.Enter) ||
               GamePad.WasButtonJustPressed(Buttons.A);
    }
}
