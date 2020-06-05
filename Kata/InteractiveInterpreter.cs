using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace InteractiveInterpreter
{
    public class Interpreter
    {
        private static readonly Parser _parser = new Parser();

        public double? input(string input)
        {
            var tokens = Tokenize(input);
            if (!tokens.Any())
                return null;

            var ast = _parser.Parse(tokens);

            try
            {
                return new EvaluationContext().Evaluate(ast, _parser.Context);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Expression does not conform to given grammar")
                    return null;

                throw;
            } 
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
        Assign,
        LParen,
        RParen,
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
        private IEnumerator<string> _tokens;

        public Parser()
        {
            Context = new ParserContext();
        }

        public ParserContext Context { get; }

        public AST Parse(IEnumerable<string> tokens)
        {
            _tokens = tokens.GetEnumerator();
            _tokens.MoveNext();

            if (_tokens.Current == null)
                return null;

            if (_tokens.Current.ToLower() == "fn")
            {
                AcceptToken(TokenType.Identifier);
                return ParseFnDef();
            }
            return ParseExpr();
        }

        private void AcceptToken(TokenType ttype)
        {
            if (_tokens.Current.GetTokenType() == ttype)
                _tokens.MoveNext();
            else
                throw new Exception("Invalid syntax error - unexpected token type");
        }

        private AST ParseFnDef()
        {
            var fName = _tokens.Current;
            AcceptToken(TokenType.Identifier);

            var formalParams = new List<string>();

            while (_tokens.Current.GetTokenType() == TokenType.Identifier)
            {
                formalParams.Add(_tokens.Current);
                AcceptToken(TokenType.Identifier);
            }

            AcceptToken(TokenType.FnBodyDef);

            var body = ParseExpr(formalParams) as BinaryOpAST;
            var fnDef = new FnDefAST(fName, formalParams, body);
            
            Context.AddDef(fName, fnDef);
            return fnDef;
        }

        private AST ParseExpr(List<string> ns = null)
        {
            // expr ::= term | (+\-) factor
            var term = ParseTerm(ns);
            var tokenType = _tokens.Current.GetTokenType();

            while (tokenType == TokenType.Add || tokenType == TokenType.Sub)
            {
                var token = _tokens.Current;
                AcceptToken(tokenType);

                var otherTerm = ParseTerm(ns);
                term = new BinaryOpAST(term, token, otherTerm);
                tokenType = _tokens.Current.GetTokenType();
            }

            return term;
        }

        private AST ParseTerm(List<string> ns)
        {
            // term ::= factor | (*\/|%) factor
            var factor = ParseFactor(ns);
            var tokenType = _tokens.Current.GetTokenType();

            while (tokenType == TokenType.Mul || tokenType == TokenType.Div || tokenType == TokenType.Mod)
            {
                var token = _tokens.Current;
                AcceptToken(tokenType);

                var otherFactor = ParseFactor(ns);
                factor = new BinaryOpAST(factor, token, otherFactor);
                tokenType = _tokens.Current.GetTokenType();
            }

            return factor;
        }

        private AST ParseFactor(List<string> ns)
        {
            // factor ::= number | identifier | assignment | '('expr')' | fn-call
            var tokenType = _tokens.Current.GetTokenType();
            AST node = null;

            switch (tokenType)
            {
                case TokenType.Number:
                    node = new NumberAST(_tokens.Current);
                    AcceptToken(TokenType.Number);
                    break;

                case TokenType.Identifier:
                    if (Context.IsFnDef(_tokens.Current))
                    {
                        var fnDef = Context.GetDef<FnDefAST>(_tokens.Current);
                        AcceptToken(TokenType.Identifier);
                        
                        var parameters = fnDef.FormalParams.Select(p => ParseExpr());
                        node = new FnCallAST(fnDef.FName, parameters);
                    }
                    else
                    {
                        if (ns != null && !ns.Contains(_tokens.Current))
                            throw new Exception($"Parameter {_tokens.Current} is not defined");

                        node = new IdentifierAST(_tokens.Current);
                        AcceptToken(TokenType.Identifier);
                    }

                    tokenType = _tokens.Current.GetTokenType();
                    while (tokenType == TokenType.Assign)
                    {
                        AcceptToken(TokenType.Assign);
                        var expr = ParseExpr();
                        node = new BinaryOpAST(node, "=", expr);
                        tokenType = _tokens.Current.GetTokenType();
                    }
                    break;

                case TokenType.LParen:
                    AcceptToken(TokenType.LParen);
                    node = ParseExpr();
                    AcceptToken(TokenType.RParen);
                    break;
            }

            return node;
        }
    }

    abstract class AST
    {
        public string Operator { get; protected set; }
        public List<AST> Children { get; } = new List<AST>();

        public string Value { get; set; }
    }

    class FnDefAST : AST
    {
        public FnDefAST(string fName, List<string> formalParams, BinaryOpAST body)
        {
            FName = Value = fName;
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
            Operator = Value = fName;
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

    class Context<TOut>
    {
        private readonly Dictionary<string, TOut> _defs = new Dictionary<string, TOut>();

        public void AddValue(string name, TOut def)
        {
            _defs[name] = def;
        }

        public TOut GetValue(string name)
        {
            if (_defs.TryGetValue(name, out var def))
                return def;

            return default;
        }
    }

    class ParserContext : Context<AST>
    {
        public void AddDef(string name, IdentifierAST def)
        {
            if (base.GetValue(name) is FnDefAST fnDef)
                throw new Exception($"Variable {name} already defined as a function");

            base.AddValue(name, def);
        }

        public void AddDef(string name, FnDefAST def)
        {
            if (base.GetValue(name) is IdentifierAST idDef)
                throw new Exception($"Function {name} already defined as a variable");

            base.AddValue(name, def);
        }

        public T GetDef<T>(string name) where T : AST
        {
            var def = base.GetValue(name);

            if (def == null)
            {
                var msg = typeof(T) == typeof(FnDefAST)
                    ? $"Function {name} is not defined"
                    : $"Variable {name} is not defined";

                throw new Exception(msg);
            }

            return (T) def;
        }

        public bool IsFnDef(string name)
        {
            return base.GetValue(name)?.GetType() == typeof(FnDefAST);
        }
    }

    class EvaluationContext : Context<double?>, IDisposable
    {
        private readonly EvaluationContext _parentContext;

        public EvaluationContext()
        {
        }

        private EvaluationContext(EvaluationContext parentContext)
        {
            _parentContext = parentContext;
        }

        public double GetVar(string name)
        {
            var def = base.GetValue(name) ?? _parentContext?.GetValue(name);

            if (def == null)
                throw new Exception("Variable {name} is not assigned");

            return def.Value;
        }

        public EvaluationContext CreateDisposableContext()
        {
            return new EvaluationContext(this);
        }

        public double Evaluate(AST tree, ParserContext parserCtx)
        {
            var nodeType = tree.GetType();

            if (nodeType == typeof(NumberAST))
                return Evaluate((NumberAST)tree, parserCtx);

            if (nodeType == typeof(IdentifierAST))
                return Evaluate((IdentifierAST)tree, parserCtx);

            if (nodeType == typeof(BinaryOpAST))
                return Evaluate((BinaryOpAST)tree, parserCtx);

            if (nodeType == typeof(FnCallAST))
                return Evaluate((FnCallAST)tree, parserCtx);

            throw new Exception("Expression does not conform to given grammar");
        }

        public double Evaluate(NumberAST ast, ParserContext parserCtx)
        {
            return double.Parse(ast.Value);
        }

        public double Evaluate(IdentifierAST ast, ParserContext parserCtx)
        {
            return GetVar(ast.Value);
        }

        public double Evaluate(BinaryOpAST ast, ParserContext parserCtx)
        {
            if (ast.Operator == "=")
            {
                var retVal = Evaluate(ast.Children[1], parserCtx);
                AddValue(ast.Children[0].Value, retVal);

                return retVal;
            }

            return ast.Operator switch
            {
                "+" => Evaluate(ast.Children[0], parserCtx) + Evaluate(ast.Children[1], parserCtx),
                "-" => Evaluate(ast.Children[0], parserCtx) - Evaluate(ast.Children[1], parserCtx),
                "*" => Evaluate(ast.Children[0], parserCtx) * Evaluate(ast.Children[1], parserCtx),
                "/" => Evaluate(ast.Children[0], parserCtx) / Evaluate(ast.Children[1], parserCtx),
                "%" => Evaluate(ast.Children[0], parserCtx) % Evaluate(ast.Children[1], parserCtx),
                _ => throw new Exception($"Evaluation internal exception: unknown operator {ast.Operator}")
            };
        }

        public double Evaluate(FnCallAST ast, ParserContext parserCtx)
        {
            var fnDef = parserCtx.GetDef<FnDefAST>(ast.Value);
            using var dc = CreateDisposableContext();

            fnDef.FormalParams.Zip(ast.Children).ToList().ForEach(np => dc.AddValue(np.First, Evaluate(np.Second, parserCtx)));
            return dc.Evaluate(fnDef.Body, parserCtx);
        }

        public void Dispose()
        {
        }
    }
}