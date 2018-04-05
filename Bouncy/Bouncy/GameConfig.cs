using Bouncy.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouncy
{
    public static class GameConfig
    {
        // resolution dimensions
        public static int RESOLUTION_WIDTH = 750;
        public static int RESOLUTION_HEIGHT = 1334;

        // logo y position
        public static float LOGO_POSITION_Y = 950.0f;

        // comment
        public static IDictionary<string, Action> BUTTONS_DICTIONARY = new Dictionary<string, Action>()
        {
            { "Play",    () => { GameController.GoToScene (new GameScene (GameController.GameView)); } }
        };

        // play button position y
        public static float PLAY_BUTTON_POSITION_Y = 180;

        // initial y position of first row
        public static float FIRST_ROW_INITIAL_POSITION_Y = 950;

        // maximum and minimum numbers of cubes in particular rows
        public static int MAX_NUM_CUBES_IN_ODD_ROW = 5;
        public static int MAX_NUM_CUBES_IN_EVEN_ROW = 4;
        public static int MIN_NUM_CUBES_IN_ROW = 2;

        // cube dimensions
        public static float CUBE_WIDTH = 139.0f;
        public static float CUBE_HEIGHT = 160.0f;

        // overall valocity of gameplay 
        public static float VELOCITY = 400.0f;

        // ball initial position
        public static float BALL_INITIAL_POSITION_X = 300.0f;
        public static float BALL_INITIAL_POSITION_Y = 1300.0f;

        // move distances
        public static float MOVE_DISTANCE_X = CUBE_WIDTH;
        public static float MOVE_DISTANCE_Y = CUBE_HEIGHT * 0.75f;

        // duration of one action pass
        public static float ACTION_DURATION_DELTA = 0.02f;
        public static float ACTION_DURATION = (MOVE_DISTANCE_Y / VELOCITY);
        public static float ACTION_DURATION_WITH_DELTA = (MOVE_DISTANCE_Y / VELOCITY) - ACTION_DURATION_DELTA;

        // position of score label
        public static float SCORE_LABEL_POSITION_X = RESOLUTION_WIDTH / 2.0f;
        public static float SCORE_LABEL_POSITION_Y = 1280.0f;

        // maximum drop probability and number of rows to reach it
        public static double MAX_DROP_PROBABILTY = 0.7;
        public static int NUM_OF_ROWS_TO_MIN_DROP_PROBABILITY = 50;

        // returns drop probability for row
        public static double GetDropProbabilityForRow(int row)
        {
            var step = row / 20 + 1.0;
            var probability = 0.25 * Math.Sin(20.0 * step) + Math.Log(step, 10.0);
            return probability < MAX_DROP_PROBABILTY
                ? probability
                : MAX_DROP_PROBABILTY;
        }

    }
}

