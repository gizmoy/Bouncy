using Bouncy.Scenes;
using CocosSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouncy.Entities
{
    public enum Direction : byte
    {
        Left = 0,
        Right = 1
    }

    public class Ball : CCNode
    {
        CCSprite graphic;

        public Cube PreviousCube;
        public Cube NextCube { get; set; }

        public Direction Direction { get; set; } = Direction.Right;
        public Direction NextDirection { get; set; } = Direction.Right;

        bool IsRightDirectionSet
        {
            get
            {
                return  Direction == Direction.Right;
            }
        }

        // action which chooses next cube accordingly to previous one
        CCCallFuncN chooseNextCube = new CCCallFuncN(node =>
        {
            var ball = node as Ball;
            ball.NextCube = ball.PreviousCube.GetNextCube(ball.Direction);
        });

        // action which covers previous cube with blue
        CCCallFuncN coverPreviousCubeWithBlue = new CCCallFuncN(node =>
        {
            var ball = node as Ball;
            ball.PreviousCube.ChangeGraphic("Blue cube");
        });

        // action which covers next cube with shadow 
        CCCallFuncN coverNextCubeWithShadow = new CCCallFuncN(node =>
        {
            var ball = node as Ball;
            ball.NextCube?.CoverWithShadow();
        });

        // action which updates previous cube to the next one
        CCCallFuncN updateCubes = new CCCallFuncN(node =>
        {
            var ball = node as Ball;
            if (ball.NextCube == null)
            {
               ball.GameOver();
            }
            else
            {
                var gameScene = ball.Parent.Parent as GameScene;
                gameScene.IncrementScore();
                ball.PreviousCube = ball.NextCube;
            }
        });

        // action which plays jump sound effect
        CCCallFunc playSoundEffect = new CCCallFunc(() =>
        {
            CCAudioEngine.SharedEngine.PlayEffect("Jump");
        });

        // action which chooses direction to jump at the next hop
        CCCallFuncN chooseDirection = new CCCallFuncN(node =>
        {
            var ball = node as Ball;
            if (ball.Direction != ball.NextDirection)
            {
                CCAudioEngine.SharedEngine.PlayEffect("Whip");
            }
            ball.Direction = ball.NextDirection;
        });
        
       

        public Ball(Cube cube)
        {
            // set cube
            this.PreviousCube = cube;
            this.NextCube = cube;

            // create graphic
            var filename = "Ball.png";
            graphic = new CCSprite(filename);
            graphic.BlendFunc = CCBlendFunc.NonPremultiplied;
            graphic.AnchorPoint = new CCPoint(0, 0);
            this.AddChild(graphic);

            // set position
            PositionX = this.PreviousCube.PositionWorldspace.X + (GameConfig.CUBE_WIDTH - graphic.ContentSize.Width) / 2.0f;
            PositionY = this.PreviousCube.PositionWorldspace.Y + (GameConfig.CUBE_HEIGHT * 0.75f);
        }

        async public void MoveAsync()
        {
            // distances on both axes
            var distanceX = GameConfig.MOVE_DISTANCE_X / 2.0f;
            var distanceY = GameConfig.MOVE_DISTANCE_Y;
            var x = PositionX;
            var y = PositionY;
            var duration = GameConfig.ACTION_DURATION_WITH_DELTA;
            CCPoint point;

            // create move up action
            x += (IsRightDirectionSet)
                ? + distanceX / 2.0f
                : - distanceX / 2.0f;
            y += distanceY / 2.0f;
            point = new CCPoint(x, y);
            var moveUp = new CCMoveTo(duration * 0.5f, point);

            // create move down action
            x += (IsRightDirectionSet)
                ? + distanceX / 2.0f
                : - distanceX / 2.0f;
            y -= distanceY / 2.0f;
            point = new CCPoint(x, y);
            var moveDown = new CCMoveTo(duration * 0.5f, point);

            // run actions asynchronously
            await RunActionsAsync(
                chooseNextCube,
                moveUp,
                coverPreviousCubeWithBlue,
                coverNextCubeWithShadow,
                moveDown,
                updateCubes,
                playSoundEffect,
                chooseDirection);
        }

        private void GameOver()
        {
            var gameScene = Parent.Parent as GameScene;
            gameScene.GameOver();
        }

        public void ChangeDirection()
        {
            if (NextDirection == Direction.Left)
            {
                NextDirection = Direction.Right;
            }
            else if (NextDirection == Direction.Right)
            {
                NextDirection = Direction.Left;
            }
        }

        public void Explode()
        {
            // create explosion particle system
            var explosion = new CCParticleExplosion(PositionWorldspace);
            explosion.Speed = 500.0f;
            explosion.StartSize = 30.0f;
            explosion.EndSize = 150.0f;
            explosion.TotalParticles = 200;
            explosion.AutoRemoveOnFinish = true;
            Parent.AddChild(explosion);

            // play explose sound effect
            CCAudioEngine.SharedEngine.PlayEffect("Explosion");
        }
    }
}
