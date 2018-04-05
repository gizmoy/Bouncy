using Bouncy.Entities;
using CocosSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouncy.Structures
{
    public class CubeMatrix : CCNode
    {
        static Random random = new Random(Guid.NewGuid().GetHashCode());

        public IDictionary<int, CubeRow> cubeRowsDict = new Dictionary<int, CubeRow>();
        CubeRow lastCubeRow;
        Cube lastCubeFromPath;

        public Cube InitialCube { get; set; }

        public List<CubeRow> CubeRows
        {
            get
            {
                return cubeRowsDict.Values.ToList();
            }
        }

        // action which removes first cube row from matrix
        CCCallFuncN removeFirstAndSpawnNewCubeRow = new CCCallFuncN(node =>
        {
            var cubeMatrix = node as CubeMatrix;
            var firstCubeRow = cubeMatrix.GetFirstCubeRow();

            if (firstCubeRow.PositionWorldspace.Y > GameConfig.RESOLUTION_HEIGHT)
            {
                cubeMatrix.RemoveFirstCubeRow();
                cubeMatrix.SpawnNewCubeRow();
            }
        });

        public CubeMatrix()
        {
            CreateCubeRows();
        }

        private void CreateCubeRows()
        {
            // compute number of rows
            var numRows = GameConfig.RESOLUTION_HEIGHT / (GameConfig.CUBE_HEIGHT * 0.75f) + 2;

            // creates rows of cubes
            for (int row = 0; row < numRows; row++)
            {
                // add cube row
                AddCubeRow(row);

                // if first row then set current cube to one and only cube from that row
                if (row == 0)
                {
                    InitialCube = this[0].FirstOrDefault();
                }
            }
        }

        public int ChooseColumn(Cube cube, int numCubesMax, int cubeNumInRow)
        {
            // if row is the first one just return 1
            if (cube.Row == 0)
            {
                // keep track of last cube from path
                lastCubeFromPath = cube;

                return 1;
            }
            // if cube is first in row try to find cube from previous row which has parent and room for child
            // and then set current cube as its child
            // otherwise if no room for child the choose randomly unoccupied column in the current row 
            else if (cubeNumInRow == 0)
            {
                //// get cubes' columns from previous row that have a room for child
                //var previousRow = cube.Row - 1;
                //var validPreviousRowColumns = this[previousRow]
                //        .Where(c => c.HasRoomForChild && c.HasParent)
                //        .Select(c => c.Col)
                //        .ToList();

                //// if no child room left then choose column from current row which neighbours with one of existing cubes
                //if (validPreviousRowColumns.Count == 0)
                //{
                //    return ChooseColumnFromCurrentRow(cube, numCubesMax);
                //};

                //// choose a random column from a valid ones
                //var chosenIndex = random.Next(validPreviousRowColumns.Count);
                //var chosenColumn = validPreviousRowColumns[chosenIndex];

                //return this[previousRow]
                //    .First(c => c.Col == chosenColumn)
                //    .FreeChildColumnAtRandom;

                // select random child column of last cube from path
                var column = lastCubeFromPath.FreeChildColumnAtRandom;

                // keep track of last cube from path
                lastCubeFromPath = cube;

                return column;
            }
            // else just choose randomly
            else
            {
                return ChooseColumnFromCurrentRow(cube, numCubesMax);
            }
        }

        private int ChooseColumnFromCurrentRow(Cube cube, int numCubesMax)
        {
            // get unocuppied columns from current row
            var currentRow = cube.Row;

            var allColumns = Enumerable
                .Range(0, numCubesMax)
                .ToList();

            var occupiedColumns = this[currentRow]
                .Select(c => c.Col)
                .ToList();

            var unoccupiedColumns = allColumns
                .Except(occupiedColumns)
                .ToList();

            // choose random column from unoccupied ones
            var chosenIndex = random.Next(unoccupiedColumns.Count);
            var chosenColumn = unoccupiedColumns[chosenIndex];

            return chosenColumn;
        }

        public void SetRelationsForCube(Cube cube)
        {
            if (cube == null || cube.Row == 0) return;

            // get left parent and set cube as its right child
            var previousRow = cube.Row - 1;

            var leftParentColumn = cube.LeftParentColumn;
            if (leftParentColumn != -1)
            {
                var leftParent = this[previousRow].FirstOrDefault(c => c.Col == leftParentColumn);
                cube.LeftParent = leftParent;
                if (leftParent != null)
                {
                    leftParent.RightChild = cube;
                }
            }

            // get right parent and set cube as its left child
            var rightParentColumn = cube.RightParentColumn;
            if (rightParentColumn != -1)
            {
                var rightParent = this[previousRow].FirstOrDefault(c => c.Col == rightParentColumn);
                cube.RightParent = rightParent;
                if (rightParent != null)
                {
                    rightParent.LeftChild = cube;
                }
            }
        }

        new public List<Cube> this[int row]
        {
            get
            {
                return (cubeRowsDict.ContainsKey(row))
                    ? cubeRowsDict[row].Cubes
                    : null;
            }
        }

        public Cube this[int row, int col]
        {
            get
            {
                var cubeRow = this[row];
                return (cubeRow != null)
                    ? cubeRow[col]
                    : null;
            }
        }

        public bool IsRowFirst(int row)
        {
            return cubeRowsDict.Keys.Min() == row;
        }

        public bool IsRowNotFirst(int row)
        {
            return !IsRowFirst(row);
        }

        public bool RemoveCubeRow(int row)
        {
            // manage child connections if row to deletetion is not a first one 
            if (IsRowNotFirst(row))
            {
                var previousRow = row - 1;
                var previousCubeRow = this[previousRow];
                foreach (var cube in previousCubeRow)
                {
                    if (cube.HasLeftChild) cube.LeftChild = null;
                    if (cube.HasRightChild) cube.RightChild = null;
                }
            }

            // remove nodes from tree and dictionary
            if (cubeRowsDict.ContainsKey(row))
            {
                var cubeRow = cubeRowsDict[row];
                var success = cubeRowsDict.Remove(row);
                this.RemoveChild(cubeRow);
                return success;
            }
            else
            {
                return false;
            }
        }

        public bool RemoveFirstCubeRow()
        {
            if (cubeRowsDict.Count <= 0) return false;
            
            var firstRow = cubeRowsDict.Keys.Min();
            return RemoveCubeRow(firstRow);
        }

        private CubeRow GetFirstCubeRow()
        {
            if (cubeRowsDict.Count <= 0) return null;

            var firstRow = cubeRowsDict.Keys.Min();
            return cubeRowsDict[firstRow];
        }

        public void AddCubeRow(int row)
        {
            // create cube row
            var cubeRow = new CubeRow(row);

            // set y position
            cubeRow.PositionY = (lastCubeRow == null)
                ? GameConfig.FIRST_ROW_INITIAL_POSITION_Y
                : lastCubeRow.PositionY - GameConfig.CUBE_HEIGHT * 0.75f;

            // add cube row to tree and dictionary
            this.AddChild(cubeRow);
            cubeRowsDict[cubeRow.Row] = cubeRow;

            // generate cubes and start motion if necessary
            cubeRow.CreateCubes();

            // keep track of last cube row
            lastCubeRow = cubeRow;
        }

        public void SpawnNewCubeRow()
        {
            AddCubeRow(lastCubeRow.Row + 1);
        }

        async public void MoveAsync()
        {
            // move up cube row one time and remove it from parent afterwards
            var duration = GameConfig.ACTION_DURATION_WITH_DELTA;
            var distance = GameConfig.MOVE_DISTANCE_Y;
            var point = new CCPoint(PositionX, PositionY + distance);

            // create action that moves cube row to final position
            var move = new CCMoveTo(duration, point);

            // run actions
            await RunActionsAsync(move, removeFirstAndSpawnNewCubeRow);
        }
    }
}
