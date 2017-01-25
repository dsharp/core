﻿using System.Collections.Generic;
using System;
using System.Text;

namespace D.Parsing
{
    using Collections;

    using static TokenKind;

    public class Tokenizer : IDisposable
    {
        private readonly SourceReader reader;
        private readonly Env env;

        private Stack<Mode> modes = new Stack<Mode>();

        public Tokenizer(string text)
            : this(new SourceReader(text), new Env()) { }

        public Tokenizer(string text, Env env)
            : this(new SourceReader(text), env) { }

        private Tokenizer(SourceReader reader, Env env)
        {
            this.reader = reader;
            this.env = env;

            modes.Push(Mode.Default);

            ReadTrivia(); // read the trivia

            this.reader.Next(); // start things off
        }

        private Location loc;

        public Token Next()
        {
            start:
            if (reader.IsEof) return new Token(EOF, reader.Location);

            switch (modes.Peek())
            {
                case Mode.Apostrophe:
                    if (reader.Peek() == '\'')
                    {
                        return Read(Character);
                    }

                    modes.Pop();

                    if (reader.Current == '\'')
                    {
                        return Read(Apostrophe); // Closing
                    }

                    break;

                case Mode.Quotes :
                    return ReadQuotedString();
            }
       
            loc = reader.Location;

            switch (reader.Current)
            {
                case '↺': return Read(Repeats);
                case '∎': return Read(End);

                case '~': return Read(Op); // bitwise compliment

                case '|':
                    switch (reader.Peek())
                    {
                        case '|': return Read(Op, 2);            // ||
                        case '>': return Read(PipeForward, 2);   // |>
                        default : return Read(Bar);              // |
                    }

                case '<':
                    switch (reader.Peek())
                    {
                        case '<': return Read(Op, 2); // <<
                        case '=': return Read(Op, 2); // <= 
                    }

                    if (char.IsLetter(reader.Peek())) // tag
                    {
                        EnterMode(Mode.Tag);

                        return Read(TagOpen);
                    }

                    return Read(Op); // <
                    
                case '>':
                    if (InMode(Mode.Tag)) // >
                    {
                        modes.Pop();

                        return Read(TagClose);
                    }

                    break;

                case '=':
                    if (reader.Peek() == '>')           
                    {
                        return Read(LambdaOperator, 2);     // => 
                    }

                    break;

                case '-':
                    switch (reader.Peek())
                    {
                        case '>': return Read(ReturnArrow, 2);  // ->

                        case '0': case '1': case '2': case '3': case '4':
                        case '5': case '6': case '7': case '8': case '9':
                            return ReadNumber(); // -{number}

                        default : return Read(Op); // -
                    }

             

 
                case '[': return Read(BracketOpen);
                case ']': return Read(BracketClose);

                case '{':
                    return Read(BraceOpen);

                case '}':
                    if (InMode(Mode.Expression))
                    {
                        ExitMode();
                    }

                    return Read(BraceClose);

                case ':': // ::, :
                    switch (reader.Peek())
                    {
                        case ':' : return Read(ColonColon, 2); // ::
                        default  : return Read(Colon);         // :
                    }

                case ',': return Read(Comma);
                case '$':
                    switch (reader.Peek())
                    {
                        case '"'    :

                            EnterMode(Mode.InterpolatedString);

                            return Read(InterpolatedStringOpen, 2);
                        default     : return Read(Dollar);
                    }

                case '.': // ., .., ...

                    if (char.IsDigit(reader.Peek())) // .{0-9}
                    {
                        return Read(DecimalPoint);
                    }

                    int dots = 0;

                    while (reader.Current == '.' && dots < 3)
                    {
                        reader.Consume();

                        dots++;

                        if (reader.IsEof) break;
                    }

                    if (dots == 2 && reader.Current == '<') // ..<
                    {
                        reader.Consume();

                        return new Token(HalfOpenRange, loc, "..<", ReadTrivia());
                    }

                    switch (dots)
                    {
                        case 1: return new Token(Dot,        loc, ".",   ReadTrivia());
                        case 2: return new Token(DotDot,     loc, "..",  ReadTrivia());
                        case 3: return new Token(DotDotDot,  loc, "...", ReadTrivia());
                    }

                    break;

                case '\'':
                    EnterMode(Mode.Apostrophe);

                    // Return token directly so we don't consume whitespace...

                    return new Token(Apostrophe, reader.Location, reader.Consume().ToString(), null);

                case '`':
                    return Read(Backtick);

                case '"':
                    if (InMode(Mode.InterpolatedString))
                    {
                        modes.Pop();
                    }
                    else
                    {
                        EnterMode(Mode.Quotes);
                    }

                    return Read(Quote);

                    
                    
                case '(': return Read(ParenthesisOpen);
                case ')': return Read(ParenthesisClose);

                // case '#': return Read(Pound);

                case '_': return Read(Underscore);

                case '?': return Read(Question);

                case ';': return Read(Semicolon);
        
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9': return ReadNumber();

                // Superscript
                case '⁰': 
                case '¹': 
                case '²': 
                case '³': 
                case '⁴': 
                case '⁵': 
                case '⁶': 
                case '⁷': 
                case '⁸': 
                case '⁹': return ReadSuperscript();
               
                case '\n':
                case '\r':
                case '\t':
                case ' ': ReadTrivia(); goto start;
            }

            // Operators 
            if (!char.IsLetter(reader.Current))
            {
                Trie<Operator>.Node node;

                if (env.Operators.Maybe(OperatorType.Infix, reader.Current, out node))
                {
                    var start = reader.Location;

                    sb.Append(reader.Consume());

                    while (node.Contains(reader.Current) && node.TryGetNode(reader.Current, out node) && !reader.IsEof)
                    {
                        sb.Append(reader.Consume());
                    }

                    return new Token(Op, start, sb.Extract(), ReadTrivia());
                }
            }

            return ReadIdentifierOrKeyword();
        }

        // Operator
        private Token Read(TokenKind kind, int count = 1)
            => new Token(kind, loc, reader.Consume(count), ReadTrivia());


        public Token ReadIdentifierOrKeyword()
        {
            var start = reader.Location;

            do
            {
                sb.Append(reader.Current);
            }
            while (char.IsLetterOrDigit(reader.Next()) && !reader.IsEof);

            // or underscore...

            var text = sb.Extract();

            TokenKind kind;

            if (keywords.TryGetValue(text, out kind))
            {
                return new Token(kind, start, text);
            }

            return new Token(Identifier, start, text, ReadTrivia());
        }

        public static readonly Dictionary<string, TokenKind> keywords = new Dictionary<string, TokenKind> {
            { "ƒ"                , Function },
            { "as"               , Op },
            { "ascending"        , Ascending },
            { "async"            , Async },
            { "false"            , False },
            { "null"             , Null },
            { "catch"            , Catch },
            { "continue"         , Continue },
            { "default"          , Default },
            { "do"               , Do },
            { "else"             , Else },
            { "emit"             , Emit },
            { "enum"             , Enum },
            { "for"              , For },
            { "from"             , From },
            { "function"         , Function },
            { "let"              , Let },
            { "match"            , Match },
            { "if"               , If },
            { "impl"             , Implementation },
            { "implementation"   , Implementation },
            { "in"               , In },
            { "is"               , Op },
            { "descending"       , Descending },
            { "mutable"          , Mutable },
            { "mutating"         , Mutating },
            { "on"               , On },
            { "observe"          , Observe },
            { "orderby"          , Orderby },
            { "primitive"        , Primitive },
            { "return"           , Return },
            { "select"           , Select },
            { "this"             , This },
            { "throw"            , Throw },
            { "to"               , To },
            { "type"             , Type },
            { "true"             , True },
            { "try"              , Try },
            { "until"            , Until },
            { "unit"             , Unit },
            { "using"            , Using },
            { "var"              , Var },
            { "when"             , When },
            { "while"            , While },
            { "with"             , With },
            { "where"            , Where },
            { "yield"            , Yield },             
            { "event"            , Event },
            { "protocal"         , Protocal },
            { "record"           , Record }
        };

        public Token ReadQuotedString()
        {
            if (reader.Current == '"')
            {
                ExitMode();

                return Read(Quote);
            }

            var start = reader.Location;

            while (!reader.IsEof && reader.Current != '"')
            {
                sb.Append(reader.Consume());
            }

            return new Token(String, start, sb.Extract());
        }

        public Token ReadNumber()
        {
            var start = reader.Location;

            if (reader.Current == '-')
            {
                sb.Append(reader.Consume());
            }

            while (!reader.IsEof && char.IsDigit(reader.Current))
            {
                sb.Append(reader.Consume());

                if (reader.Current == 'e')
                {
                    sb.Append(reader.Consume());

                    if (reader.Current == '-')
                    {
                        sb.Append(reader.Consume());
                    }
                }
            }

            // if decimal, we'll yield two more tokens (consumed by parser)
            // yield .
            // yield number
            // this simplifies building integer & rational numbers

            return new Token(Number, start, sb.Extract(), ReadTrivia());
        }

        public Token ReadSuperscript()
        {
            var start = reader.Location;

            do
            {
                sb.Append(reader.Current);
            }
            while (!reader.IsEof && IsSuperscript(reader.Next()));

            return new Token(Superscript, start, sb.Extract(), ReadTrivia());
        }

        // TODO: c# 7, use local function
        private bool IsSuperscript(char c)
            => (c == '⁰' || c == '¹' || c == '²'|| c == '³'|| c == '⁴'
             || c == '⁵'|| c == '⁶'|| c == '⁷'|| c == '⁸' || c == '⁹');               

        public void Dispose()
        {
            reader.Dispose();
        }

        #region Whitespace

        private readonly StringBuilder sb = new StringBuilder();

        private string ReadTrivia()
        {
            while (char.IsWhiteSpace(reader.Current) && !reader.IsEof)
            {
                sb.Append(reader.Current);

                reader.Next();
            }

            if (reader.Current == '/' && reader.Peek() == '/')
            {
                // TODO: Append to trivia

                ReadComment();
            }

            if (sb.Length == 0) return null;

            return sb.Extract();
        }

        public void ReadComment()
        {
            sb.Append(reader.Consume()); // /
            sb.Append(reader.Consume()); // /

            var l = reader.Line;

            while (l == reader.Line && !reader.IsEof)
            {
                sb.Append(reader.Current);

                reader.Next();
            }
        }

        #endregion

        #region Modes

        public void EnterMode(Mode mode)
          => modes.Push(mode);

        public void ExitMode()
            => modes.Pop();

        public bool InMode(Mode mode)
            => modes.Peek() == mode;

        public enum Mode
        {
            Default,
            Apostrophe,
            Quotes,
            InterpolatedString,
            Expression,
            Tag, // <tag
        }

        #endregion
    }
}