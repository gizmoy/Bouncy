using Bouncy.Entities;
using CocosSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bouncy.Scenes
{
    class MenuScene : CCScene
    {
        CCLayer backgroundLayer;
        CCLayer foregroundLayer;
        IList<Button> buttons = new List<Button>();



        public MenuScene(CCGameView gameView) : base(gameView)
        {
            CreateLayers();

            CreateBackground();

            CreateForeground();

            CreateTouchListener();
        }

        private void CreateLayers()
        {
            backgroundLayer = new CCLayer();
            this.AddLayer(backgroundLayer);

            foregroundLayer = new CCLayer();
            this.AddLayer(foregroundLayer);
        }

        private void CreateBackground()
        {
            var background = new CCSprite("Background.png");
            background.AnchorPoint = new CCPoint(0, 0);
            backgroundLayer.AddChild(background);
        }

        private void CreateForeground()
        {
            CreateLogo();

            CreateButtons();
        }

        private void CreateLogo()
        {
            // create logo graphic
            var logo = new CCSprite("Logo.png");
            logo.BlendFunc = CCBlendFunc.NonPremultiplied;
            logo.AnchorPoint = new CCPoint(0, 0);
            logo.PositionX = (base.GameView.DesignResolution.Width - logo.ContentSize.Width) / 2;
            logo.PositionY = GameConfig.LOGO_POSITION_Y;
            logo.IsAntialiased = false;
            foregroundLayer.AddChild(logo);

            // start position
            var startPositionX = -500.0f;
            var startPositionY = GameConfig.LOGO_POSITION_Y;
            logo.PositionX = startPositionX;
            logo.PositionY = startPositionY;

            // end position
            var endPositionX = (base.GameView.DesignResolution.Width - logo.ContentSize.Width) / 2;
            var endPositionY = startPositionY;

            // create and run ease move action
            var point = new CCPoint(endPositionX, endPositionY);
            var coreAction = new CCMoveTo(4.0f, point);
            var easeAction = new CCEaseElasticOut(coreAction);
            logo.RunAction(easeAction);
        }

        private void CreateButtons()
        {
            var i = 0;
            foreach (var buttonText in GameConfig.BUTTONS_DICTIONARY.Keys)
            {
                // create button and set action
                var button = new Button(buttonText);
                button.Action = GameConfig.BUTTONS_DICTIONARY[buttonText];

                // add button
                foregroundLayer.AddChild(button);
                buttons.Add(button);

                // start position
                var startPositionX = (base.GameView.DesignResolution.Width - button.ContentSize.Width) / 2;
                var startPositionY = GameConfig.PLAY_BUTTON_POSITION_Y - i * 170 - GameConfig.PLAY_BUTTON_POSITION_Y;
                button.PositionX = startPositionX;
                button.PositionY = startPositionY;

                // end position
                var endPositionX = (base.GameView.DesignResolution.Width - button.ContentSize.Width) / 2;
                var endPositionY = GameConfig.PLAY_BUTTON_POSITION_Y - i * 170;

                // create and run ease move action
                var point = new CCPoint(endPositionX, endPositionY);
                var coreAction = new CCMoveTo(4.0f, point);
                var easeAction = new CCEaseBackOut(coreAction);
                button.RunAction(easeAction);

                // increment counter
                i += 1;
            }
        }

        private void CreateTouchListener()
        {
            var touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesBegan = HandleTouchesBegan;
            foregroundLayer.AddEventListener(touchListener);
        }

        private void HandleTouchesBegan(List<CCTouch> touches, CCEvent ccevent)
        {
            foreach (var touch in touches)
            {
                foreach (var button in buttons)
                {
                    if (button.ContainsPoint(touch.Location))
                    {
                        // play press sound effect and perform action
                        CCAudioEngine.SharedEngine.PlayEffect("Press");
                        button.PerformAction();
                    }
                }
            }
        }
    }
}
