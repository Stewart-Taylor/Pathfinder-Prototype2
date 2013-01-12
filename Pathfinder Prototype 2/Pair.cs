using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathfinder_Prototype_2
{
    class Pair<T, U>
    {

    /**
     * One of the two values, for the declared type T.
     */
    private T object1;
    /**
     * One of the two values, for the declared type U.
     */
    private U object2;
    private bool object1Null;
    private bool object2Null;
    private bool dualNull;

    /**
     * Constructs a new <code>Pair&lt;T, U&gt;</code> with T object1 and U object2 as
     * its values. The order of the arguments is of no consequence. One or both of
     * the values may be <code>null</code> and both values may be the same object.
     *
     * @param object1 T to serve as one value.
     * @param object2 U to serve as the other value.
     */
    public Pair(T object1, U object2) {

        this.object1 = object1;
        this.object2 = object2;
        object1Null = object1 == null;
        object2Null = object2 == null;
        dualNull = object1Null && object2Null;

    }

    /**
     * Gets the value of this Pair provided as the first argument in the constructor.
     *
     * @return a value of this Pair.
     */
    public T first() {

        return object1;

    }

    /**
     * Gets the value of this Pair provided as the second argument in the constructor.
     *
     * @return a value of this Pair.
     */
    public U second() {

        return object2;

    }

    /*
     * Sets the value of the first Pair
     */
    public void setFirst(T object1)
	{
	   this.object1 = object1;
	   object1Null = object1 == null;
	   dualNull = object1Null && object2Null;
    }

	/*
	 * Sets the value of the second pair
	 */
	public void setSecond(U object2)
	{
		this.object2 = object2;
		object2Null = object2 == null;
		dualNull = object1Null && object2Null;
	}


    /**
     * Returns a shallow copy of this Pair. The returned Pair is a new instance
     * created with the same values as this Pair. The values themselves are not
     * cloned.
     *
     * @return a clone of this Pair.
     */
    
    public Pair<T, U> clone() {

        return new Pair<T, U>(object1, object2);

    }

    /**
     * Indicates whether some other object is "equal" to this one.
     * This Pair is considered equal to the object if and only if
     * <ul>
     * <li>the Object argument is not null,
     * <li>the Object argument has a runtime type Pair or a subclass,
     * </ul>
     * AND
     * <ul>
     * <li>the Object argument refers to this pair
     * <li>OR this pair's values are both null and the other pair's values are both null
     * <li>OR this pair has one null value and the other pair has one null value and
     * the remaining non-null values of both pairs are equal
     * <li>OR both pairs have no null values and have value tuples &lt;v1, v2> of
     * this pair and &lt;o1, o2> of the other pair so that at least one of the
     * following statements is true:
     * <ul>
     * <li>v1 equals o1 and v2 equals o2
     * <li>v1 equals o2 and v2 equals o1
     * </ul>
     * </ul>
     * In any other case (such as when this pair has two null parts but the other
     * only one) this method returns false.<p>
     * The type parameters that were used for the other pair are of no importance.
     * A Pair&lt;T, U> can return <code>true</code> for equality testing with
     * a Pair&lt;T, V> even if V is neither a super- nor subtype of U, should
     * the the value equality checks be positive or the U and V type values
     * are both <code>null</code>. Type erasure for parameter types at compile
     * time means that type checks are delegated to calls of the <code>equals</code>
     * methods on the values themselves.
     *
     * @param obj the reference object with which to compare.
     * @return true if the object is a Pair equal to this one.
     */
    public bool equals(Object obj) {

        if(obj == null)
            return false;

        if(this == obj)
            return true;


        if(obj == this)
        {
            return true;
        }

        return false;

     //   if(!(obj is Pair<?, ?>))
       //     return false;

        /*

        Pair<?, ?> otherPair = (Pair<?, ?>)obj;

        if(dualNull)
            return otherPair.dualNull;

        //After this we're sure at least one part in this is not null

        if(otherPair.dualNull)
            return false;

        //After this we're sure at least one part in obj is not null

        if(object1Null) {
            if(otherPair.object1Null) //Yes: this and other both have non-null part2
                return object2.equals(otherPair.object2);
            else if(otherPair.object2Null) //Yes: this has non-null part2, other has non-null part1
                return object2.equals(otherPair.object1);
            else //Remaining case: other has no non-null parts
                return false;
        } else if(object2Null) {
            if(otherPair.object2Null) //Yes: this and other both have non-null part1
                return object1.equals(otherPair.object1);
            else if(otherPair.object1Null) //Yes: this has non-null part1, other has non-null part2
                return object1.equals(otherPair.object2);
            else //Remaining case: other has no non-null parts
                return false;
        } else {
            //Transitive and symmetric requirements of equals will make sure
            //checking the following cases are sufficient
            if(object1.equals(otherPair.object1))
                return object2.equals(otherPair.object2);
            else if(object1.equals(otherPair.object2))
                return object2.equals(otherPair.object1);
            else
                return false;
        }
         * */

    }

    /**
     * Returns a hash code value for the pair. This is calculated as the sum
     * of the hash codes for the two values, wherein a value that is <code>null</code>
     * contributes 0 to the sum. This implementation adheres to the contract for
     * <code>hashCode()</code> as specified for <code>Object()</code>. The returned
     * value hash code consistently remain the same for multiple invocations
     * during an execution of a Java application, unless at least one of the pair
     * values has its hash code changed. That would imply information used for
     * equals in the changed value(s) has also changed, which would carry that
     * change onto this class' <code>equals</code> implementation.
     *
     * @return a hash code for this Pair.
     */
    public int hashCode()
    {

        int hashCode = object1Null ? 0 : object1.GetHashCode();
        hashCode += (object2Null ? 0 : object2.GetHashCode());
        return hashCode;

    }

}
}