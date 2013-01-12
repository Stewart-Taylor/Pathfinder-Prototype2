using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathfinder_Prototype_2
{
     class DstarB
    {

        //Private Member variables
        private List<State> path = new List<State>();
        private double C1;
        private double k_m;
        private State s_start = new State();
        private State s_goal = new State();
        private State s_last = new State();
        private int maxSteps;

        private List<State> openList = new List<State>();
        //Change back to private****
        public Dictionary<State, CellInfo> cellHash = new Dictionary<State, CellInfo>();
        private Dictionary<State, float> openHash = new Dictionary<State, float>();

        //Constants
        private double M_SQRT2 = Math.Sqrt(2.0);

        //Default constructor
        public DstarB()
        {
            maxSteps = 80000;
            C1 = 1;
        }



        /*
         * Initialise Method
         * @params start and goal coordinates
         */
        public void init(int sX, int sY, int gX, int gY)
        {
            cellHash.Clear();
            path.Clear();
            openHash.Clear();
            openList.Clear();
          //  while (openList.Count != 0) openList.poll();

            k_m = 0;

            s_start.x = sX;
            s_start.y = sY;
            s_goal.x = gX;
            s_goal.y = gY;

            CellInfo tmp = new CellInfo();
            tmp.g = 0;
            tmp.rhs = 0;
            tmp.cost = C1;

            cellHash.Add(s_goal, tmp);

            tmp = new CellInfo();
            tmp.g = tmp.rhs = heuristic(s_start, s_goal);
            tmp.cost = C1;
            cellHash.Add(s_start, tmp);
            s_start = calculateKey(s_start);

            s_last = s_start;

        }

        /*
         * CalculateKey(state u)
         * As per [S. Koenig, 2002]
         */
        private State calculateKey(State u)
        {
            double val = Math.Min(getRHS(u), getG(u));

            u.k.setFirst(val + heuristic(u, s_start) + k_m);
            u.k.setSecond(val);

            return u;
        }

        /*
         * Returns the rhs value for state u.
         */
        private double getRHS(State u)
        {
            if (u == s_goal) return 0;

            //if the cellHash doesn't contain the State u
            if (cellHash.ContainsKey(u) == false)
            // if (contains(cellHash, u) == false)
            {
                return heuristic(u, s_goal);
            }
            return cellHash[u].rhs;
        }

        /*
         * Returns the g value for the state u.
         */
        private double getG(State u)
        {
            //if the cellHash doesn't contain the State u
            if (cellHash.ContainsKey(u) == false)
         //   if (contains(cellHash ,u) == false)
            {
                return heuristic(u, s_goal);
            }
            return cellHash[u].g;
        }


        private bool contains(Dictionary<State, CellInfo> cellHash, State u)
        {

            foreach (KeyValuePair<State, CellInfo> v in cellHash)
            {
                if ((v.Key.x == u.x) && (v.Key.y == u.y))
                {
                    return true;
                }
            }

            return false;
        }




        /*
         * Pretty self explanatory, the heuristic we use is the 8-way distance
         * scaled by a constant C1 (should be set to <= min cost)
         */
        private double heuristic(State a, State b)
        {
            return eightCondist(a, b) * C1;
        }

        /*
         * Returns the 8-way distance between state a and state b
         */
        private double eightCondist(State a, State b)
        {
            double temp;
            double min = Math.Abs(a.x - b.x);
            double max = Math.Abs(a.y - b.y);
            if (min > max)
            {
                temp = min;
                min = max;
                max = temp;
            }
            return ((M_SQRT2 - 1.0) * min + max);

        }

        public bool replan()
        {
            path.Clear();

            int res = computeShortestPath();
            if (res < 0)
            {
                Console.Out.WriteLine("No Path to Goal");
                return false;
            }

            LinkedList<State> n = new LinkedList<State>();
            State cur = s_start;

            if (getG(s_start) == Double.PositiveInfinity)
            {
                 Console.Out.WriteLine("No Path to Goal");
                return false;
            }

            while (cur.neq(s_goal))
            {
                path.Add(cur);
                n = new LinkedList<State>();
                n = getSucc(cur);

                if (n.Count == 0)
                {
                    Console.Out.WriteLine("No Path to Goal");
                    return false;
                }

                double cmin = Double.PositiveInfinity;
                double tmin = 0;
                State smin = new State();

                foreach (State i in n)     //(State i : n)
                {
                    double val = cost(cur, i);
                    double val2 = trueDist(i, s_goal) + trueDist(s_start, i);
                    val += getG(i);

                    if (close(val, cmin))
                    {
                        if (tmin > val2)
                        {
                            tmin = val2;
                            cmin = val;
                            smin = i;
                        }
                    }
                    else if (val < cmin)
                    {
                        tmin = val2;
                        cmin = val;
                        smin = i;
                    }
                }
                n.Clear();
                cur = new State(smin);
                //cur = smin;
            }
            path.Add(s_goal);
            return true;
        }

        /*
         * As per [S. Koenig,2002] except for two main modifications:
         * 1. We stop planning after a number of steps, 'maxsteps' we do this
         *    because this algorithm can plan forever if the start is surrounded  by obstacles
         * 2. We lazily remove states from the open list so we never have to iterate through it.
         */
        private int computeShortestPath()
        {
            LinkedList<State> s = new LinkedList<State>();

            if (openList.Count ==0) return 1;

            int k = 0;
            while ((openList.Count !=0) && (openList.Last().lt(s_start = calculateKey(s_start))) ||  (getRHS(s_start) != getG(s_start)))
            {

                if (k++ > maxSteps)
                {
                    Console.Out.WriteLine("At maxsteps");
                    return -1;
                }

                State u;

                bool test = (getRHS(s_start) != getG(s_start));

                //lazy remove
                while (true)
                {
                    if (openList.Count ==0) return 1;
                    u = openList.Last();
                    openList.Remove(u);

                    if (!isValid(u)) continue;
                    if (!(u.lt(s_start)) && (!test)) return 2;
                    break;
                }

                openHash.Remove(u);

                State k_old = new State(u);

                if (k_old.lt(calculateKey(u)))
                { //u is out of date
                    insert(u);
                }
                else if (getG(u) > getRHS(u))
                { //needs update (got better)
                    setG(u, getRHS(u));
                    s = getPred(u);
                    foreach (State i in s)         // (State i : s) 
                    {
                        updateVertex(i);
                    }
                }
                else
                {						 // g <= rhs, state has got worse
                    setG(u, Double.PositiveInfinity);
                    s = getPred(u);

                    foreach (State i in s)
                    {
                        updateVertex(i);
                    }
                    updateVertex(u);
                }
            } //while
            return 0;
        }

        /*
         * Returns a list of successor states for state u, since this is an
         * 8-way graph this list contains all of a cells neighbours. Unless
         * the cell is occupied, in which case it has no successors.
         */
        private LinkedList<State> getSucc(State u)
        {
            LinkedList<State> s = new LinkedList<State>();
            State tempState;

            if (occupied(u)) return s;

            //Generate the successors, starting at the immediate right,
            //Moving in a clockwise manner
            tempState = new State(u.x + 1, u.y, new Pair<double,double>(-1.0, -1.0));
            s.AddFirst(tempState);
            tempState = new State(u.x + 1, u.y + 1, new Pair<double, double>(-1.0, -1.0));
            s.AddFirst(tempState);
            tempState = new State(u.x, u.y + 1, new Pair<double, double>(-1.0, -1.0));
            s.AddFirst(tempState);
            tempState = new State(u.x - 1, u.y + 1, new Pair<double, double>(-1.0, -1.0));
            s.AddFirst(tempState);
            tempState = new State(u.x - 1, u.y, new Pair<double, double>(-1.0, -1.0));
            s.AddFirst(tempState);
            tempState = new State(u.x - 1, u.y - 1, new Pair<double, double>(-1.0, -1.0));
            s.AddFirst(tempState);
            tempState = new State(u.x, u.y - 1, new Pair<double, double>(-1.0, -1.0));
            s.AddFirst(tempState);
            tempState = new State(u.x + 1, u.y - 1, new Pair<double, double>(-1.0, -1.0));
            s.AddFirst(tempState);

            return s;
        }

        /*
         * Returns a list of all the predecessor states for state u. Since
         * this is for an 8-way connected graph, the list contains all the
         * neighbours for state u. Occupied neighbours are not added to the list
         */
        private LinkedList<State> getPred(State u)
        {
            LinkedList<State> s = new LinkedList<State>();
            State tempState;

            tempState = new State(u.x + 1, u.y, new Pair<double, double>(-1, -1));
            if (!occupied(tempState)) s.AddFirst(tempState);
            tempState = new State(u.x + 1, u.y + 1, new Pair<double, double>(-1, -1));
            if (!occupied(tempState)) s.AddFirst(tempState);
            tempState = new State(u.x, u.y + 1, new Pair<double, double>(-1, -1));
            if (!occupied(tempState)) s.AddFirst(tempState);
            tempState = new State(u.x - 1, u.y + 1, new Pair<double, double>(-1,-1));
            if (!occupied(tempState)) s.AddFirst(tempState);
            tempState = new State(u.x - 1, u.y, new Pair<double, double>(-1, -1));
            if (!occupied(tempState)) s.AddFirst(tempState);
            tempState = new State(u.x - 1, u.y - 1, new Pair<double, double>(-1, -1));
            if (!occupied(tempState)) s.AddFirst(tempState);
            tempState = new State(u.x, u.y - 1, new Pair<double, double>(-1, -1));
            if (!occupied(tempState)) s.AddFirst(tempState);
            tempState = new State(u.x + 1, u.y - 1, new Pair<double, double>(-1, -1));
            if (!occupied(tempState)) s.AddFirst(tempState);

            return s;
        }


        /*
         * Update the position of the agent/robot.
         * This does not force a replan.
         */
        public void updateStart(int x, int y)
        {
            s_start.x = x;
            s_start.y = y;

            k_m += heuristic(s_last, s_start);

            s_start = calculateKey(s_start);
            s_last = s_start;

        }

        /*
         * This is somewhat of a hack, to change the position of the goal we
         * first save all of the non-empty nodes on the map, clear the map, move the
         * goal and add re-add all of the non-empty cells. Since most of these cells
         * are not between the start and goal this does not seem to hurt performance
         * too much. Also, it frees up a good deal of memory we are probably not
         * going to use.
         */
        public void updateGoal(int x, int y)
	{
		List<Pair<ipoint2, Double> > toAdd = new List<Pair<ipoint2, Double> >();
		Pair<ipoint2, Double> tempPoint;

	//	for (Map.Entry<State,CellInfo> entry : cellHash.entrySet())
     //   foreach(Map.Entry<State,CellInfo> entry in cellHash)
    //    {
	//		if (!close(entry.getValue().cost, C1)) {
	//			tempPoint = new Pair<double,double>(
	//						new ipoint2(entry.getKey().x,entry.getKey().y),
	//						entry.getValue().cost);
	//			toAdd.Add(tempPoint);
	//		}
	//	}

		cellHash.Clear();
		openHash.Clear();

		while(openList.Count != 0)
			//openList.poll();

		k_m = 0;

		s_goal.x = x;
		s_goal.y = y;

		CellInfo tmp = new CellInfo();
		tmp.g = tmp.rhs = 0;
		tmp.cost = C1;

		cellHash.Add(s_goal, tmp);

		tmp = new CellInfo();
		tmp.g = tmp.rhs = heuristic(s_start, s_goal);
		tmp.cost = C1;
		cellHash.Add(s_start, tmp);
		s_start = calculateKey(s_start);

		s_last = s_start;

	//	ArgIterator<Pair<ipoint2,Double> > iterator = toAdd.iterator();
            
	//	while(iterator.hasNext())
    //    {
	//		tempPoint = iterator.next();
	//		updateCell(tempPoint.first().x, tempPoint.first().y, tempPoint.second());
	//	}

        for (int i = 0; i < toAdd.Count; i++)
        {
            updateCell(toAdd[i].first().x, toAdd[i].first().y, toAdd[i].second());
        }

      


	}

        /*
         * As per [S. Koenig, 2002]
         */
        private void updateVertex(State u)
	{
		LinkedList<State> s = new LinkedList<State>();

		if (u.neq(s_goal)) {
			s = getSucc(u);
			double tmp = Double.PositiveInfinity;
			double tmp2;

			foreach(State i in s) // (State i : s) 
            {
				tmp2 = getG(i) + cost(u,i);
				if (tmp2 < tmp) tmp = tmp2;
			}
			if (!close(getRHS(u),tmp)) setRHS(u,tmp);
		}

		if (!close(getG(u),getRHS(u))) insert(u);
	}

        /*
         * Returns true if state u is on the open list or not by checking if
         * it is in the hash table.
         */
        private bool isValid(State u)
        {
            if (openHash[u] == null) return false;
            if (!close(keyHashCode(u), openHash[u])) return false;
            return true;
        }

        /*
         * Sets the G value for state u
         */
        private void setG(State u, double g)
        {
            makeNewCell(u);
            cellHash[u].g = g;
        }

        /*
         * Sets the rhs value for state u
         */
        private void setRHS(State u, double rhs)
        {
            makeNewCell(u);
            cellHash[u].rhs = rhs;
        }

        /*
         * Checks if a cell is in the hash table, if not it adds it in.
         */
        private void makeNewCell(State u)
        {
            if (cellHash.ContainsKey(u))
          //  if (contains(cellHash, u))
            {
                return;
            }

            CellInfo tmp = new CellInfo();
            tmp.g = tmp.rhs = heuristic(u, s_goal);
            tmp.cost = C1;
            cellHash.Add(u, tmp);
        }

        /*
         * updateCell as per [S. Koenig, 2002]
         */
        public void updateCell(int x, int y, double val)
        {
            State u = new State();
            u.x = x;
            u.y = y;

            if ((u.eq(s_start)) || (u.eq(s_goal))) return;

            makeNewCell(u);
            cellHash[u].cost = val;
            updateVertex(u);
        }

        /*
         * Inserts state u into openList and openHash
         */
        private void insert(State u)
        {
            //iterator cur
            float csum;

            u = calculateKey(u);
            //cur = openHash.find(u);
            csum = keyHashCode(u);

            // return if cell is already in list. TODO: this should be
            // uncommented except it introduces a bug, I suspect that there is a
            // bug somewhere else and having duplicates in the openList queue
            // hides the problem...
            //if ((cur != openHash.end()) && (close(csum,cur->second))) return;

            openHash.Add(u, csum);
            openList.Add(u);
        }

        /*
         * Returns the key hash code for the state u, this is used to compare
         * a state that has been updated
         */
        private float keyHashCode(State u)
        {
            return (float)(u.k.first() + 1193 * u.k.second());
        }

        /*
         * Returns true if the cell is occupied (non-traversable), false
         * otherwise. Non-traversable are marked with a cost < 0
         */
        private bool occupied(State u)
        {
            //if the cellHash does not contain the State u
            if (cellHash.ContainsKey(u) == false)
          //  if (contains(cellHash, u) == false)
            {
                return false;
            }
            return (cellHash[u].cost < 0);
        }

        /*
         * Euclidean cost between state a and state b
         */
        private double trueDist(State a, State b)
        {
            float x = a.x - b.x;
            float y = a.y - b.y;
            return Math.Sqrt(x * x + y * y);
        }

        /*
         * Returns the cost of moving from state a to state b. This could be
         * either the cost of moving off state a or onto state b, we went with the
         * former. This is also the 8-way cost.
         */
        private double cost(State a, State b)
        {
            int xd = Math.Abs(a.x - b.x);
            int yd = Math.Abs(a.y - b.y);
            double scale = 1;

            if (xd + yd > 1) scale = M_SQRT2;

          //  if (cellHash.ContainsKey(a) == false) return scale * C1;
            if (cellHash.ContainsKey(a) == false)
            {
                return scale * C1;
            }

            return scale * cellHash[a].cost;
        }

        /*
         * Returns true if x and y are within 10E-5, false otherwise
         */
        private bool close(double x, double y)
        {
            if (x == Double.PositiveInfinity && y == Double.PositiveInfinity) return true;
            return (Math.Abs(x - y) < 0.00001);
        }

        public List<State> getPath()
        {
            return path;
        }


        public List<PathNode> getPathP()
        {
            List<PathNode> path = new List<PathNode>();

            foreach (State s in getPath())
            {
                PathNode p = new PathNode();
                p.x = s.x;
                p.y = s.y;
                path.Add(p);
            }

            return path;
        }


        public static void main(String[] args)
        {
            DstarB pf = new DstarB();
            pf.init(0, 1, 3, 1);
            pf.updateCell(2, 1, -1);
            pf.updateCell(2, 0, -1);
            pf.updateCell(2, 2, -1);
            pf.updateCell(3, 0, -1);

            Console.Out.WriteLine("Start node: (0,1)");
            Console.Out.WriteLine("End node: (3,1)");

            //Time the replanning
       //     long begin = System.currentTimeMillis();
            pf.replan();
            pf.updateGoal(3, 2);
        //    long end = System.currentTimeMillis();

        //    Console.Out.WriteLine("Time: " + (end - begin) + "ms");

            List<State> path = pf.getPath();
            foreach (State i in path)          //(State i : path)
            {
                Console.Out.WriteLine("x: " + i.x + " y: " + i.y);
            }

        }
    }

    class CellInfo
    {
        public double g = 0;
        public double rhs = 0;
        public double cost = 0;
    }

    class ipoint2
    {
        public int x = 0;
        public int y = 0;

        //default constructor
        public ipoint2()
        {

        }

        //overloaded constructor
        public ipoint2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}