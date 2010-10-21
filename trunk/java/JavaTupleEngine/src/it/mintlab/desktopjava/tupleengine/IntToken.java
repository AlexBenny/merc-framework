package it.mintlab.desktopjava.tupleengine;

	public class IntToken implements IToken
	{
		private int valueInt;

		public IntToken(int number)
		{
			this.valueInt = number;
		}

		public final int getValue()
		{
			return valueInt;
		}

		@Override
		public String toString()
		{
			return (new Integer(valueInt)).toString();
		}

	}