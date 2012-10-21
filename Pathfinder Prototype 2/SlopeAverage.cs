using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathfinder_Prototype_2
{
    class SlopeAverage : SlopeAlgrotithm
    {

        public SlopeAverage(float[,] model) : base(model)
        {
            heightMap = model;
            width = heightMap.GetLength(0);
            height = heightMap.GetLength(1);

            generateSlopeModel();
        }


        protected override float calculateSlopeValue(int x, int y)
        {

            float topLeft = getTopLeftSlope(x, y);
            float topMid = getTopMiddleSlope(x, y);
            float topRight = getTopRightSlope(x, y);

            float midLeft = getMiddleLeftSlope(x, y);
            float midRight = getMiddleRightSlope(x, y);

            float botLeft = getBottomLeftSlope(x, y);
            float botMid = getBottomMiddleSlope(x, y);
            float botRight = getBottomRightSlope(x, y);

            float total = topLeft + topRight + topMid + midLeft + midRight + botLeft + botMid + botRight;

            float value = total / 8f;

            return value;
        }


    }
}
