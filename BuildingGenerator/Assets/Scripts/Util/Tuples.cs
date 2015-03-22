using UnityEngine;

namespace Tuples
{
	public class GenTuple2<A, B>
	{
		public A e0;
		public B e1;

		public GenTuple2(A e0, B e1)
		{
			this.e0 = e0;
			this.e1 = e1;
		}

		public override bool Equals(object obj)
		{
			if (obj is GenTuple2<A, B>)
			{
				GenTuple2<A, B> other = obj as GenTuple2<A, B>;
				return (other.e0.Equals(e0)) && (other.e1.Equals(e1));
			}
			return false;
		}

		public override string ToString()
		{
			return "(" + e0.ToString() + ", " + e1.ToString() + ")";
		}
	}

	public class GenTuple3<A, B, C>
	{
		public A e0;
		public B e1;
		public C e2;

		public GenTuple3(A e0, B e1, C e2)
		{
			this.e0 = e0;
			this.e1 = e1;
			this.e2 = e2;
		}

		public override bool Equals(object obj)
		{
			if (obj is GenTuple3<A, B, C>)
			{
				GenTuple3<A, B, C> other = obj as GenTuple3<A, B, C>;
				return (other.e0.Equals(e0)) && (other.e1.Equals(e1)) && (other.e2.Equals(e2));
			}
			return false;
		}

		public override string ToString()
		{
			return "(" + e0.ToString() + ", " + e1.ToString() + ", " + e2.ToString() + ")";
		}
	}

	[System.Serializable]
	public class IntTuple2
	{
		public int e0;
		public int e1;

		public IntTuple2(int e0, int e1)
		{
			this.e0 = e0;
			this.e1 = e1;
		}

		public override bool Equals(object obj)
		{
			if (obj is IntTuple2)
			{
				IntTuple2 other = obj as IntTuple2;
				return (other.e0 == e0) && (other.e1 == e1);
			}
			return false;
		}

		public override string ToString()
		{
			return "(" + e0.ToString() + ", " + e1.ToString() + ")";
		}
	}

	[System.Serializable]
	public class IntTuple3 : UnityEngine.Object
	{
		public int e0;
		public int e1;
		public int e2;

		public IntTuple3(int e0, int e1, int e2)
		{
			this.e0 = e0;
			this.e1 = e1;
			this.e2 = e2;
		}
		
		public override bool Equals(object obj)
		{
			if (obj is IntTuple3)
			{
				IntTuple3 other = obj as IntTuple3;
				return (other.e0 == e0) && (other.e1 == e1) && (other.e2 == e2);
			}
			return false;
		}

		public override string ToString()
		{
			return "(" + e0.ToString() + ", " + e1.ToString() + ", " + e2.ToString() + ")";
		}
	}
}
