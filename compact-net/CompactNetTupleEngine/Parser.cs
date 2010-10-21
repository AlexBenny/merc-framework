using System;
using System.Collections.Generic;
using System.Text;

namespace it.mintlab.mobilenet.tupleengine
{
    public class Parser
    {
        private Dictionary<string, Var> varMap;

        public Term getTerms(List<IToken> tokens)
        {
            if (tokens == null) throw new ParsingException("Invalid tokens to parsing");
            varMap = new Dictionary<string, Var>();
            IToken t = tokens[0];
            tokens.RemoveAt(0);
            if (t is AtomToken)
            {
                if (tokens.Count == 0)
                {
                    Atom atom = new Atom(t.ToString());
                    if (((AtomToken)t).isWithParameter())
                    {
                        atom.setRefObject(((AtomToken)t).getParameter());
                    }
                    return atom;
                }
                IToken t1 = tokens[0];
                if (t1 is CommaToken || t1 is RparToken)
                {
                    Atom atom = new Atom(t.ToString());
                    if (((AtomToken)t).isWithParameter())
                    {
                        atom.setRefObject(((AtomToken)t).getParameter());
                    }
                    return atom;
                }
                tokens.RemoveAt(0);
                if (t1 is LparToken)
                {
                    List<Term> terms = new List<Term>();
                    terms.Add(getTerms(tokens));
                    IToken t2 = tokens[0];
                    tokens.RemoveAt(0);
                    while (t2 is CommaToken)
                    {
                        terms.Add(getTerms(tokens));
                        t2 = tokens[0];
                        tokens.RemoveAt(0);
                    }
                    if (t2 is RparToken)
                    {
                        Compound compound = new Compound(t.ToString(), terms);
                        if (((AtomToken)t).isWithParameter())
                        {
                            compound.setRefObject(((AtomToken)t).getParameter());
                        }
                        return compound;
                    }
                    throw new ParsingException("Error during parsing " + t2.ToString());
                }
                throw new ParsingException("Error during parsing "+t1.ToString());
            }
            if (t is IntToken)
            {
                return new Int(((IntToken) t).getValue());
            }
            if (t is FloatToken)
            {
                return new Float(((FloatToken) t).getValue());
            }
            if (t is VarToken)
            {
                string key = t.ToString();
                if (varMap.ContainsKey(key))
                {
                    return varMap[key];
                }
                Var v = new Var(key);
                varMap.Add(key, v);
                return v;
            }
            throw new ParsingException("Invalid start token");
        }

    }
}
