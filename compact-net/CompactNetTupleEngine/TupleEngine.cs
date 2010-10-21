using System;
using System.Collections.Generic;
using System.Text;

namespace it.mintlab.mobilenet.tupleengine
{
    public static class TupleEngine
    {
        public static Term parse(string tuple)
        {
            return new Parser().getTerms(new Lexer().getTokens(tuple));
        }

        public static Term parse(string tuple, object[] parameters)
        {
            List<IToken> tokens = new Lexer().getTokens(tuple);
            foreach (IToken token in tokens)
            {
                string tokenName = token.ToString();
                if (token is AtomToken && tokenName.StartsWith("$") && tokenName.Length >= 2)
                {
                    try
                    {
                        int i = int.Parse(tokenName.Substring(1));
                        ((AtomToken)token).setParameter(parameters[i]);
                    }
                    catch (Exception ex)
                    {}
                }
            }
            return new Parser().getTerms(tokens);
        }



    }
}
