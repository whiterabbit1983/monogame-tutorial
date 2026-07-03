using System;
using Gum.DataTypes;
using Gum.Forms.Controls;
using Gum.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace DungeonSlime.UI;

public class GameSceneUI : ContainerRuntime
{
    private static readonly string ScoreFormat = "SCORE: {0:D6}";
    private SoundEffect _uiSoundEffect;
    private Panel _pausePanel;
    private AnimatedButton _resumeButton;
    private Panel _gameOverPanel;
    private AnimatedButton _retryButton;
    private TextRuntime _scoreText;

    public event EventHandler ResumeButtonClick;
    public event EventHandler QuitButtonClick;
    public event EventHandler RetryButtonClick;

    public GameSceneUI()
    {
        // The game scene UI inherits from ContainerRuntime, so we set its
        // doc to fill so it fills the entire screen.
        Dock(Gum.Wireframe.Dock.Fill);

        // Add it to the root element.
        this.AddToRoot();

        // Get a reference to the content manager that was registered with the
        // GumService when it was original initialized.
        ContentManager content = GumService.Default.ContentLoader.XnaContentManager;

        // Use that content manager to load the sound effect and atlas for the
        // user interface elements
        _uiSoundEffect = content.Load<SoundEffect>("audio/ui");
        TextureAtlas atlas = TextureAtlas.FromFile(content, "images/atlas-definition.xml");

        // Create the text that will display the players score and add it as
        // a child to this container.
        _scoreText = CreateScoreText();
        AddChild(_scoreText);

        // Create the Pause panel that is displayed when the game is paused and
        // add it as a child to this container
        _pausePanel = CreatePausePanel(atlas);
        AddChild(_pausePanel.Visual);

        // Create the Game Over panel that is displayed when a game over occurs
        // and add it as a child to this container
        _gameOverPanel = CreateGameOverPanel(atlas);
        AddChild(_gameOverPanel.Visual);
    }

    private TextRuntime CreateScoreText()
    {
        TextRuntime text = new TextRuntime();
        text.Anchor(Gum.Wireframe.Anchor.TopLeft);
        text.WidthUnits = DimensionUnitType.RelativeToChildren;
        text.X = 20.0f;
        text.Y = 5.0f;
        text.UseCustomFont = true;
        text.CustomFontFile = @"fonts/04b_30.fnt";
        text.FontScale = 0.25f;
        text.Text = string.Format(ScoreFormat, 0);

        return text;
    }

    private Panel CreatePausePanel(TextureAtlas atlas)
    {
        Panel panel = new Panel();
        panel.Anchor(Gum.Wireframe.Anchor.Center);
        panel.WidthUnits = DimensionUnitType.Absolute;
        panel.HeightUnits = DimensionUnitType.Absolute;
        panel.Width = 264.0f;
        panel.Height = 70.0f;
        panel.IsVisible = false;

        TextureRegion backgroundRegion = atlas.GetRegion("panel-background");

        NineSliceRuntime background = new NineSliceRuntime();
        background.Dock(Gum.Wireframe.Dock.Fill);
        background.Texture = backgroundRegion.Texture;
        background.TextureAddress = TextureAddress.Custom;
        background.TextureHeight = backgroundRegion.Height;
        background.TextureWidth = backgroundRegion.Width;
        background.TextureTop = backgroundRegion.SourceRectangle.Top;
        background.TextureLeft = backgroundRegion.SourceRectangle.Left;
        panel.AddChild(background);

        TextRuntime text = new TextRuntime();
        text.Text = "PAUSED";
        text.UseCustomFont = true;
        text.CustomFontFile = "fonts/04b_30.fnt";
        text.FontScale = 0.5f;
        text.X = 10.0f;
        text.Y = 10.0f;
        panel.AddChild(text);

        _resumeButton = new(atlas)
        {
            Text = "RESUME"
        };
        _resumeButton.Anchor(Gum.Wireframe.Anchor.BottomLeft);
        _resumeButton.X = 9.0f;
        _resumeButton.Y = -9.0f;

        _resumeButton.Click += OnResumeButtonClicked;
        _resumeButton.GotFocus += OnElementGotFocus;

        panel.AddChild(_resumeButton);

        AnimatedButton quitButton = new(atlas)
        {
            Text = "QUIT"
        };
        quitButton.Anchor(Gum.Wireframe.Anchor.BottomRight);
        quitButton.X = -9.0f;
        quitButton.Y = -9.0f;

        quitButton.Click += OnQuitButtonClicked;
        quitButton.GotFocus += OnElementGotFocus;

        panel.AddChild(quitButton);

        return panel;
    }

    private Panel CreateGameOverPanel(TextureAtlas atlas)
    {
        Panel panel = new Panel();
        panel.Anchor(Gum.Wireframe.Anchor.Center);
        panel.WidthUnits = DimensionUnitType.Absolute;
        panel.HeightUnits = DimensionUnitType.Absolute;
        panel.Width = 264.0f;
        panel.Height = 70.0f;
        panel.IsVisible = false;

        TextureRegion backgroundRegion = atlas.GetRegion("panel-background");

        NineSliceRuntime background = new();
        background.Dock(Gum.Wireframe.Dock.Fill);
        background.Texture = backgroundRegion.Texture;
        background.TextureAddress = TextureAddress.Custom;
        background.TextureHeight = backgroundRegion.Height;
        background.TextureWidth = backgroundRegion.Width;
        background.TextureTop = backgroundRegion.SourceRectangle.Top;
        background.TextureLeft = backgroundRegion.SourceRectangle.Left;
        panel.AddChild(background);

        TextRuntime text = new()
        {
            Text = "GAME OVER",
            WidthUnits = DimensionUnitType.RelativeToChildren,
            UseCustomFont = true,
            CustomFontFile = "fonts/04b_30.fnt",
            FontScale = 0.5f,
            X = 10.0f,
            Y = 10.0f
        };
        panel.AddChild(text);

        _retryButton = new(atlas)
        {
            Text = "RETRY"
        };
        _retryButton.Anchor(Gum.Wireframe.Anchor.BottomLeft);
        _retryButton.X = 9.0f;
        _retryButton.Y = -9.0f;

        _retryButton.Click += OnRetryButtonClicked;
        _retryButton.GotFocus += OnElementGotFocus;

        panel.AddChild(_retryButton);

        AnimatedButton quitButton = new(atlas)
        {
            Text = "QUIT"
        };
        quitButton.Anchor(Gum.Wireframe.Anchor.BottomRight);
        quitButton.X = -9.0f;
        quitButton.Y = -9.0f;

        quitButton.Click += OnQuitButtonClicked;
        quitButton.GotFocus += OnElementGotFocus;

        panel.AddChild(quitButton);

        return panel;
    }

    private void OnResumeButtonClicked(object sender, EventArgs args)
    {
        Core.Audio.PlaySoundEffect(_uiSoundEffect);
        HidePausePanel();
        ResumeButtonClick?.Invoke(sender, args);
    }

    private void OnRetryButtonClicked(object sender, EventArgs args)
    {
        Core.Audio.PlaySoundEffect(_uiSoundEffect);
        HideGameOverPanel();

        RetryButtonClick?.Invoke(sender, args);
    }

    private void OnQuitButtonClicked(object sender, EventArgs args)
    {
        Core.Audio.PlaySoundEffect(_uiSoundEffect);
        HidePausePanel();
        HideGameOverPanel();

        QuitButtonClick?.Invoke(sender, args);
    }

    private void OnElementGotFocus(object sender, EventArgs args)
    {
        Core.Audio.PlaySoundEffect(_uiSoundEffect);
    }

    public void UpdateScoreText(int score)
    {
        _scoreText.Text = string.Format(ScoreFormat, score);
    }

    public void ShowPausePanel()
    {
        _pausePanel.IsVisible = true;
        _resumeButton.IsFocused = true;
        _gameOverPanel.IsVisible = false;
    }

    public void HidePausePanel()
    {
        _pausePanel.IsVisible = false;
    }

    public void ShowGameOverPanel()
    {
        _gameOverPanel.IsVisible = true;
        _retryButton.IsFocused =true;
        _pausePanel.IsVisible = false;
    }

    public void HideGameOverPanel()
    {
        _gameOverPanel.IsVisible = false;
    }

    public void Update(GameTime gameTime)
    {
        GumService.Default.Update(gameTime);
    }

    public void Draw()
    {
        GumService.Default.Draw();
    }

}