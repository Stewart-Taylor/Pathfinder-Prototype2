using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.IO;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Threading;

namespace Pathfinder_Prototype_2
{
    class Vehicle
    {

        DispatcherTimer blinkUpdate = new DispatcherTimer();
        Dispatcher dispatcher;

        private List<PathNode> takenPath = new List<PathNode>();
        private List<PathNode> givenPath = new List<PathNode>();

        float[,] knownMap;
        float[,] realMap;

        private int positionX;
        private int positionY;

        private int startX;
        private int startY;
        private int targetX;
        private int targetY;



        private Bitmap pathBitmap;

        public int getX()
        {
            return positionY;
        }

        public int getY()
        {
            return positionY;
        }

        public Vehicle(float[,] realMapT , Dispatcher d)
        {
            dispatcher = d;
            realMap = realMapT;
            knownMap = new float[realMap.GetLength(0), realMap.GetLength(1)]; // find another way to get map dimensions dynamically

            pathBitmap = new Bitmap(knownMap.GetLength(0), knownMap.GetLength(1));

           
        }



        public void traverseMap(int startXt, int startYt, int endXt, int endYt)
        {

            startX = startXt;
            startY = startYt;
            targetX = endXt;
            targetY = endYt;

            positionX = startX;
            positionY = startY;



            bool atTarget = false;

            SearchAlgorithm search ; // = new AStar(knownMap,positionX , positionY , targetX , targetY);

            do
            {
                search = new AStar(knownMap,positionX , positionY , targetX , targetY);

                givenPath = search.getPath();


                
                do
                {

                    if( (positionX == targetX) && (positionY == targetY))
                    {
                        atTarget = true;
                        break;
                    }

                    PathNode nextNode = givenPath.Last();
                    givenPath.Remove(nextNode);

                    if (isNextNodeSafe(nextNode) == true)
                    {
                        positionX = nextNode.x;
                        positionY = nextNode.y;
                        takenPath.Add(nextNode);

                        updateOwnMap(positionX, positionY);
                    }
                    else
                    {
                        updateOwnMap(nextNode.x, nextNode.y);
                        break; 
                    }



                }while(true);


            } while (atTarget == false);
            blinkUpdate.Stop();
            generatePathImage();
           
        }




        private bool isNextNodeSafe(PathNode node)
        {
            if (realMap[node.x, node.y] <= 0.5)
            {
                return true;
            }

            if (realMap[node.x, node.y] == knownMap[node.x, node.y])
            {
                return true; // means no safer path
            }
            return false;
        }

        private void updateOwnMap(int x , int y)
        {
            knownMap[x, y] = realMap[x, y];

            System.Drawing.Color tempColor = getVehicleColorValue(knownMap[x, y], x, y);

            pathBitmap.SetPixel(x, y, tempColor);

        }


        public void imageUpdate()
        {

            ((MainWindow)App.Current.MainWindow).img_main.Source = getPathImage();

        }


        private void generatePathImage()
        {
            Bitmap bitmap = new Bitmap(knownMap.GetLength(0), knownMap.GetLength(1));


            for (int x = 0; x < knownMap.GetLength(0); x++)
            {
                for (int y = 0; y < knownMap.GetLength(1); y++)
                {
                    System.Drawing.Color tempColor = getVehicleColorValue(knownMap[x, y] , x , y);

                    bitmap.SetPixel(x, y, tempColor);

                }

            }

            pathBitmap = bitmap;
        }


        private System.Drawing.Color getVehicleColorValue(float gradient, int x, int y)
        {
            System.Drawing.Color color = System.Drawing.Color.White;

            gradient = Math.Abs(gradient);

            float green = 255;
            float red = 255;
            float blue = 0;

            if (gradient == 0)
            {
                gradient = realMap[x, y];

            }

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

            foreach (PathNode n in takenPath)
            {
                if ((n.x == x) && (n.y == y))
                {
                    red = 0;
                    green = 0;
                    blue = 255;
                }

            }


            if( (targetX == x) && ( targetY ==y))
            {
                green = 255; red = 255; blue = 255;
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
