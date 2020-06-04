using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Kata
{
    public class Interpreter
    {
#pragma warning disable IDE1006 // Naming Styles
        public double? input(string input)
#pragma warning restore IDE1006 // Naming Styles
        {
            var tokens = Tokenize(input);
        }

        private static IEnumerable<string> Tokenize(string input)
        {
            input += ")";
            var tokens = new List<string>();
            var rgxMain = new Regex("=>|[-+*/%=\\(\\)]|[A-Za-z_][A-Za-z0-9_]*|[0-9]*(\\.?[0-9]+)");
            var matches = rgxMain.Matches(input);

            foreach (Match m in matches)
                tokens.Add(m.Groups[0].Value);

            return tokens;
        }
    }

    enum TokenType
    {
        Unknown,
        Number,
        Identifier,
        Operator,
        FnDef,
        FnBodyDef,
        FnCall,
        Assign,
        LParen,
        RParen,
        EOF
    }

    static class TokenExtensions
    {
        public static TokenType GetTokenType(this string token)
        {
            if (double.TryParse(token, out var val))
                return TokenType.Number;

            if (Regex.Match(token, "[A-Za-z_][A-Za-z0-9_]*").Success)
                return TokenType.Identifier;

            if (Regex.Match(token, "[-+*/%=\\(\\)]").Success)
                return TokenType.Operator;

            return TokenType.Unknown;
        }
    }

    class Parser
    {
        private readonly IEnumerator<string> _tokens;

        public Parser(IEnumerable<string> tokens)
        {
            _tokens = tokens.GetEnumerator();
            _tokens.MoveNext();
        }

        private void EatToken(TokenType ttype)
        {
            if (_tokens.Current.GetTokenType() == ttype)
                _tokens.MoveNext();
            else
                throw new Exception("Invalid syntax error - unexpected token type");
        }

        public AST Parse()
        {
            if (_tokens.Current.GetTokenType() == TokenType.FnDef)
                return ParseFnDef();
            return ParseExpr();
        }

        private AST ParseFnDef()
        {
            EatToken(TokenType.FnDef);
            var fName = _tokens.Current;
            EatToken(TokenType.Identifier);
            var formalParams = new List<string>();

            while (_tokens.Current.GetTokenType() == TokenType.Identifier)
            {
                formalParams.Add(_tokens.Current);
                EatToken(TokenType.Identifier);
            }

            EatToken(TokenType.FnBodyDef);

            var body = Parse() as ExprAST;
            return new FnDefAST(fName, formalParams, body);
        }

        private AST ParseExpr()
        {
            throw new NotImplementedException();
        }
    }

    class AST
    {

    }

    class FnDefAST : AST
    {
        public FnDefAST(string fName, List<string> formalParams, ExprAST body)
        {
            FName = fName;
            FormalParams = formalParams;
            Body = body;
        }

        public ExprAST Body { get; }

        public List<string> FormalParams { get; }

        public string FName { get; }
    }

    class ExprAST : AST
    {

    }
}