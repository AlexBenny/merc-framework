package it.mintlab.desktopjava.tupleengine;

	public class Var extends Term
	{
		private String varName;
		private Term value;
		private boolean unified;


		public Var(String varName)
		{
			this.varName = varName;
			this.value = null;
			this.unified = false;
		}

		public final String getName()
		{
			return varName;
		}

		public final Term getValue()
		{
			if (unified && value instanceof Var)
			{
				return ((Var)value).getValue();
			}
			return value;
		}

		public final void setValue(Term value)
		{
			this.value = value;
			this.unified = true;
		}

		public final void deunify()
		{
			this.value = null;
			this.unified = false;
		}

		public final boolean isUnified()
		{
			if (!unified)
			{
				return false;
			}
			if (value instanceof Var)
			{
				return ((Var)value).isUnified();
			}
			return true;
		}

		@Override
		public String toString()
		{
			return isUnified() ? varName + "=" + getValue().toString() : varName;
		}

		@Override
		public boolean unify(Term t, java.util.ArrayList<Var> vars1, java.util.ArrayList<Var> vars2)
		{
			if (unified)
			{
				return value.unify(t, vars1, vars2);
			}
			else
			{
				setValue(t);
				vars1.add(this);
				return true;
			}
		}

		@Override
		public Object getRefObject()
		{
			return isUnified() ? getValue().getRefObject() : null;
		}

	}