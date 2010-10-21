package it.mintlab.desktopjava.tupleengine;

import java.util.ArrayList;

	public class Parser
	{

		private java.util.HashMap<String, Var> varMap;


		public final Term getTerms(java.util.ArrayList<IToken> tokens)
		{
			if (tokens == null)
			{
				throw new ParsingException("Invalid tokens to parsing");
			}
			varMap = new java.util.HashMap<String, Var>();
			IToken t = tokens.get(0);
			tokens.remove(0);
			if (t instanceof AtomToken)
			{
				return parseAtom(t,tokens);
			}
			if (t instanceof IntToken)
			{
				return new Int(((IntToken)t).getValue());
			}
			if (t instanceof FloatToken)
			{
				return new Float(((FloatToken)t).getValue());
			}
			if (t instanceof VarToken)
			{
				String key = t.toString();
				if (varMap.containsKey(key))
				{
					return varMap.get(key);
				}
				Var v = new Var(key);
				varMap.put(key, v);
				return v;
			}
			throw new ParsingException("Invalid start token");
		}
		
		private Term parseAtom(IToken t, ArrayList<IToken> tokens)
        {
            if (tokens.size() == 0)
            {
                Atom atom = new Atom(t.toString());
                if (((AtomToken)t).isWithParameter())
                {
                    atom.setRefObject(((AtomToken)t).getParameter());
                }
                return atom;
            }
            IToken t1 = tokens.get(0);
            if (t1 instanceof CommaToken || t1 instanceof RparToken)
            {
                Atom atom = new Atom(t.toString());
                if (((AtomToken)t).isWithParameter())
                {
                    atom.setRefObject(((AtomToken)t).getParameter());
                }
                return atom;
            }
            tokens.remove(0);
            if (t1 instanceof LparToken)
            {
            	ArrayList<Term> terms = new ArrayList<Term>();
                terms.add(getTerms(tokens));
                IToken t2 = tokens.get(0);
                tokens.remove(0);
                while (t2 instanceof CommaToken)
                {
                    terms.add(getTerms(tokens));
                    t2 = tokens.get(0);
                    tokens.remove(0);
                }
                if (t2 instanceof RparToken)
                {
                    Compound compound = new Compound(t.toString(), terms);
                    if (((AtomToken)t).isWithParameter())
                    {
                        compound.setRefObject(((AtomToken)t).getParameter());
                    }
                    return compound;
                }
                throw new ParsingException("Error during parsing " + t2.toString());
            }
            throw new ParsingException("Error during parsing " + t1.toString());
        }

	}