using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathfinder_Prototype_2
{
    abstract class SlopeAlgrotithm
    {

        protected float[,] heightMap;
        protected float[,] slopeModel;
        protected int height;
        protected int width; 


        public SlopeAlgrotithm(float[,] model)
        {
            heightMap = model;
            width = heightMap.GetLength(0);
            height = heightMap.GetLength(1);


            generateSlopeModel();
        }



        protected void generateSlopeModel()
        {
            slopeModel = new float[width ,height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    slopeModel[x, y] = calculateSlopeValue(x,y);
                }
            }

        }



        protected virtual float calculateSlopeValue(int x, int y)
        {

            float topLeft = getTopLeftSlope(x, y);
            float topMid = getTopMiddleSlope(x, y);
            float topRight = getTopRightSlope(x, y);

            float midLeft = getMiddleLeftSlope(x, y);
            float midRight = getMiddleRightSlope(x, y);

            float botLeft = getBottomLeftSlope(x, y);
            float botMid = getBottomMiddleSlope(x, y);
            float botRight = getBottomRightSlope(x, y);


            float high = 0f;

            if (topLeft >= high) { high = topLeft; }
            if (topMid >= high) { high = topMid; }
            if (topRight >= high) { high = topRight; }
            if (midLeft >= high) { high = midLeft; }
            if (midRight >= high) { high = midRight; }
            if (botLeft >= high) { high = botLeft; }
            if (botMid >= high) { high = botMid; }
            if (botRight >= high) { high = botRight; }


            float total = topLeft + topRight + topMid + midLeft + midRight + botLeft + botMid + botRight;

            // float value = total / 8;
            float value = high;

            return value;
        }



        private float getGradientValue(float x, float y)
        {
            float gradient = 0;

            if ((x > 0) && (y > 0))
            {
                gradient = x - y;
                gradient = Math.Abs(gradient);
            }

            return gradient;

        }


        #region slope Calcs

        protected float getTopLeftSlope(int x, int y)
        {
            float gradient = 0;

            if ((x > 0) && (y > 0))
            {
                gradient = getGradientValue((float)heightMap[x - 1, y - 1], (float)heightMap[x, y]);
            }

            return gradient;
        }

        protected float getTopMiddleSlope(int x, int y)
        {
            float gradient = 0;

            if (y > 0)
            {
                gradient = getGradientValue((float)heightMap[x, y - 1], (float)heightMap[x, y]);
            }

            return gradient;
        }

        protected float getTopRightSlope(int x, int y)
        {
            float gradient = 0;

            if ((x < width - 1) && (y > 0))
            {
                gradient = getGradientValue((float)heightMap[x + 1, y - 1], (float)heightMap[x, y]);
            }

            return gradient;
        }

        protected float getMiddleLeftSlope(int x, int y)
        {
            float gradient = 0;

            if (x > 0)
            {
                gradient = getGradientValue((float)heightMap[x - 1, y], (float)heightMap[x, y]);
            }

            return gradient;

        }

        protected float getMiddleRightSlope(int x, int y)
        {
            float gradient = 0;


            if (x < width - 1)
            {
                gradient = getGradientValue((float)heightMap[x + 1, y], (float)heightMap[x, y]);
            }

            return gradient;
        }

        protected float getBottomLeftSlope(int x, int y)
        {
            float gradient = 0;

            if ((x > 0) && (y < height - 1))
            {
                gradient = getGradientValue((float)heightMap[x - 1, y + 1], (float)heightMap[x, y]);
            }

            return gradient;
        }

        protected float getBottomMiddleSlope(int x, int y)
        {
            float gradient = 0;

            if (y < height - 1)
            {
                gradient = getGradientValue((float)heightMap[x, y + 1], (float)heightMap[x, y]);
            }

            return gradient;
        }

        protected float getBottomRightSlope(int x, int y)
        {
            float gradient = 0;

            if ((x < width - 1) && (y < height - 1))
            {
                gradient = getGradientValue((float)heightMap[x + 1, y + 1], (float)heightMap[x, y]);
            }

            return gradient;
        }



        #endregion



        public float[,] getSlopeModel()
        {
            return slopeModel;
        }

    }
}
