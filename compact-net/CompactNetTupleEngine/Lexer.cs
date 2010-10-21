using System;
using System.Collections.Generic;
using System.Text;

namespace it.mintlab.mobilenet.tupleengine
{
    public class Lexer
    {
        const int NOTHING = -1;
        const int ATOM = 0;
        const int LPAR = 1;
        const int RPAR = 2;
        const int VAR = 3;
        const int NUMBER = 4;
        const int COMMA = 5;
        const int QUOTE = 6;

        private int state;
        private int indexCh;
        private string text;
        private string token;
        private List<IToken> tokens;

        public List<IToken> getTokens(string text)
        {
            if (text == null) throw new ParsingException("Invalid text to parsing");
            text += " ";
            this.text = text;
            state = NOTHING;
            token = "";
            tokens = new List<IToken>();
            char[] t = text.ToCharArray();
            for (indexCh = 0; indexCh < t.Length; indexCh++)
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
            if (ch == ' ') return;
            throw new ParsingException("Lexer error at '" + text.Substring(0, indexCh));
        }


        private void atomReact(char ch)
        {
            if (ch >= 'a' && ch <= 'z' ||
                ch >= 'A' && ch <= 'Z' ||
                ch >= '0' && ch <= '9' ||
                ch == '-' || ch == '_' ||
                ch == '$')
            {
                token += ch;
                return;
            }
            indexCh--;
            tokens.Add(new AtomToken(token));
            token = "";
            state = NOTHING;
        }

        private void varReact(char ch)
        {
            if (ch >= 'a' && ch <= 'z' ||
                ch >= 'A' && ch <= 'Z' ||
                ch >= '0' && ch <= '9' ||
                ch == '-' || ch == '_')
            {
                token += ch;
                return;
            }
            indexCh--;
            tokens.Add(new VarToken(token));
            token = "";
            state = NOTHING;
        }

        private void numberReact(char ch)
        {
            if (ch >= '0' && ch <= '9' ||
                ch == '-' || ch == '.')
            {
                token += ch;
                return;
            }
            token = token.Replace('.', ',');
            int resInt;
            try {
                resInt = int.Parse(token);
                tokens.Add(new IntToken(resInt));
            } catch (Exception ex1) {
                float resFloat;
                try {
                    resFloat = float.Parse(token);
                    tokens.Add(new FloatToken(resFloat));
                } catch (Exception ex2) {
                    throw new ParsingException("Invalid parsing number " + token);
                }
            }
            indexCh--;
            token = "";
            state = NOTHING;
        }

        private void lparReact(char ch)
        {
            tokens.Add(new LparToken());
            token = "";
            state = NOTHING;
        }

        private void rparReact(char ch)
        {
            tokens.Add(new RparToken());
            token = "";
            state = NOTHING;
        }

        private void commaReact(char ch)
        {
            tokens.Add(new CommaToken());
            token = "";
            state = NOTHING;
        }

        private void quoteReact(char ch)
        {
            token += ch;
            if (token.Length > 1)
            {
                if (ch == token[0])
                {
                    tokens.Add(new AtomToken(token));
                    state = NOTHING;
                    return;
                }
            }
        }


    }
}
