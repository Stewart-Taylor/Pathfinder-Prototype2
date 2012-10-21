using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathfinder_Prototype_2
{
    class AStar :SearchAlgorithm
    {

        List<Node> closed = new List<Node>();
        List<Node> open = new List<Node>();

        List<Node> path;

        private Node targetNode;

        public class Node
        {
            public int f()
            {
                return g + h;
            }
           public int g;
           public int h;
           public int x;
           public int y;
           public Node parent;

        }

        public AStar(float[,] gridT , int startXT , int startYT , int targetXT , int targetYT) : base (  gridT , startXT , startYT , targetXT , targetYT)
        {
            grid = gridT;
            startX = startXT;
            startY = startYT;
            targetX = targetXT;
            targetY = targetYT;


            if (((startX == targetX) && (startY == targetY)) == false)
            {

                bool found = false; // used to determine if path is found

                // creates the start node
                Node current = new Node();
                current.x = startX;
                current.y = startY;

                //creates the target node
                targetNode = new Node();
                targetNode.x = targetX;
                targetNode.y = targetY;
                

                //puts the start item in list
                open.Add(current);

                do
                {
                    // This call places the lowest f at the bottom

                    open.Sort(
                    delegate(Node x, Node y)
                    {
                        return x.f() - y.f();
                    });


                    // Get the node with the lowest TotalCost
                    current = open[0];

                    if (current.x == targetNode.x)
                    {
                        if (current.y == targetNode.y)
                        {
                            found = true;
                            break;
                        }
                    }

                    checkAdjacent(current.x, current.y, open, closed, current);


                    //remove a from the open list and move into the 'closed' list
                    open.Remove(current);
                    closed.Add(current);


                } while (open.Count > 0); // Keeps going until the open list is empty



                // if a path was found the path is worked out then returned
                if (found == true)
                {

                    foreach (Node n in closed)
                    {
                        PathNode pathNode = new PathNode();
                        pathNode.x = current.x;
                        pathNode.y = current.y;

                        pathNodes.Add(pathNode);

                        if( current.parent == null)
                        {
                            break;
                        }
                        current = current.parent;
                    }


                    /*
                    do
                    {
                        PathNode pathNode = new PathNode();
                        pathNode.x = current.x;
                        pathNode.y = current.y;

                        pathNodes.Add(pathNode);

                        if (current.parent.Equals(null))
                        {
                            // path.Add(new int[2] { current.x, current.y });
                            current = current.parent;
                        }
                        else
                        {
                            closed.Clear();
                        }

                    } while (closed.Count > 0);
                    */
                }


            }

        }




        private void checkAdjacent(int x, int y, List<Node> open, List<Node> closed, Node parent)
        {
           
            if (checkTile((x - 1), (y), open, closed) == true)  //MIDDLE LEFT
            {
                createNewNode((x - 1), y, 10, open, parent);

            }


            if (checkTile((x + 1), y, open, closed) == true) //MIDDLE RIGHT
            {
                createNewNode((x + 1), y, 10, open, parent);

            }



            if (checkTile((x), (y + 1), open, closed) == true) // MIDDLE BOTTOM
            {
                createNewNode((x), (y + 1), 10, open, parent);

            }



            if (checkTile((x), (y - 1), open, closed) == true) // MIDDLE TOP
            {
                createNewNode((x), (y - 1), 10, open, parent);

            }




           



                if (checkTile((x - 1), (y - 1), open, closed) == true)    //TOP LEFT
                {
                    createNewNode((x - 1), (y - 1), 14, open, parent);
                }



                if (checkTile((x - 1), (y + 1), open, closed) == true)   // BOTTOM LEFT
                {
                    createNewNode((x - 1), (y + 1), 14, open, parent);
                }


                if (checkTile((x + 1), (y - 1), open, closed) == true) //TOP RIGHT
                {

                    createNewNode((x + 1), (y - 1), 14, open, parent);
                }



                if (checkTile((x + 1), (y + 1), open, closed) == true)  //BOTTOM RIGHT
                {
                    createNewNode((x + 1), (y + 1), 14, open, parent);
                }


            

        }


        private void createNewNode(int x, int y, int value, List<Node> open, Node parent)
        {

            Node newNode = new Node();
            newNode.x = x;
            newNode.y = y;
            newNode.g = (int)(grid[x, y] * 10);
            newNode.h = estimateDistance(x, y, targetNode);
            newNode.parent = parent;
                

            open.Add(newNode);
        }



        private bool checkTile(int x, int y, List<Node> closed, List<Node> open)
        {

            if (x < 0)
            {
                return false;
            }
            if (y < 0)
            {
                return false;
            }
            if (x > grid.GetLength(0) - 1)
            {
                return false;
            }
            if (y > grid.GetLength(1) - 1)
            {
                return false;
            }

            foreach (Node node in closed) // Loop through List with foreach
            {
                if (node.x == x)
                {
                    if (node.y == y)
                    {
                        return false;
                    }
                }
            }

            foreach (Node node in open) // Loop through List with foreach
            {
                if (node.x == x)
                {
                    if (node.y == y)
                    {
                        return false;
                    }
                }
            }



            return true;
        }





        private int estimateDistance(int currentX, int currentY, Node target)
        {

            int distance;

            int xDistance = Math.Abs(currentX - target.x);

            int yDistance = Math.Abs(currentY - target.y);

            if (xDistance > yDistance)
            {
                distance =  yDistance  * (xDistance - yDistance);
            }
            else
            {
                distance = xDistance  * (yDistance - xDistance);
            }

            distance = (Math.Abs(currentX - target.x) + Math.Abs(currentY - target.y));

            return distance;

    

        }


        

    }
}
