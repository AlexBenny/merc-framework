package it.mintlab.desktopjava.tupleengine;

	public final class TupleEngine
	{
		public static Term parse(String tuple)
		{
			return new Parser().getTerms(new Lexer().getTokens(tuple));
		}

		public static Term parse(String tuple, Object[] parameters)
		{
			//java.util.HashMap<String, AtomToken> parTokens = new java.util.HashMap<String,AtomToken>();
			java.util.ArrayList<IToken> tokens = new Lexer().getTokens(tuple);
			for (IToken token : tokens)
			{
				int outvalue = 0;
				String tokenName = token.toString();
				if (token instanceof AtomToken && tokenName.startsWith("$")){
					try{
						outvalue = Integer.parseInt(tokenName.substring(1));
						((AtomToken)token).setParameter(parameters[outvalue]);
					}catch(NumberFormatException nfe){}
				}
			}
			return new Parser().getTerms(tokens);
		}


	}