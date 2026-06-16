using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Input;
using MonoGameLibrary.Scenes;
using MonoGameLibrary.Audio;

namespace MonoGameLibrary;

public class Core : Game
{
    internal static Core instance;
    public static Core Instance => instance;
    public static GraphicsDeviceManager Graphics { get; private set; }
    public static new GraphicsDevice GraphicsDevice { get; private set; }
    public static SpriteBatch SpriteBatch { get; private set; }
    public static new ContentManager Content { get; private set; }
    public static InputManager Input { get; private set; }
    public static bool ExitOnEscape { get; set; }
    private static Scene _activeScene;
    private static Scene _nextScene;
    public static AudioController Audio { get; private set; }

    public Core(string title, int width, int height, bool fullScreen)
    {
        if (instance != null)
        {
            throw new InvalidOperationException($"Only a single Core instance can be created");
        }

        instance = this;

        Graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = width,
            PreferredBackBufferHeight = height,
            IsFullScreen = fullScreen
        };

        Graphics.ApplyChanges();

        Window.Title = title;

        Content = base.Content;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();

        GraphicsDevice = base.GraphicsDevice;
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        Input = new();
        Audio = new();
    }
    protected override void UnloadContent()
    {
        // Dispose of the audio controller.
        Audio.Dispose();

        base.UnloadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        Audio.Update();
        // Update the input manager.
        Input.Update(gameTime);

        // if (ExitOnEscape && Input.Keyboard.IsKeyDown(Keys.Escape))
        // {
        //     Exit();
        // }

        if (_nextScene != null)
        {
            TransitionScene();
        }

        _activeScene?.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _activeScene?.Draw(gameTime);

        base.Draw(gameTime);
    }

    public static void ChangeScene(Scene next)
    {
        if (_activeScene != next)
        {
            _nextScene = next;
        }        
    }

    private static void TransitionScene()
    {
        _activeScene?.Dispose();

        GC.Collect();
        _activeScene = _nextScene;
        _nextScene = null;
        _activeScene?.Initialize();
    }
}
