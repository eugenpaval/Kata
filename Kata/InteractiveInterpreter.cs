using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace InterpreterKata
{
    public class Interpreter
    {
        private readonly Parser _parser = new Parser();
        private readonly EvaluationContext _evalCtx = new EvaluationContext();

        public double? input(string input)
        {
            var tokens = Tokenize(input);
            if (!tokens.Any())
                return null;

            var ast = _parser.Parse(tokens);

            try
            {
                return _evalCtx.Evaluate(ast, _parser.Context);
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
        EOF
    }

    class Lexer
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

        private readonly IEnumerator<string> _tokens;
        private bool _eof;

        public static TokenType GetTokenType(string token)
        {
            if (double.TryParse(token, out var val))
                return TokenType.Number;

            if (Regex.Match(token, "[A-Za-z_][A-Za-z0-9_]*").Success)
                return TokenType.Identifier;

            if (Regex.Match(token, "[-+*/%=\\(\\)]").Success)
                return _opTokenToType[token];

            return TokenType.Unknown;
        }

        public Lexer(IEnumerable<string> tokens)
        {
            _tokens = tokens.GetEnumerator();
            _eof = !_tokens.MoveNext();

            CurrentToken = new Token(_tokens.Current, _eof ? TokenType.EOF : GetTokenType(_tokens.Current));
        }

        public Token CurrentToken { get; private set; }

        public void AcceptToken(TokenType tokenType)
        {
            if (CurrentToken.Type == tokenType)
            {
                _eof = !_tokens.MoveNext();
                CurrentToken = new Token(_tokens.Current, _eof ? TokenType.EOF : GetTokenType(_tokens.Current));
            }
            else
                throw new Exception($"Invalid syntax error - unexpected token {CurrentToken.Value} , {CurrentToken.Type}");
        }
    }

    class Token
    {
        public Token(string value, TokenType tokenType) => (Value, Type) = (value, tokenType);

        public string Value { get; }
        public TokenType Type { get; }
    }

    class Parser
    {
        private Lexer _lexer;
        private static readonly TokenType[] _allowedOpsAfterFnCall =
        {
            TokenType.Add, TokenType.Sub, TokenType.Mul, TokenType.Div, TokenType.Mod, TokenType.EOF
        };

        public Parser()
        {
            Context = new ParserContext();
        }

        public ParserContext Context { get; }

        public AST Parse(IEnumerable<string> tokens)
        {
            _lexer = new Lexer(tokens);

            if (_lexer.CurrentToken.Type == TokenType.EOF)
                return null;

            var tree = _lexer.CurrentToken.Value.ToLower() == "fn" ? ParseFnDef() : ParseExpr();
            _lexer.AcceptToken(TokenType.EOF);
            return tree;
        }

        private AST ParseFnDef()
        {
            _lexer.AcceptToken(TokenType.Identifier);
            var fName = _lexer.CurrentToken.Value;
            _lexer.AcceptToken(TokenType.Identifier);

            var formalParams = new List<string>();

            while (_lexer.CurrentToken.Type == TokenType.Identifier)
            {
                formalParams.Add(_lexer.CurrentToken.Value);
                _lexer.AcceptToken(TokenType.Identifier);
            }

            _lexer.AcceptToken(TokenType.FnBodyDef);

            var body = ParseExpr(formalParams);
            var fnDef = new FnDefAST(fName, formalParams, body);
            
            Context.AddDef(fName, fnDef);
            return fnDef;
        }

        private AST ParseExpr(List<string> ns = null)
        {
            // expr ::= term | (+\-) factor
            var term = ParseTerm(ns);
            var token = _lexer.CurrentToken;

            while (token.Type == TokenType.Add || token.Type == TokenType.Sub)
            {
                _lexer.AcceptToken(token.Type);

                var otherTerm = ParseTerm(ns);
                term = new BinaryOpAST(term, token.Value, otherTerm);
                token = _lexer.CurrentToken;
            }

            return term;
        }

        private AST ParseTerm(List<string> ns)
        {
            // term ::= factor | (*\/|%) factor
            var factor = ParseFactor(ns);
            var token = _lexer.CurrentToken;

            while (token.Type == TokenType.Mul || token.Type == TokenType.Div || token.Type == TokenType.Mod)
            {
                _lexer.AcceptToken(token.Type);

                var otherFactor = ParseFactor(ns);
                factor = new BinaryOpAST(factor, token.Value, otherFactor);
                token = _lexer.CurrentToken;
            }

            return factor;
        }

        private AST ParseFactor(List<string> ns)
        {
            // factor ::= number | identifier | assignment | '('expr')' | fn-call
            AST node = null;

            switch (_lexer.CurrentToken.Type)
            {
                case TokenType.Number:
                    node = new NumberAST(_lexer.CurrentToken.Value);
                    _lexer.AcceptToken(TokenType.Number);
                    break;

                case TokenType.Identifier:
                    node = Context.IsFnDef(_lexer.CurrentToken.Value) ? ParseFnCall() : ParseIdentifierRef(ns);
                    node = ParseAssignment(node);
                    break;

                case TokenType.LParen:
                    _lexer.AcceptToken(TokenType.LParen);
                    node = ParseExpr();
                    _lexer.AcceptToken(TokenType.RParen);
                    break;
            }

            return node;
        }

        private AST ParseAssignment(AST node)
        {
            while (_lexer.CurrentToken.Type == TokenType.Assign)
            {
                if (node.GetType() == typeof(FnCallAST))
                    throw new Exception($"Syntax error - function {node.Value} cannot be assigned to");

                _lexer.AcceptToken(TokenType.Assign);
                var expr = ParseExpr();
                node = new BinaryOpAST(node, "=", expr);
            }

            return node;
        }

        private AST ParseIdentifierRef(List<string> ns)
        {
            if (ns != null && !ns.Contains(_lexer.CurrentToken.Value))
                throw new Exception($"Parameter {_lexer.CurrentToken.Value} is not defined");

            var node = new IdentifierAST(_lexer.CurrentToken.Value);
            _lexer.AcceptToken(TokenType.Identifier);
            return node;
        }

        private AST ParseFnCall()
        {
            var fnDef = Context.GetDef<FnDefAST>(_lexer.CurrentToken.Value);
            _lexer.AcceptToken(TokenType.Identifier);

            var parameters = fnDef.FormalParams.Select(p => ParseExpr());
            var node = new FnCallAST(fnDef.Value, parameters);

            //if (!_allowedOpsAfterFnCall.Contains(_lexer.CurrentToken.Type))
            //    throw new Exception($"Unexpected token after {fnDef.Value}");

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
        public FnDefAST(string fName, List<string> formalParams, AST body)
        {
            Value = Operator = fName;
            FormalParams = formalParams;
            Body = body;
        }

        public AST Body { get; }

        public List<string> FormalParams { get; }
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
                throw new Exception($"Variable {name} is not assigned");

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
            
            if (fnDef.FormalParams.Count != ast.Children.Count)
                throw new Exception($"Function {fnDef.Value} has {fnDef.FormalParams.Count} formal parameters but it is called with {ast.Children.Count} actual parameters");

            using var dc = CreateDisposableContext();

            fnDef.FormalParams.Zip(ast.Children).ToList().ForEach(np => dc.AddValue(np.First, Evaluate(np.Second, parserCtx)));
            return dc.Evaluate(fnDef.Body, parserCtx);
        }

        public void Dispose()
        {
        }
    }
}