package it.mintlab.desktopjava.tupleengine;

	public class Int extends Term
	{
		private int value;

		public Int(int value)
		{
			this.value = value;
		}

		public final int getValue()
		{
			return value;
		}

		@Override
		public String toString()
		{
			return (new Integer(value)).toString();
		}



//VB & C# TO JAVA CONVERTER TODO TASK: There is no preprocessor in Java:
		///#region Term Membri di

		@Override
		public boolean unify(Term t, java.util.ArrayList<Var> vars1, java.util.ArrayList<Var> vars2)
		{
			if (t instanceof Float)
			{
				float f = ((Float)t).getValue();
				{
					if (f == this.value)
					{
						return true;
					}
				}
			}
			if (t instanceof Int)
			{
				int i = ((Int)t).getValue();
				{
					if (i == this.value)
					{
						return true;
					}
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
			return value;
		}

//VB & C# TO JAVA CONVERTER TODO TASK: There is no preprocessor in Java:
		///#endregion

	}