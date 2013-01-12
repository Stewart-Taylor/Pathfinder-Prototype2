using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathfinder_Prototype_2
{
    class State
    {
        public int x = 0;
        public int y = 0;
        public Pair<Double, Double> k = new Pair<double,double>(0.0f, 0.0f);


        //Default constructor
        public State()
        {

        }

        //Overloaded constructor
        public State(int x, int y, Pair<Double, Double> k)
        {
            this.x = x;
            this.y = y;
            this.k = k;
        }

        //Overloaded constructor
        public State(State other)
        {
            this.x = other.x;
            this.y = other.y;
            this.k = other.k;
        }

        //Equals
        public bool eq(State s2)
        {
            return ((this.x == s2.x) && (this.y == s2.y));
        }

        //Not Equals
        public bool neq(State s2)
        {
            return ((this.x != s2.x) || (this.y != s2.y));
        }

        //Greater than
        public bool gt(State s2)
        {
            if (k.first() - 0.00001 > s2.k.first()) return true;
            else if (k.first() < s2.k.first() - 0.00001) return false;
            return k.second() > s2.k.second();
        }

        //Less than or equal to
        public bool lte(State s2)
        {
            if (k.first() < s2.k.first()) return true;
            else if (k.first() > s2.k.first()) return false;
            return k.second() < s2.k.second() + 0.00001;
        }

        //Less than
        public bool lt(State s2)
        {
            return true;
            if (k.first() + 0.000001 < s2.k.first()) return true;
            else if (k.first() - 0.000001 > s2.k.first()) return false;
            return k.second() < s2.k.second();
        }

        //CompareTo Method. This is necessary when this class is used in a priority queue
        public int compareTo(Object that)
        {
            //This is a modified version of the gt method
            State other = (State)that;
            if (k.first() - 0.00001 > other.k.first()) return 1;
            else if (k.first() < other.k.first() - 0.00001) return -1;
            if (k.second() > other.k.second()) return 1;
            else if (k.second() < other.k.second()) return -1;
            return 0;
        }

        //Override the CompareTo function for the HashMap usage

        public int hashCode()
        {
            return this.x + 34245 * this.y;
        }

        public bool equals(Object aThat)
     {
		//check for self-comparison
		if ( this == aThat ) return true;

		//use instanceof instead of getClass here for two reasons
		//1. if need be, it can match any supertype, and not just one class;
		//2. it renders an explict check for "that == null" redundant, since
		//it does the check for null already - "null instanceof [type]" always
		//returns false. (See Effective Java by Joshua Bloch.)
	//	if ( !(aThat instanceof State) ) return false;
		//Alternative to the above line :
		//if ( aThat == null || aThat.getClass() != this.getClass() ) return false;
           if ( aThat == null || aThat != this ) 
           {
               return false;
           }

		//cast to native object is now safe
		State that = (State)aThat;

		//now a proper field-by-field evaluation can be made
		if (this.x == that.x && this.y == that.y) return true;
		return false;

	}

    }
}