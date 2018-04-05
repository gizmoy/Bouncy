using Bouncy.Structures;
using CocosSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouncy.Entities
{
    public class CubeRow : CCNode
    {
        static Random random = new Random(Guid.NewGuid().GetHashCode());

        IDictionary<int, Cube> cubesDict = new Dictionary<int, Cube>();
        CubeMatrix cubeMatrix
        {
            get
            {
                return Parent as CubeMatrix;
            }
        }

        public int Row { get; private set; }

        public List<Cube> Cubes
        {
            get
            {
                return cubesDict.Values.ToList();
            }
        }



        public CubeRow(int row = -1)
        {
            this.Row = row;
        }

        public void CreateCubes()
        {
            // check whether row number is even and set maximum number of cubes accordingly
            var isRowEven = (Row % 2 == 0);
            var numCubesMax = isRowEven
                ? GameConfig.MAX_NUM_CUBES_IN_EVEN_ROW
                : GameConfig.MAX_NUM_CUBES_IN_ODD_ROW;

            // init cubes list and get a random valid number of cubes
            var cubeNum = 0;
            var i = 0;

            while (i < numCubesMax)
            {
                // if number of cubes exceeds the minimum then drop cube with a given probabiliy
                if (cubeNum >= GameConfig.MIN_NUM_CUBES_IN_ROW)
                {
                    var probability = GameConfig.GetDropProbabilityForRow(Row);
                    if (random.NextDouble() <= probability)
                    {
                        i += 1;
                        continue;
                    }
                }

                // create a cube
                var cube = new Cube(Row);

                // check if path cube was generated and then choose column and if no column available then break loop 'cause no room left 
                var col = cubeMatrix.ChooseColumn(cube, numCubesMax, cubeNum);
                if (col == -1) break;

                // otherwise set column and parents
                cube.Col = col;
                cubeMatrix.SetRelationsForCube(cube);

                // set cube position x in row
                var rowPositionX = (GameConfig.RESOLUTION_WIDTH - GameConfig.CUBE_WIDTH * numCubesMax) / 2;
                cube.PositionX = rowPositionX + cube.Col * GameConfig.CUBE_WIDTH;

                // add cube to tree and dictionary
                AddChild(cube);
                cubesDict[cube.Col] = cube;

                // if first row then break the loop after first cube was added
                if (Row == 0) break;

                // increment number of cubes and index
                cubeNum += 1;
                i += 1;
            }
        }

        new public Cube this[int col]
        {
            get
            {
                return (cubesDict.ContainsKey(col))
                    ? cubesDict[col]
                    : null;
            }
        }
    }
}