using Bouncy.Scenes;
using CocosSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouncy
{
    public static class GameController
    {
        public static CCGameView GameView { get; private set; }

        public static void Initialize(CCGameView gameView)
        {
            GameView = gameView;

            // Add content search paths
            var contentSearchPaths = new List<string>() { "Fonts", "Sounds", "Images" };

#if __IOS__
            contentSearchPaths.Add("Sounds/iOS/");
#else // android
            contentSearchPaths.Add("Sounds/Android/");
#endif
            GameView.ContentManager.SearchPaths = contentSearchPaths;

            // Set display resolution to Iphone 6s/6 dims
            int width = GameConfig.RESOLUTION_WIDTH;
            int height = GameConfig.RESOLUTION_HEIGHT;
            GameView.DesignResolution = new CCSizeI(width, height);

            // Init game sounds
            InitializeAudio();

            // Create and run menu scene
            var scene = new MenuScene(GameView);
            GameView.RunWithScene(scene);
        }

        private static void InitializeAudio()
        { 
            try
            {
                CCAudioEngine.SharedEngine.PlayBackgroundMusic(filename: "BouncySong", loop: true);
            }
            catch (Exception) { /* continue if cannot load music */ }
        }

        public static void GoToScene(CCScene scene)
        {
            GameView.Director.ReplaceScene(scene);
        }
    }
}

