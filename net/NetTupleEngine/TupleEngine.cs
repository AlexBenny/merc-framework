using System;
using System.Collections.Generic;
using System.Text;

namespace it.mintlab.desktopnet.tupleengine
{
    public static class TupleEngine
    {
        public static Term parse(string tuple)
        {
            return new Parser().getTerms(new Lexer().getTokens(tuple));
        }

        public static Term parse(string tuple, object[] parameters)
        {
            Dictionary<string, AtomToken> parTokens = new Dictionary<string,AtomToken>();
            List<IToken> tokens = new Lexer().getTokens(tuple);
            foreach (IToken token in tokens)
            {
                int outvalue;
                string tokenName = token.ToString();
                if (token is AtomToken && tokenName.StartsWith("$") && int.TryParse(tokenName.Substring(1), out outvalue))
                {
                    ((AtomToken)token).setParameter(parameters[outvalue]);
                }
            }
            return new Parser().getTerms(tokens);
        }


    }
}
