namespace Gurosi;

public sealed class Tokenizer {
    private SourceReader _reader;

    public Tokenizer(string source)
    {
        Guard.Null(source);
        source = source.ReplaceLineEndings("\n");
        _reader = new SourceReader(source);
    }

    public Lexical Tokenize()
    {
        List<Token> tokens = new List<Token>();
        int line = 1;
        int lineStart = 0;

        void addToken(string value, TokenType type)
        {
            tokens.Add(new Token(value, type, new TextPoint(line, _reader.Position - lineStart - value.Length)));
        }

        void procDouble(char c, TokenType single, TokenType dble)
        {
            if (_reader.MatchNext(c))
            {
                _reader.Read();
                addToken(new string(c, 2), dble);
            }
            else
            {
                addToken(c.ToString(), single);
            }
        }

        void procCont(char left, char right, TokenType type1, TokenType type2)
        {
            if (_reader.MatchNext(right))
            {
                _reader.Read();
                addToken(left.ToString() + right, type2);
            }
            else
            {
                addToken(left.ToString(), type1);
            }
        }

        while (!_reader.EOF)
        {
            char c = _reader.Read();

            if (c == '\n')
            {
                lineStart = _reader.Position;
                line++;
                continue;
            }

            if (c == '#')
            {
                _reader.TakeWhile(x => x != '\n');
                continue;
            }

            if (c == '+')
            {
                if (_reader.MatchNext('='))
                {
                    _reader.Read();
                    addToken("+=", TokenType.AddAssign);
                }
                else
                {
                    procDouble('+', TokenType.Plus, TokenType.Increment);
                }
                
                continue;
            }
            
            if (c == '-')
            {
                if (_reader.MatchNext('='))
                {
                    _reader.Read();
                    addToken("-=", TokenType.SubAssign);
                }
                else
                {
                    procDouble('-', TokenType.Minus, TokenType.Decrement);
                }
                
                continue;
            }

            if (c == '*')
            {
                if (_reader.MatchNext('='))
                {
                    _reader.Read();
                    addToken("*=", TokenType.MultAssign);
                }
                else
                {
                    procDouble('*', TokenType.Asterisk, TokenType.Power);
                }
                
                continue;
            }

            if (c == '=')
            {
                procDouble('=', TokenType.Assign, TokenType.Equal);
                continue;
            }

            if (c == '/')
            {
                if (_reader.MatchNext('='))
                {
                    _reader.Read();
                    addToken("/=", TokenType.DivAssign);
                }
                else
                {
                    addToken("/", TokenType.Slash);
                }
                
                continue;
            }

            if (c == '%')
            {
                if (_reader.MatchNext('='))
                {
                    _reader.Read();
                    addToken("%=", TokenType.ModAssign);
                }
                else
                {
                    addToken("/", TokenType.Percent);
                }

                continue;
            }

            if (c == '&')
            {
                procDouble('&', TokenType.And, TokenType.LogAnd);
                continue;
            }

            if (c == '|')
            {
                procDouble('|', TokenType.Pipe, TokenType.LogOr);
                continue;
            }

            if (c == '(')
            {
                addToken("(", TokenType.OpenParen);
                continue;
            }

            if (c == ')')
            {
                addToken(")", TokenType.CloseParen);
                continue;
            }

            if (c == '{')
            {
                addToken("{", TokenType.OpenBrace);
                continue;
            }

            if (c == '}')
            {
                addToken("}", TokenType.CloseBrace);
                continue;
            }

            if (c == '[')
            {
                addToken("[", TokenType.OpenBracket);
                continue;
            }

            if (c == ']')
            {
                addToken("]", TokenType.CloseBracket);
                continue;
            }

            if (c == '@')
            {
                addToken("@", TokenType.AtSign);
                continue;
            }

            if (c == ';')
            {
                addToken(";", TokenType.Semicolon);
                continue;
            }

            if (c == ':')
            {
                addToken(":", TokenType.Colon);
                continue;
            }

            if (c == '\\')
            {
                addToken("\\", TokenType.BackSlash);
                continue;
            }

            if (c == '_')
            {
                if (_reader.MatchNext(x => !TokenData.IsIdent(x)))
                {
                    addToken("_", TokenType.Underscore);
                    continue;
                }
            }

            if (c == '$')
            {
                addToken("$", TokenType.Dollar);
                continue;
            }

            if (c == ',')
            {
                addToken(",", TokenType.Comma);
                continue;
            }

            if (c == '?')
            {
                addToken("?", TokenType.Question);
                continue;
            }

            if (c == '.')
            {
                addToken(".", TokenType.DotOperator);
                continue;
            }

            if (c == '>')
            {
                procCont('>', '=', TokenType.GreaterThan, TokenType.GreaterThanEqual);
                continue;
            }

            if (c == '<')
            {
                if (_reader.MatchNext('-'))
                {
                    _reader.Read();
                    addToken("<-", TokenType.Inherit);
                }
                else
                    procCont('<', '=', TokenType.LessThan, TokenType.LessThanEqual);
                continue;
            }

            if (c == '!')
            {
                procCont('!', '=', TokenType.Excl, TokenType.NotEqual);
                continue;
            }

            if (TokenData.IsNumber(c))
            {
                _reader.Retr();
                string digits = _reader.TakeWhile(c => TokenData.IsNumber(c));
                // contains decimals
                if (_reader.MatchNext('.'))
                {
                    _reader.Read();
                    string dec = _reader.TakeWhile(x => TokenData.IsNumber(x));
                    if (_reader.MatchNext('d') || !float.TryParse(digits + "." + dec, out _))
                    {
                        if (_reader.MatchNext('d'))
                            _reader.Read();
                        addToken(digits + "." + dec, TokenType.DoubleLiteral);
                    }
                    else
                    {
                        if (_reader.MatchNext('f') || _reader.MatchNext('F'))
                            _reader.Read();

                        addToken(digits + "." + dec, TokenType.FloatLiteral);
                    }
                }
                else
                {
                    if (_reader.MatchNext('f'))
                    {
                        // as float.
                        _reader.Read();
                        addToken(digits, TokenType.FloatLiteral);
                    }
                    else if (_reader.MatchNext('d'))
                    {
                        _reader.Read();
                        addToken(digits, TokenType.DoubleLiteral);
                    }
                    else
                    {
                        addToken(digits, TokenType.IntLiteral);
                    }
                }

                continue;
            }

            if (c == '\"')
            {
                string content = _reader.TakeWhile(x => x != '\"');
                _reader.Read();

                addToken(content, TokenType.StringLiteral);
                continue;
            }

            if (TokenData.IsIdent(c))
            {
                _reader.Retr();
                string word = _reader.TakeWhile(x => TokenData.IsIdent(x));
                if (TokenData.IsKeyword(word))
                {
                    // keyword
                    addToken(word, TokenData.GetKeywordTokenType(word));
                }
                else
                {
                    // identifier
                    addToken(word, TokenType.Ident);
                }

                continue;
            }
        }

        return new Lexical(tokens);
    }
}