using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.IO;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace Pathfinder_Prototype_2
{
    class Pathfinder
    {
        private int width;
        private int height;


        private Bitmap pathBitmap;
        private float[,] hazardModel;

        List<PathNode> pathNodes = new List<PathNode>();

      

        public Pathfinder(float[,] hazardModelT , int startX , int startY , int targetX , int targetY)
        {
            hazardModel = hazardModelT;

            width = hazardModel.GetLength(0);
            height = hazardModel.GetLength(1);

            findPath(startX , startY , targetX , targetY);
            generatePathImage();
        }


        public List<PathNode> getPath()
        {
            return pathNodes;
        }


        private void findPath(int startX , int startY , int targetX , int targetY)
        {
            SearchAlgorithm aStar = new AStar(hazardModel , startX , startY , targetX , targetY);

            

            pathNodes = aStar.getPath();
        }







        private void generatePathImage()
        {
            Bitmap bitmap = new Bitmap(width, height);

            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    System.Drawing.Color tempColor = getPathColorValue(hazardModel[x, y] , x , y);

                    bitmap.SetPixel(x, y, tempColor);
                 
                }
            
            }

            pathBitmap = bitmap;
        }


        private System.Drawing.Color getPathColorValue(float gradient , int x , int y)
        {
            System.Drawing.Color color = System.Drawing.Color.White;

            gradient = Math.Abs(gradient);

            float green = 255;
            float red = 255;
            float blue = 0;


            if (gradient <= 1f)
            {

                red = (1f - gradient) * 255;

            }
            else if (gradient <= 3f)
            {
                float percent = (gradient) / (3f);

                green = (1f - percent) * 255;
            }
            else
            {
                green = 0;
            }

            foreach (PathNode n in pathNodes)
            {
                if ((n.x == x) && (n.y == y))
                {
                    red = 0;
                    green = 0;
                    blue = 255;
                }

            }


            color = System.Drawing.Color.FromArgb(255, (int)red, (int)green, (int)blue);

            return color;
        }




        public ImageSource getPathImage()
        {
            MemoryStream ms = new MemoryStream();
            pathBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();

            ImageSource img = bi;

            return img;
        }


    }
}
