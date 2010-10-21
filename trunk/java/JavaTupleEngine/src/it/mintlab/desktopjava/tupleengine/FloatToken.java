package it.mintlab.desktopjava.tupleengine;

	public class FloatToken implements IToken
	{
		private float valueFloat;


		public FloatToken(float number)
		{
			this.valueFloat = number;
		}

		public final float getValue()
		{
			return valueFloat;
		}

		@Override
		public String toString()
		{
			return (new Float(valueFloat)).toString();
		}
	}