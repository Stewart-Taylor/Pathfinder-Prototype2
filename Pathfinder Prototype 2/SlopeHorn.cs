using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathfinder_Prototype_2
{
    class SlopeHorn : SlopeAlgrotithm
    {

        public SlopeHorn(float[,] model): base(model)
        {
            heightMap = model;
            width = heightMap.GetLength(0);
            height = heightMap.GetLength(1);

            generateSlopeModel();
        }

        protected override float calculateSlopeValue(int x, int y)
        {

            float a = getTopLeft(x, y);
            float b = getTopMiddle(x, y);
            float c = getTopRight(x, y);

            float d = getMiddleLeft(x, y);
            float f = getMiddleRight(x, y);

            float g = getBottomLeft(x, y);
            float h = getBottomMiddle(x, y);
            float i = getBottomRight(x, y);


            float x_cell_size = 0.1f;
            float y_cell_size = 0.1f;

            float deltaX = ((c + (2 * f) + i) - (a + (2 * d) + g)) / (8 * x_cell_size);
            float deltaY = ((g + (2 * h) + i) - (a + (2 * b) + c)) / (8 * y_cell_size);

            float value = deltaY / deltaX;

            return value;
        }



        #region slope Heights

        private float getTopLeft(int x, int y)
        {
            if ((x > 0) && (y > 0))
            {
                return heightMap[x - 1, y - 1];
            }
            else
            {
                return heightMap[x , y ];
            }
        }

        private float getTopMiddle(int x, int y)
        {
            if (y > 0)
            {
                return heightMap[x, y - 1];
            }
            else
            {
                return heightMap[x, y];
            }
        }

        private float getTopRight(int x, int y)
        {
            if ((x < width - 1) && (y > 0))
            {
                return heightMap[x + 1, y - 1];
            }
            else
            {
                return heightMap[x, y];
            }
        }

        private float getMiddleLeft(int x, int y)
        {
            if (x > 0)
            {
                return heightMap[x - 1, y];
            }
            else
            {
                return heightMap[x, y];
            }
        }

        private float getMiddleRight(int x, int y)
        {
            if (x < width - 1)
            {
                return heightMap[x + 1, y];
            }
            else
            {
                return heightMap[x, y];
            }
        }

        private float getBottomLeft(int x, int y)
        {
            if ((x > 0) && (y < height - 1))
            {
                return heightMap[x - 1, y + 1];
            }
            else
            {
                return heightMap[x, y];
            }
        }

        private float getBottomMiddle(int x, int y)
        {
            if (y < height - 1)
            {
                return heightMap[x, y + 1];
            }
            else
            {
                return heightMap[x, y];
            }
        }

        private float getBottomRight(int x, int y)
        {

            if ((x < width - 1) && (y < height - 1))
            {
                return heightMap[x + 1, y + 1];
            }
            else
            {
                return heightMap[x, y];
            }
        }
        #endregion

    }
}

   