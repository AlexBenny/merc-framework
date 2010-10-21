package it.mintlab.desktopjava.tupleengine;

	public class AtomToken implements IToken
	{
		private String atom;
		private Object parameter;
		private boolean withParameter;

		public AtomToken(String atom)
		{
			this.atom = atom;
			this.withParameter = false;
			this.parameter = null;
		}

		public final void setParameter(Object parameter)
		{
			this.parameter = parameter;
			this.withParameter = true;
		}

		public final Object getParameter()
		{
			return parameter;
		}

		@Override
		public String toString()
		{
			return atom;
		}

		public final boolean isWithParameter()
		{
			return this.withParameter;
		}


	}