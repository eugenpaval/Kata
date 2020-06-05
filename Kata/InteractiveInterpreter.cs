using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace InteractiveInterpreter
{
    public class Interpreter
    {
#pragma warning disable IDE1006 // Naming Styles
        public double? input(string input)
#pragma warning restore IDE1006 // Naming Styles
        {
            var tokens = Tokenize(input);
            var ctx = new Context();
            var parser = new Parser(tokens, ctx);
            var ast = parser.Parse();

            return new Evaluator(ctx).Evaluate(ast);
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
        Add,
        Sub,
        Mul,
        Div,
        Mod,
        FnBodyDef,
        FnCall,
        Assign,
        LParen,
        RParen,
        EOF
    }

    static class TokenExtensions
    {
        private static readonly Dictionary<string, TokenType> _opTokenToType = new Dictionary<string, TokenType>
        {
            ["+"] = TokenType.Add,
            ["-"] = TokenType.Sub,
            ["*"] = TokenType.Mul,
            ["/"] = TokenType.Div,
            ["%"] = TokenType.Mod,
            ["="] = TokenType.Assign,
            ["("] = TokenType.LParen,
            [")"] = TokenType.RParen,
            ["=>"] = TokenType.FnBodyDef
        };

        public static TokenType GetTokenType(this string token)
        {
            if (double.TryParse(token, out var val))
                return TokenType.Number;

            if (Regex.Match(token, "[A-Za-z_][A-Za-z0-9_]*").Success)
                return TokenType.Identifier;

            if (Regex.Match(token, "[-+*/%=\\(\\)]").Success)
                return _opTokenToType[token];

            return TokenType.Unknown;
        }
    }

    class Parser
    {
        private readonly IEnumerator<string> _tokens;
        private readonly Context _context;

        public Parser(IEnumerable<string> tokens, Context context)
        {
            _context = context;
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
            if (_tokens.Current == null)
                return null;

            if (_tokens.Current.ToLower() == "fn")
            {
                EatToken(TokenType.Identifier);
                return ParseFnDef();
            }

            return ParseExpr();
        }

        private AST ParseFnDef()
        {
            var fName = _tokens.Current;
            EatToken(TokenType.Identifier);

            var formalParams = new List<string>();

            while (_tokens.Current.GetTokenType() == TokenType.Identifier)
            {
                formalParams.Add(_tokens.Current);
                EatToken(TokenType.Identifier);
            }

            EatToken(TokenType.FnBodyDef);

            var body = ParseExpr() as BinaryOpAST;
            var fnDef = new FnDefAST(fName, formalParams, body);
            
            _context.AddFnDef(fName, fnDef);
            return fnDef;
        }

        private AST ParseExpr()
        {
            // expr ::= term | (+\-) factor
            var term = ParseTerm();
            var tokenType = _tokens.Current.GetTokenType();

            while (tokenType == TokenType.Add || tokenType == TokenType.Sub)
            {
                var token = _tokens.Current;
                EatToken(tokenType);

                var otherTerm = ParseTerm();
                term = new BinaryOpAST(term, token, otherTerm);
                
                tokenType = _tokens.Current.GetTokenType();
            }

            return term;
        }

        private AST ParseTerm()
        {
            // term ::= factor | (*\/|%) factor
            var factor = ParseFactor();
            var tokenType = _tokens.Current.GetTokenType();

            while (tokenType == TokenType.Mul || tokenType == TokenType.Div || tokenType == TokenType.Mod)
            {
                var token = _tokens.Current;
                EatToken(tokenType);

                var otherFactor = ParseFactor();
                factor = new BinaryOpAST(factor, token, otherFactor);
                tokenType = _tokens.Current.GetTokenType();
            }

            return factor;
        }

        private AST ParseFactor()
        {
            // factor ::= number | identifier | assignment | '('expr')' | fn-call
            var tokenType = _tokens.Current.GetTokenType();
            AST node = null;

            if (tokenType == TokenType.Identifier && _context.IsFnDef(_tokens.Current))
                tokenType = TokenType.FnCall;

            switch (tokenType)
            {
                case TokenType.Number:
                    node = new NumberAST(_tokens.Current);
                    EatToken(TokenType.Number);
                    break;

                case TokenType.Identifier:
                    node = new IdentifierAST(_tokens.Current);
                    EatToken(TokenType.Identifier);

                    if (_context.IsFnDef(_tokens.Current))
                        throw new Exception($"Identifier {_tokens.Current} is already defined as a function");

                    tokenType = _tokens.Current.GetTokenType();
                    while (tokenType == TokenType.Assign)
                    {
                        EatToken(TokenType.Assign);
                        var expr = ParseExpr();
                        node = new BinaryOpAST(node, "=", expr);
                        tokenType = _tokens.Current.GetTokenType();
                    }
                    break;

                case TokenType.FnCall:
                    var fnDef = _context.GetFnDef(_tokens.Current);
                    EatToken(TokenType.FnCall);
                    var parameters = fnDef.FormalParams.Select(p => ParseExpr());

                    node = new FnCallAST(fnDef.FName, parameters);
                    break;

                case TokenType.LParen:
                    EatToken(TokenType.LParen);
                    node = ParseExpr();
                    EatToken(TokenType.RParen);
                    break;
            }

            return node;
        }
    }

    abstract class AST
    {
        private Evaluator _visitor;

        public string Operator { get; protected set; }
        public List<AST> Children { get; } = new List<AST>();

        public string Value { get; set; }

        public void AcceptVisitor(Evaluator visitor)
        {
            _visitor = visitor;
        }
    }

    class FnDefAST : AST
    {
        public FnDefAST(string fName, List<string> formalParams, BinaryOpAST body)
        {
            FName = fName;
            FormalParams = formalParams;
            Body = body;
        }

        public BinaryOpAST Body { get; }

        public List<string> FormalParams { get; }

        public string FName { get; }
    }

    class FnCallAST : AST
    {
        public FnCallAST(string fName, IEnumerable<AST> parameters)
        {
            Operator = fName;
            Children.AddRange(parameters);
        }
    }

    class BinaryOpAST : AST
    {
        public BinaryOpAST(AST left, string op, AST right)
        {
            Operator = op;
            Children.Add(left);
            Children.Add(right);
        }
    }

    class NumberAST : AST
    {
        public NumberAST(string value)
        {
            Value = value;
        }
    }

    class IdentifierAST : AST
    {
        public IdentifierAST(string value)
        {
            Value = value;
        }
    }

    class Context
    {
        private readonly Dictionary<string, FnDefAST> _fnDefs = new Dictionary<string, FnDefAST>();
        private readonly Dictionary<string, double> _vars = new Dictionary<string, double>();

        public void AddFnDef(string fName, FnDefAST fnDef)
        {
            if (_vars.ContainsKey(fName))
                throw new Exception($"Function {fName} is already defined as a variable");

            _fnDefs[fName] = fnDef;
        }

        public void AddVar(string id, double val)
        {
            if (_fnDefs.ContainsKey(id))
                throw new Exception($"Variable {id} already defined as a function");

            _vars[id] = val;
        }

        public bool IsFnDef(string id)
        {
            return _fnDefs.ContainsKey(id);
        }

        public FnDefAST GetFnDef(string fName)
        {
            if (_fnDefs.TryGetValue(fName, out var fnDef))
                return fnDef;

            throw new Exception($"Function {fName} is not defined");
        }

        public double GetVar(string id)
        {
            if (_vars.TryGetValue(id, out var val))
                return val;

            throw new Exception($"Variable {id} is not assigned");
        }
    }

     class Evaluator
     {
         private readonly Context _context;

         public Evaluator(Context context)
         {
             _context = context;
         }

         public double Evaluate(AST tree)
         {
             return 0;
         }

         public double Evaluate(NumberAST ast)
         {
             return double.Parse(ast.Value);
         }

         public double Evaluate(IdentifierAST ast)
         {
             return _context.GetVar(ast.Value);
         }

         public double Evaluate(BinaryOpAST ast)
         {
             if (ast.Operator == "=")
             {
                 var retVal = Evaluate(ast.Children[1]);
                 _context.AddVar(ast.Children[0].Value, retVal);

                 return retVal;
             }

             return ast.Operator switch
             {
                 "+" => Evaluate(ast.Children[0]) + Evaluate(ast.Children[1]),
                 "-" => Evaluate(ast.Children[0]) - Evaluate(ast.Children[1]),
                 "*" => Evaluate(ast.Children[0]) * Evaluate(ast.Children[1]),
                 "/" => Evaluate(ast.Children[0]) / Evaluate(ast.Children[1]),
                 "%" => Evaluate(ast.Children[0]) % Evaluate(ast.Children[1]),
                 _ => throw new Exception($"Evaluation internal exception: unknown operator {ast.Operator}")
             };
         }

         public double Evaluate(FnCallAST ast)
         {
             var fnDef = _context.GetFnDef(ast.Value);
             var ctxClone = _context.Clone();
         }
     }
}