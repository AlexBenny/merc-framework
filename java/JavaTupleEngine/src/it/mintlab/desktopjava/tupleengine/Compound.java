package it.mintlab.desktopjava.tupleengine;

	public class Compound extends Term implements IRefObject
	{
		private String rootValue;
		private java.util.ArrayList<Term> leafValues;
		private Object refObject;
		private boolean withRefObject;

		public Compound(String rootValue, java.util.ArrayList<Term> leafValues)
		{
			this.rootValue = rootValue;
			this.leafValues = leafValues;
			this.withRefObject = false;
		}

		public final void setRefObject(Object obj)
		{
			refObject = obj;
			withRefObject = true;
		}

		@Override
		public String toString()
		{
			String leafs = "";
			for (Term leaf : leafValues)
			{
				leafs += leaf.toString() + ",";
			}
			return rootValue + "(" + leafs.substring(0, leafs.length() - 1) + ")";
		}

		@Override
		public boolean unify(Term t, java.util.ArrayList<Var> vars1, java.util.ArrayList<Var> vars2)
		{
			if (t instanceof Compound)
			{
				Compound tc = (Compound)t;
				if (this.rootValue.equals(tc.rootValue) && this.leafValues.size() == tc.leafValues.size())
				{
					for (int i = 0; i < leafValues.size(); i++)
					{
						if (!this.leafValues.get(i).unify(tc.leafValues.get(i), vars1, vars2))
						{
							return false;
						}
					}
					return true;
				}
			}
			if (t instanceof Var)
			{
				return t.unify(this, vars2, vars1);
			}
			return false;
		}

		@Override
		public Object getRefObject()
		{
			return withRefObject ? refObject : rootValue;
		}
	}