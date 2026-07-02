using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace DungeonSlime.GameObjects;

public class Slime(AnimatedSprite sprite)
{
    private static readonly TimeSpan s_movementTime = TimeSpan.FromMilliseconds(200);
    private TimeSpan _movementTimer;
    private float _movementProgress;
    private Vector2 _nextDirection;
    private float _stride;
    private List<SlimeSegment> _segments;
    private AnimatedSprite _sprite = sprite;

    public event EventHandler BodyCollision;

    public void Initialize(Vector2 startingPosition, float stride)
    {
        _segments = [];
        _stride = stride;

        SlimeSegment head = new()
        {
            At = startingPosition,
            To = startingPosition + new Vector2(_stride, 0),
            Direction = Vector2.UnitX
        };

        _segments.Add(head);
        _nextDirection = head.Direction;
        _movementTimer = TimeSpan.Zero;
    }

    private void HandleInput()
    {
        Vector2 potentialNextDirection = _nextDirection;

        if (GameController.MoveUp())
        {
            potentialNextDirection = -Vector2.UnitY;
        }
        else if (GameController.MoveDown())
        {
            potentialNextDirection = Vector2.UnitY;
        }
        else if (GameController.MoveLeft())
        {
            potentialNextDirection = -Vector2.UnitX;
        }
        else if (GameController.MoveRight())
        {
            potentialNextDirection = Vector2.UnitX;
        }

        float dot = Vector2.Dot(potentialNextDirection, _segments[0].Direction);
        if (dot >= 0)
        {
            _nextDirection = potentialNextDirection;
        }
    }

    private void Move()
    {
        SlimeSegment head = _segments[0];
        head.Direction = _nextDirection;
        head.At = head.To;
        head.To = head.At + head.Direction * _stride;
        _segments.Insert(0, head);
        _segments.RemoveAt(_segments.Count - 1);

        for (int i = 1; i < _segments.Count; i++)
        {
            SlimeSegment segment = _segments[i];

            if (head.At == segment.At)
            {
                BodyCollision?.Invoke(this, EventArgs.Empty);

                return;
            }
        }
    }

    public void Grow()
    {
        SlimeSegment tail = _segments[^1];
        SlimeSegment newTail = new()
        {
            At = tail.To + tail.ReverseDirection * _stride,
            To = tail.At
        };
        newTail.Direction = Vector2.Normalize(tail.At - newTail.At);
        _segments.Add(newTail);
    }

    public void Update(GameTime gameTime)
    {
        _sprite.Update(gameTime);
        HandleInput();
        _movementTimer += gameTime.ElapsedGameTime;

        if (_movementTimer >= s_movementTime)
        {
            _movementTimer -= s_movementTime;
            Move();
        }
        _movementProgress = (float)(_movementTimer.TotalSeconds / s_movementTime.TotalSeconds);
    }

    public void Draw()
    {
        foreach (SlimeSegment segment in _segments)
        {
            var pos = Vector2.Lerp(segment.At, segment.To, _movementProgress);
           _sprite.Draw(Core.SpriteBatch, pos);
        }
    }

    public Circle GetBounds()
    {
        SlimeSegment head = _segments[0];
        var pos = Vector2.Lerp(head.At, head.To, _movementProgress);

        Circle bounds = new(
            (int)(pos.X + (_sprite.Width * 0.5f)),
            (int)(pos.Y + (_sprite.Height * 0.5f)),
            (int)(_sprite.Width * 0.5f)
        );

        return bounds;
    }
}
