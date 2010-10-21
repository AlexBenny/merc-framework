package it.mintlab.desktopjava.tupleengine;

	public class Lexer
	{
		private static final int NOTHING = -1;
		private static final int ATOM = 0;
		private static final int LPAR = 1;
		private static final int RPAR = 2;
		private static final int VAR = 3;
		private static final int NUMBER = 4;
		private static final int COMMA = 5;
		private static final int QUOTE = 6;

		private int state;
		private int indexCh;
		private String text;
		private String token;
		private java.util.ArrayList<IToken> tokens;

		public final java.util.ArrayList<IToken> getTokens(String text)
		{
			if (text == null)
			{
				throw new ParsingException("Invalid text to parsing");
			}
			text += " ";

			this.text = text;
			state = NOTHING;
			token = "";
			tokens = new java.util.ArrayList<IToken>();
			char[] t = text.toCharArray();
			for (indexCh = 0; indexCh < t.length; indexCh++)
			{
				char ch = t[indexCh];
				switch (state)
				{
					case NOTHING:
						nothingReact(ch);
						break;
					case ATOM:
						atomReact(ch);
						break;
					case LPAR:
						lparReact(ch);
						break;
					case RPAR:
						rparReact(ch);
						break;
					case VAR:
						varReact(ch);
						break;
					case NUMBER:
						numberReact(ch);
						break;
					case COMMA:
						commaReact(ch);
						break;
					case QUOTE:
                        quoteReact(ch);
                        break;
				}
			}
			return tokens;
		}


		private void nothingReact(char ch)
		{
			if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'z' || ch == '$')
			{
				state = ATOM;
				indexCh--;
				return;
			}
			if (ch == '\'' || ch == '"')
			{
				state = QUOTE;
				indexCh--;
				return;
			}
			if (ch == '_')
			{
				state = VAR;
				indexCh--;
				return;
			}
			if (ch >= '0' && ch <= '9')
			{
				state = NUMBER;
				indexCh--;
				return;
			}
			if (ch == '(')
			{
				state = LPAR;
				indexCh--;
				return;
			}
			if (ch == ')')
			{
				state = RPAR;
				indexCh--;
				return;
			}
			if (ch == ',')
			{
				state = COMMA;
				indexCh--;
				return;
			}
			if (ch == ' ')
			{
				return;
			}
			throw new ParsingException("Lexer error at '" + text.substring(0, indexCh));
		}


		private void atomReact(char ch)
		{
			if (ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z' || ch >= '0' && ch <= '9' || ch == '-' || ch == '_' || ch == '$')
			{
				token += ch;
				return;
			}
			indexCh--;
			tokens.add(new AtomToken(token));
			token = "";
			state = NOTHING;
		}

		private void varReact(char ch)
		{
			if (ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z' || ch >= '0' && ch <= '9' || ch == '-' || ch == '_')
			{
				token += ch;
				return;
			}
			indexCh--;
			tokens.add(new VarToken(token));
			token = "";
			state = NOTHING;
		}

		private void numberReact(char ch)
		{
			if (ch >= '0' && ch <= '9' || ch == '-' || ch == '.')
			{
				token += ch;
				return;
			}
			int resInt = 0;
			token = token.replace('.', ',');
			try{
				resInt = Integer.parseInt(token);
				tokens.add(new IntToken(resInt));
			}catch(NumberFormatException nfe){
				float resFloat = 0F;
				try{
					resFloat = java.lang.Float.parseFloat(token);
					tokens.add(new FloatToken(resFloat));
				}catch(NumberFormatException nfe2){
					throw new ParsingException("Invalid parsing number " + token);
				}
			}
			indexCh--;
			token = "";
			state = NOTHING;
		}

		private void lparReact(char ch)
		{
			tokens.add(new LparToken());
			token = "";
			state = NOTHING;
		}

		private void rparReact(char ch)
		{
			tokens.add(new RparToken());
			token = "";
			state = NOTHING;
		}

		private void commaReact(char ch)
		{
			tokens.add(new CommaToken());
			token = "";
			state = NOTHING;
		}
		
		private void quoteReact(char ch)
		{
			token += ch;
			if (token.length() > 1)
			{
				if (ch == token.charAt(0))
				{
					tokens.add(new AtomToken(token));
					state = NOTHING;
					return;
				}
			}
			
		}

	}