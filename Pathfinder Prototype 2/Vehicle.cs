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
        private List<PathNode> takenPath = new List<PathNode>();
        private List<PathNode> givenPath = new List<PathNode>();

        float[,] knownMap;
        float[,] realMap;
        float[,] realImageMap;

        private int positionX;
        private int positionY;
        private int previousX;
        private int previousY;

        private int startX;
        private int startY;
        private int targetX;
        private int targetY;

        private int steps = 0;
        private bool atTarget = false;

        private bool stepTraverseStarted = false;

        private Bitmap pathBitmap;


        public int getSteps()
        {
            return steps;
        }

        public int getX()
        {
            return positionY;
        }

        public int getY()
        {
            return positionY;
        }

        public Vehicle(float[,] realMapT , float[,] imageMap )
        {
            realMap = realMapT;
            realImageMap = imageMap;
            knownMap = new float[realMap.GetLength(0), realMap.GetLength(1)]; // find another way to get map dimensions dynamically

            pathBitmap = new Bitmap(knownMap.GetLength(0), knownMap.GetLength(1));  
        }



        public void startTraverse(int startXt, int startYt, int endXt, int endYt)
        {
            steps = 0;

            startX = startXt;
            startY = startYt;
            targetX = endXt;
            targetY = endYt;

            positionX = startX;
            positionY = startY;

            knownMap = new float[realMap.GetLength(0), realMap.GetLength(1)];

            atTarget = false;

            pathBitmap = new Bitmap(knownMap.GetLength(0), knownMap.GetLength(1));
        }


        public void traverseMap(int startXt, int startYt, int endXt, int endYt)
        {
             steps = 0;

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

                    if (steps >= 1600)
                    {
                        atTarget = true;
                        break;
                    }
                    steps++;

                    if( (positionX == targetX) && (positionY == targetY))
                    {
                        atTarget = true;
                        break;
                    }
                    if (givenPath.Count == 0)
                    {
                        atTarget = true;
                        break;
                    }
                    PathNode nextNode = givenPath.Last();
                    givenPath.Remove(nextNode);

                    if (isNextNodeSafe(nextNode) == true)
                    {
                        previousX = positionX;
                        previousY = positionY;
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
          
        }




        public void traverseMapDstar(int startXt, int startYt, int endXt, int endYt)
        {
            steps = 0;

            startX = startXt;
            startY = startYt;
            targetX = endXt;
            targetY = endYt;

            positionX = startX;
            positionY = startY;

            bool atTarget = false;

            Dstar search = new Dstar(knownMap, positionX, positionY, targetX, targetY);

           do
           {
               search.updateStart(positionX, positionY);
               search.replan(knownMap);

               givenPath = search.getPath();

               do
               {

                   if (steps >= 1600)
                   {
                       atTarget = true;
                       break;
                   }
                   steps++;

                   if ((positionX == targetX) && (positionY == targetY))
                   {
                       atTarget = true;
                       break;
                   }
                   if (givenPath.Count == 0)
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
                       search.updateVertex(nextNode.x, nextNode.y);
                       updateOwnMap(nextNode.x, nextNode.y);
                       break;
                   }


               } while (true);


           } while (atTarget == false);
          
            
            generatePathImage();
            
        }


        public void traverseMapStep()
        {
            SearchAlgorithm search;

            search = new AStar(knownMap, positionX, positionY, targetX, targetY);

            givenPath = search.getPath();


            if ((positionX == targetX) && (positionY == targetY))
            {
                atTarget = true;

            }
            if (givenPath.Count == 0)
            {
                atTarget = true;

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
            }

        }


        private bool isNextNodeSafe(PathNode node)
        {
            if (realMap[node.x, node.y] <= 5.0)
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

            System.Drawing.Color tempColor = getVehicleColorValue(realImageMap[x, y], x, y);

            pathBitmap.SetPixel(x, y, tempColor);
        }


        private void updateFrontView()
        {
            if ((previousX != 0) && (previousY != 0))
            {
                if (realMap[positionX, positionY] != knownMap[positionX, positionY])
                {

                    int directionX = positionX - previousX;
                    int directionY = positionY - previousY;

                    if ((directionX == -1) && (directionY == -1)) { updateFacingTopLeft(); }
                    else if ((directionX == 0) && (directionY == -1)) { updateFacingTopMiddle(); }
                    else if ((directionX == 1) && (directionY == -1)) { updateFacingTopRight(); }
                    else if ((directionX == -1) && (directionY == 0)) { updateFacingMiddleLeft(); }
                    else if ((directionX == 1) && (directionY == 0)) { updateFacingMiddleRight(); }
                    else if ((directionX == -1) && (directionY == 1)) { updateFacingBottomLeft(); }
                    else if ((directionX == 0) && (directionY == 1)) { updateFacingBottomMiddle(); }
                    else if ((directionX == 1) && (directionY == 1)) { updateFacingBottomRight(); }
                }
            }
        }

        private void updateTile(int x , int y)
        {
            knownMap[x, y] = realMap[x, y];
        }

        private void updateFacingTopLeft()
        {
            updateTile(positionX - 1, positionY - 1);
            updateTile(positionX - 2, positionY  );
            updateTile(positionX + 1, positionY  + 1);
            updateTile(positionX - 2, positionY - 2);
        }

        private void updateFacingTopMiddle()
        {
            updateTile(positionX - 1, positionY - 1);
            updateTile(positionX , positionY - 1);
            updateTile(positionX + 1, positionY - 1);
            updateTile(positionX , positionY - 2);
        }

        private void updateFacingTopRight()
        {
            updateTile(positionX + 1, positionY - 1);
            updateTile(positionX , positionY -2);
            updateTile(positionX + 2, positionY );
            updateTile(positionX + 2, positionY - 2);
        }

        private void updateFacingMiddleLeft()
        {
            updateTile(positionX - 1, positionY );
            updateTile(positionX -1, positionY - 1);
            updateTile(positionX - 1, positionY + 1);
            updateTile(positionX -2, positionY );
        }

        private void updateFacingMiddleRight()
        {
            updateTile(positionX + 1, positionY);
            updateTile(positionX + 1, positionY - 1);
            updateTile(positionX + 1, positionY + 1);
            updateTile(positionX + 2, positionY);
        }

        private void updateFacingBottomLeft()
        {
            updateTile(positionX -1, positionY + 1);
            updateTile(positionX - 2, positionY );
            updateTile(positionX , positionY + 2);
            updateTile(positionX -2, positionY + 2);
        }

        private void updateFacingBottomMiddle()
        {
            updateTile(positionX , positionY + 1);
            updateTile(positionX - 1, positionY + 1);
            updateTile(positionX + 1, positionY + 1);
            updateTile(positionX , positionY + 2);
        }

        private void updateFacingBottomRight()
        {
            updateTile(positionX + 1, positionY + 1);
            updateTile(positionX + 2, positionY);
            updateTile(positionX, positionY + 2);
            updateTile(positionX + 2, positionY + 2);
        }

        public void imageUpdate()
        {
            ((MainWindow)App.Current.MainWindow).img_main.Source = getPathImage();
        }


        private void generateImageQuick()
        {
            Bitmap bitmap = new Bitmap(knownMap.GetLength(0), knownMap.GetLength(1));

            for (int x = 0; x < knownMap.GetLength(0); x++)
            {
                for (int y = 0; y < knownMap.GetLength(1); y++)
                {
                    System.Drawing.Color tempColor = getVehicleColorValue(realImageMap[x, y], x, y);

                    bitmap.SetPixel(x, y, tempColor);

                }

            }

            pathBitmap = bitmap;
        }


        public void generatePathImage()
        {
            Bitmap bitmap = new Bitmap(knownMap.GetLength(0), knownMap.GetLength(1));


            for (int x = 0; x < knownMap.GetLength(0); x++)
            {
                for (int y = 0; y < knownMap.GetLength(1); y++)
                {
                    System.Drawing.Color tempColor = getVehicleColorValue(realImageMap[x, y], x, y);

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

            bool notKnown = false;
            if (gradient == 0)
            {
                notKnown = true;
                gradient = realImageMap[x, y];
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


            if (notKnown == true)
            {
                green = ((float)green * 0.3f);
                red = ((float)red * 0.3f);

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
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = ms;
            bitmap.EndInit();

            return bitmap;
        }

    }
}
