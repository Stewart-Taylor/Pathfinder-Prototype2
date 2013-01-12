using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Pathfinder_Prototype_2
{
    public enum Tag
    {
        NEW,
        CLOSED,
        OPEN
    };

    //    Used to represent values for each square.
    public class Node
    {
        public int i = 0;         // x location
        public int j = 0;         // y location
        public Node parent = null;      // Parent with lowest cost
        public int Gcost = 0;         // Distance cost
        public int Hcost = 0;         // Heuristic
        public int value = 0;         // V = G + H
        public bool passed = false;     // Open list marker
        public Tag tag = Tag.NEW;
    }

    class DStar
    {
        Node goal;
        Node start;
        public Node[,] node_array;
        public float[,] actual_cost_array;
        public bool STARTED;
        int k_min;
        Node current;

        List<Node> node_list;


        int xLength;
        int yLength;

        public DStar(float[,] knownMap)
        {
            goal = new Node();
            start = new Node();

            xLength = knownMap.GetLength(0);
            yLength = knownMap.GetLength(1);

            node_list = new List<Node>();
            node_array = new Node[xLength, yLength];
            actual_cost_array = new float[xLength, yLength];

            for (int y = 0; y < yLength; y++)
            {
                for (int x = 0; x < xLength; x++)
                {
                    node_array[y, x] = new Node();
                    node_array[y, x].i = y;
                    node_array[y, x].j = x;
                    node_array[y, x].tag = Tag.NEW;
                    node_array[y, x].Gcost = 1;
                    node_array[y, x].Hcost = 0;
                }
            }

            STARTED = false;
        }



        //Does the initial D* setup.
        public void Start(int start_x, int start_y, int end_x, int end_y)
        {
            //Begin setup.
            goal = node_array[end_x, end_y];
            start = node_array[start_x, start_y];
            Insert(goal, 0);

            int k_min = 0;

            while (start.tag != Tag.CLOSED && k_min != -1)
            {
                k_min = ProcessState();
            }

            current = start;
            STARTED = true;
            this.k_min = 0;
        }

        public PathNode Think(float[,] actual_cost_array)
        {
            //Updates with percepts.
            this.actual_cost_array = actual_cost_array;
            Node move = null;
            

            //Begin the dynamic portion of D*.   Procede until you reach goal.
            if (!Equal(current, goal))
            {
                bool found_move = false;

                //D* will loop till it can make one move.
                while (!found_move)
                {
                    int estimated_cost = current.Hcost - current.parent.Hcost;
                    float actual_cost = actual_cost_array[current.parent.i, current.parent.j];


                    if (estimated_cost != actual_cost)
                    {
                        //Update the cost model to reflect actual cost.
                        ModifyCost(current.parent, current, actual_cost);

                        //Propogate the cost change out.
                        int kk_min = -2;
                        int h_cost = current.Hcost;

                        while (kk_min < h_cost)
                        {
                            if (kk_min != -1)
                            {
                                kk_min = ProcessState();
                                h_cost = current.Hcost;
                            }
                            else
                            {
                                //Error,  path cannot be found.    
                                break;
                            }
                        }
                    }

                    //Delete(current);
                    move = current;
                    node_list.Add(current);         //Make the move.
                    current = current.parent;
                    found_move = true;

                    UpdateWithPercepts(current, actual_cost_array);
                }

            }
            else
            {
                STARTED = false;
            }

            PathNode nodeTemp = null;
            if (move != null)
            {
                nodeTemp = new PathNode();
                nodeTemp.x = move.i;
                nodeTemp.y = move.j;
            }

            return nodeTemp;
        }

        //Updates the cells around Zippy within his percept range based on percept knowledge.
        public void UpdateWithPercepts(Node s, float[,] actual_cost_array)
        {
            int x_start = s.j - 2;
            if (x_start < 0)
            {
                x_start = 0;
            }

            int x_end = s.j + 3;
            if (x_end > xLength)
            {
                x_end = xLength;
            }

            int y_start = s.i - 2;
            if (y_start < 0)
            {
                y_start = 0;
            }
            int y_end = s.i + 3;
            if (y_end > yLength)
            {
                y_end = yLength;
            }

            for (int y = y_start; y < y_end; y++)
            {
                for (int x = x_start; x < x_end; x++)
                {

                    node_array[y, x].Gcost = (int)actual_cost_array[x, y];
                   
                    if (actual_cost_array[y, x] <= 6)
                    {
                        node_array[y, x].Gcost = (int)actual_cost_array[x, y];
                    }
                    else
                    {
                       node_array[y, x].Gcost = 768;
                    }
                    
                   
                }
            }
        }

        /** Core function of the D* search algorithm. Propogates g-costs.*/
        int ProcessState()
        {
            Node X = MinState();

            if (X == null)
            {
                return -1;
            }

            List<Node> neighbors = Successors(X);

            int k_old = GetKMin();
            Delete(X);						//Removes min-state from open list.

            if (k_old < X.Hcost)
            {
                foreach (Node Y in neighbors)
                {
                    if (Y.Hcost <= k_old && X.Hcost > (Y.Hcost + c(Y, X)))
                    {
                        X.parent = Y;
                        X.Hcost = Y.Hcost + c(Y, X);
                    }
                }
            }
            if (k_old == X.Hcost)
            {
                foreach (Node Y in neighbors)
                {
                    if ((Y.tag == Tag.NEW) ||
                        ((Y.parent == X) && Y.Hcost != (X.Hcost + c(X, Y))) ||
                        ((Y.parent != X) && Y.Hcost > (X.Hcost + c(X, Y))))
                    {

                        Y.parent = X;
                        Insert(Y, X.Hcost + c(X, Y));
                    }
                }
            }
            else
            {
                foreach (Node Y in neighbors)
                {
                    if (Y.tag == Tag.NEW ||
                        ((Y.parent == X) && (Y.Hcost != (X.Hcost + c(X, Y)))))
                    {
                        Y.parent = X;
                        Insert(Y, X.Hcost + c(X, Y));
                    }
                    else
                    {
                        if ((Y.parent != X) && (Y.Hcost > (X.Hcost + c(X, Y))))
                        {
                            Insert(X, X.Hcost);
                        }
                        else
                        {
                            if ((Y.parent != X) &&
                                (X.Hcost > (Y.Hcost + c(Y, X))) &&
                                (Y.tag == Tag.CLOSED) &&
                                (Y.Hcost > k_old))
                            {
                                Insert(Y, Y.Hcost);
                            }
                        }
                    }
                }
            }

            return GetKMin();
        }

        /** Updates the cost function for a particular node arc. */
        int ModifyCost(Node X, Node Y, float cval)
        {
            X.Gcost = (int)cval;

            if (X.tag == Tag.CLOSED)
            {
                Insert(X, X.Hcost);
            }

            return GetKMin();
        }

        //Returns the state with the lowest k-value in the open node list.
        Node MinState()
        {
            Node min_state = null;
            float key = 9999999.0f;

            for (int y = 0; y < yLength; y++)
            {
                for (int x = 0; x < xLength; x++)
                {
                    Node s = node_array[y, x];

                    if (s.value < key && s.tag == Tag.OPEN)
                    {
                        key = s.value;
                        min_state = s;
                    }
                }
            }

            return min_state;
        }

        /** Returns the lowest k-value on the open list.*/
        int GetKMin()
        {
            Node min_state = MinState();
            if (min_state != null)
            {
                return MinState().value;
            }

            return -1;
        }

        /** Adds a node to the open list. */
        void Insert(Node state, int h_new)
        {
            if (state.tag == Tag.NEW)
            {
                state.value = h_new;
            }
            else if (state.tag == Tag.OPEN)
            {

                state.value = Min(state.value, h_new);
            }
            else if (state.tag == Tag.CLOSED)
            {
                state.value = Min(state.Hcost, h_new);
            }

            state.Hcost = h_new;
            state.tag = Tag.OPEN;

        }

        /** Removes a node from the open list. */
        void Delete(Node state)
        {
            node_array[state.i, state.j].tag = Tag.CLOSED;
        }

        /** Returns the minimum of two values. */
        int Min(int x, int y)
        {
            if (x < y)
            {
                return x;
            }

            return y;
        }

        /** Returns the arc cost from y to x. */
        int c(Node X, Node Y)
        {
            return node_array[X.i, X.j].Gcost;
            //return 1;
        }

        /** Compares to squares by (x,y) location. */
        bool Equal(Node one, Node two)
        {
            if (one.i == two.i && one.j == two.j)
            {
                return true;
            }

            return false;
        }

        /** Returns all possible successor states of the current state. */
        List<Node> Successors(Node current)
        {
            List<Node> successors = new List<Node>();

            int x = current.j;
            int y = current.i;

            //Right
            if ((x + 1) <= 31)
            {
                Node square = node_array[y, x + 1];
                successors.Add(square);
            }

            //Left
            if ((x - 1) >= 0)
            {
                Node square = node_array[y, x - 1];
                successors.Add(square);
            }

            //Up
            if ((y + 1) <= 23)
            {
                Node square = node_array[y + 1, x];
                successors.Add(square);
            }

            //Down
            if ((y - 1) >= 0)
            {
                Node square = node_array[y - 1, x];
                successors.Add(square);
            }

            return successors;
        }



        public List<PathNode> getPath()
        {
            List<PathNode> path = new List<PathNode>();



            foreach (Node n in node_list)
            {
                PathNode newNode = new PathNode();
                newNode.x = n.i;
                newNode.y = n.j;
            }

            return path;
        }

    }
}