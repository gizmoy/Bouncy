using CocosSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouncy.Entities
{
    public class Cube : CCNode
    {
        static Random random = new Random();

        CCSprite graphic;
        string color;

        public Cube LeftParent { get; set; }
        public Cube RightParent { get; set; }
        public Cube LeftChild { get; set; }
        public Cube RightChild { get; set; }

        public int Row { get; set; }
        public int Col { get; set; }


        public bool HasLeftParent
        {
            get { return LeftParent != null; }
        }

        public bool HasRightParent
        {
            get { return RightParent != null; }
        }

        public bool HasParent
        {
            get { return HasLeftParent || HasRightParent; }
        }

        public bool HasRoomForChild
        {
            get { return IsLeftChildAvailable || IsRightChildAvailable; }
        }

        public int LeftChildColumn
        {
            get
            {
                var isRowEven = (Row % 2 == 0);

                if (isRowEven)
                {
                    return Col;
                }
                else
                {
                    return (Col == 0)
                        ? -1
                        : Col - 1;
                }
            }
        }

        public int RightChildColumn
        {
            get
            {
                var isRowEven = (Row % 2 == 0);
                var maxNumCubes = isRowEven
                    ? GameConfig.MAX_NUM_CUBES_IN_EVEN_ROW
                    : GameConfig.MAX_NUM_CUBES_IN_ODD_ROW;

                if (isRowEven)
                {
                    return Col + 1;
                }
                else
                {
                    return (Col == maxNumCubes - 1)
                        ? -1
                        : Col;
                }
            }
        }

        public int FreeChildColumnAtRandom
        {
            get
            {
                if (IsLeftChildAvailable && IsRightChildAvailable)
                {
                    var lotto = random.Next(2);
                    return (lotto == 0)
                        ? LeftChildColumn
                        : RightChildColumn;
                }
                else if (IsLeftChildAvailable)
                {
                    return LeftChildColumn;
                }
                else if (IsRightChildAvailable)
                {
                    return RightChildColumn;
                }
                else
                {
                    return -1;
                }
            }
        }

        public int LeftParentColumn
        {
            get
            {
                var isRowEven = (Row % 2 == 0);

                if (isRowEven)
                {
                    return Col;
                }
                else
                {
                    return (Col == 0)
                        ? -1
                        : Col - 1;
                }
            }
        }

        public int RightParentColumn
        {
            get
            {
                var isRowEven = (Row % 2 == 0);
                var maxNumCubes = isRowEven
                    ? GameConfig.MAX_NUM_CUBES_IN_EVEN_ROW
                    : GameConfig.MAX_NUM_CUBES_IN_ODD_ROW;

                if (isRowEven)
                {
                    return Col + 1;
                }
                else
                {
                    return (Col == maxNumCubes - 1)
                        ? -1
                        : Col;
                }
            }
        }

        public bool IsLeftChildAvailable
        {
            get { return LeftChildColumn != -1 && LeftChild == null; }
        }

        public bool IsRightChildAvailable
        {
            get { return RightChildColumn != -1 && RightChild == null; }
        }

        public bool HasLeftChild
        {
            get { return LeftChildColumn != -1 && LeftChild != null; }
        }

        public bool HasRightChild
        {
            get { return RightChildColumn != -1 && RightChild != null; }
        }



        public Cube() : this(-1) { }

        public Cube(int row, string color = "Red")
        {
            // set variables
            this.Row = row;
            this.color = color;

            // if first row get graphic with shadow 'cause this is initial cube
            var filename = (row == 0)
                ? color + " cube with shadow.png"
                : color + " cube.png";
            graphic = new CCSprite(filename);
            graphic.BlendFunc = CCBlendFunc.NonPremultiplied;
            graphic.AnchorPoint = new CCPoint(0, 0);
            this.AddChild(graphic);
        }

        public bool ContainsPoint(CCPoint point)
        {
            return graphic.BoundingBoxTransformedToWorld.ContainsPoint(point);
        }

        public void CoverWithShadow()
        {
            ChangeGraphic(color + " cube with shadow");
        }

        public Cube GetNextCube(Direction direction)
        {
            // Cube != null && 
            if (direction == Direction.Right && HasRightChild)
            {
                return RightChild;
            }
            else if (direction == Direction.Left && HasLeftChild)
            {
                return LeftChild;
            }
            else
            {
                return null;
            }
        }

        public void ChangeGraphic(string name)
        {
            // remove old graphic from parent
            this.RemoveChild(graphic);

            // create new one
            var filename = name + ".png";
            graphic = new CCSprite(filename);
            graphic.BlendFunc = CCBlendFunc.NonPremultiplied;
            graphic.AnchorPoint = new CCPoint(0, 0);
            this.AddChild(graphic);
        }
    }
}
