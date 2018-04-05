using Bouncy.Entities;
using Bouncy.Structures;
using CocosSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouncy.Scenes
{
    public class GameScene : CCScene
    {
        static Random random = new Random(Guid.NewGuid().GetHashCode());

        CCLayer backgroundLayer;
        CCLayer gameplayLayer;
        CCLabel scoreLabel;

        CubeMatrix cubeMatrix;
        Ball ball;
        
        bool isFirstTouch = true;
        int score = 0;


        public GameScene(CCGameView gameView) : base(gameView)
        {
            CreateLayers();

            CreateBackground();

            CreateGameplay();

            CreateTouchListener();
        }

        private void CreateLayers()
        {
            backgroundLayer = new CCLayer();
            this.AddLayer(backgroundLayer);

            gameplayLayer = new CCLayer();
            this.AddLayer(gameplayLayer);
        }

        private void CreateBackground()
        {
            var background = new CCSprite("Background.png");
            background.AnchorPoint = new CCPoint(0, 0);
            backgroundLayer.AddChild(background);
        }

        private void CreateGameplay()
        {
            CreateCubes();

            CreateBall();

            CreateScore();
        }

        private void CreateCubes()
        {
            cubeMatrix = new CubeMatrix();
            gameplayLayer.AddChild(cubeMatrix);
        }

        private void CreateBall()
        {
            ball = new Ball(cubeMatrix[0].FirstOrDefault());
            gameplayLayer.AddChild(ball);
        }

        private void CreateScore()
        {
            scoreLabel = new CCLabel(String.Format("{0}", score), "MarkerFelt", 22);
            scoreLabel.PositionX = GameConfig.SCORE_LABEL_POSITION_X;
            scoreLabel.PositionY = GameConfig.SCORE_LABEL_POSITION_Y;
            gameplayLayer.AddChild(scoreLabel);
        }

        private void CreateTouchListener()
        {
            var touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesBegan = HandleTouchesBegan;
            gameplayLayer.AddEventListener(touchListener);
        }

        private void HandleTouchesBegan(List<CCTouch> touches, CCEvent ccevent)
        {
            if (touches.Count > 0)
            {
                if(isFirstTouch)
                {
                    // schedule movement
                    var duration = GameConfig.ACTION_DURATION;
                    ScheduleOnce(t => Move(), 0.0f);
                    Schedule(t => Move(), duration);

                    // update flag
                    isFirstTouch = false;
                }
                else
                {
                    // change direction of ball
                    ball.ChangeDirection();
                }
            }
        }

        private void Move()
        {
            ball.MoveAsync();
            cubeMatrix.MoveAsync();
        }

        async public void GameOver()
        {
            // unschedule and stop all actions
            UnscheduleAll();
            StopAllActions();

            // explode ball and remove it from gameplay layer
            ball.Explode();
            gameplayLayer.RemoveChild(ball);

            // wait some time and replace scene
            await Task.Delay(3000);
            GameController.GoToScene(new MenuScene(GameController.GameView));
        }

        public void IncrementScore()
        {
            score += 1;
            scoreLabel.Text = String.Format("{0}", score);
        }
    }
}
