package it.mintlab.desktopjava.tupleengine;

	public abstract class Term
	{
		public abstract boolean unify(Term t, java.util.ArrayList<Var> vars1, java.util.ArrayList<Var> vars2);
		public abstract Object getRefObject();

		public final boolean unify(Term t, java.util.HashMap<String, Term> varMap)
		{
			java.util.ArrayList<Var> vars1 = new java.util.ArrayList<Var>();
			java.util.ArrayList<Var> vars2 = new java.util.ArrayList<Var>();
			synchronized (this)
			{
				boolean result = unify(t, vars1, vars2);
				for (Var v : vars1)
				{
					varMap.put(v.getName(), v.getValue());
					v.deunify();
				}
				return result;
			}
		}

		public final boolean match(Term t)
		{
			java.util.ArrayList<Var> vars1 = new java.util.ArrayList<Var>();
			java.util.ArrayList<Var> vars2 = new java.util.ArrayList<Var>();
			synchronized (this)
			{
				boolean result = unify(t, vars1, vars2);
				for (Var v : vars1)
				{
					v.deunify();
				}
				for (Var v : vars2)
				{
					v.deunify();
				}
				return result;
			}
		}

		@Override
		public boolean equals(Object obj)
		{
			return this.match((Term)obj);
		}
	}