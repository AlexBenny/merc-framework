package it.mintlab.desktopjava.tupleengine;

	public class VarToken implements IToken
	{
		private String var;

		public VarToken(String var)
		{
			this.var = var;
		}

		@Override
		public String toString()
		{
			return var;
		}


	}