package it.mintlab.desktopjava.tupleengine;

	public class Atom extends Term implements IRefObject
	{
		private String value;
		private Object refObject;
		private boolean withRefObject;

		public Atom(String value)
		{
			this.value = value;
			withRefObject = false;
		}

		public final void setRefObject(Object obj)
		{
			refObject = obj;
			withRefObject = true;
		}

		@Override
		public String toString()
		{
			return value;
		}

		@Override
		public boolean unify(Term t, java.util.ArrayList<Var> vars1, java.util.ArrayList<Var> vars2)
		{
			if (t instanceof Atom)
			{
				Atom ta = (Atom)t;
				if (this.value.equals(ta.value))
				{
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
			return withRefObject ? refObject : value;
		}

	}