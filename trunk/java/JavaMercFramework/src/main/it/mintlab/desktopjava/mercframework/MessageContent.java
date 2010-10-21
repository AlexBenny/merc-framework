package it.mintlab.desktopjava.mercframework;

import it.mintlab.desktopjava.tupleengine.Term;
import it.mintlab.desktopjava.tupleengine.TupleEngine;

	public class MessageContent
	{
		private Category category = Category.forValue(0);
		private Term tuple;

		public enum Category
		{
			COMMAND,
			ERROR,
			LOG;

			public int getValue()
			{
				return this.ordinal();
			}

			public static Category forValue(int value)
			{
				return values()[value];
			}
		}

		public MessageContent(Category category, String tuple)
		{
			this(category, TupleEngine.parse(tuple));
		}

		public MessageContent(Category category, String tuple, Object[] pars)
		{
			this(category, TupleEngine.parse(tuple, pars));
		}

		public MessageContent(Category category, Term tuple)
		{
			this.category = category;
			this.tuple = tuple;
		}

		public final Category getCategory()
		{
			return category;
		}

		public final Term getTuple()
		{
			return tuple;
		}

		@Override
		public boolean equals(Object obj)
		{
			MessageContent mc = (MessageContent)obj;
			return (category == mc.getCategory() && tuple.match(mc.getTuple()));
		}
		
		@Override
		public int hashCode()
		{
			return getKey().hashCode();
		}

		@Override
		public String toString()
		{
			return tuple + " [" + category + "]";
		}

		public final String getKey()
		{
			String functor = tuple.toString().split(java.util.regex.Pattern.quote("("))[0]; //return the first part of tuple
			return "" + category + functor;
		}
	}