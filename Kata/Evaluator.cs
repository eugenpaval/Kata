﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Kata
{
    public class Evaluator
    {
        public double Evaluate(string expression)
        {
            var operatorStack = new Stack<Operator>();
            var valueStack = new Stack<double>();
            Operator @operator = null;

            foreach (var token in Token.Parse(expression))
            {
                switch (token.Operator.Type)
                {
                    case OperatorType.Null:
                        valueStack.Push(token.Operand);
                        break;

                    case OperatorType.LParens:
                        operatorStack.Push(token.Operator);
                        break;

                    case OperatorType.RParens:
                        while (true)
                        {
                            if (operatorStack.TryPop(out @operator) && @operator.Type == OperatorType.LParens)
                                break;

                            ComputeOnStack(valueStack, @operator);
                        }
                        break;

                    case OperatorType.Mult:
                    case OperatorType.Div:
                    case OperatorType.Add:
                    case OperatorType.Sub:
                        while (true)
                        {
                            if (operatorStack.Count == 0)
                                break;
                                    
                            if (operatorStack.TryPeek(out @operator) && @operator.Priority < token.Operator.Priority)
                                break;

                            operatorStack.Pop();

                            ComputeOnStack(valueStack, @operator);
                        }

                        operatorStack.Push(token.Operator);
                        break;

                    default:
                        throw new Exception($"Illegal or unsupported expression, {expression}");
                }
            }

            while (operatorStack.Count > 0)
            {
                @operator = operatorStack.Pop();
                ComputeOnStack(valueStack, @operator);
            }

            var result = valueStack.Pop();
            return result;
        }

        private void ComputeOnStack(Stack<double> valueStack, Operator @operator)
        {
            var v1 = valueStack.Pop();
            var v2 = valueStack.Pop();
            valueStack.Push(Compute(@operator.Type, v2, v1));
        }

        private double Compute(OperatorType operatorType, in double v1, in double v2)
        {
            switch (operatorType)
            {
                case OperatorType.Add:
                    return v1 + v2;

                case OperatorType.Sub:
                    return v1 - v2;

                case OperatorType.Mult:
                    return v1 * v2;

                case OperatorType.Div:
                    return v1 / v2;

                default:
                    throw new Exception($"Unsupported operator, {operatorType.ToString()}");
            }
        }
    }

    public class Token
    {
        private static readonly (char OpChar, Operator Operator)[] _operators = 
        {
            ('*', new Operator(OperatorType.Mult)),
            ('/', new Operator(OperatorType.Div)),
            ('+', new Operator(OperatorType.Add)),
            ('-', new Operator(OperatorType.Sub)),
            ('(', new Operator(OperatorType.LParens)),
            (')', new Operator(OperatorType.RParens))
        };

        public static IEnumerable<Token> Parse(string expr)
        {
            var tokens = new List<Token>();
            
            InternalParse(new string(expr.Where(c => !char.IsWhiteSpace(c)).ToArray()), true, tokens);
            return tokens;
        }

        private static void InternalParse(string expr, bool tokenWasOperator, List<Token> tokens)
        {
            var trackParens = new ParensTrack();

            for (var i = 0; i < expr.Length;)
            {
                var t = expr[i];

                if (IsLeadingDigit(expr[i]))
                {
                    var start = i;
                    for (; i < expr.Length; ++i)
                        if (!IsDigitOrComma(expr[i]))
                            break;

                    if (double.TryParse(expr.Substring(start, i - start), out var number))
                    {
                        tokens.Add(new Token(number));
                        tokenWasOperator = false;

                        if (trackParens.CheckTracker())
                        {
                            tokens.Add(new Token(')'));
                            tokens.Add(new Token(')'));
                        }
                    }
                }
                else if (t == '*' || t == '/')
                {
                    tokens.Add(new Token(t));
                    tokenWasOperator = true;
                    ++i;
                }
                else if (t == '+' || t == '-')
                {
                    var sign = Sign(expr, ref i);
                    var op = sign > 0 ? '+' : '-';

                    if (tokenWasOperator)
                    {
                        tokens.Add(new Token('('));
                        tokens.Add(new Token(sign));
                        tokens.Add(new Token('*'));
                        tokens.Add(new Token('('));
                        
                        trackParens.AddTracker();
                    }
                    else
                        tokens.Add(new Token(op));

                    tokenWasOperator = true;
                }
                else if (t == '(' || t == ')')
                {
                    tokens.Add(new Token(t));
                    if (t == '(')
                    {
                        tokenWasOperator = true;
                        trackParens.Open();
                    }
                    else
                    {
                        tokenWasOperator = false;
                        if (trackParens.Close())
                        {
                            tokens.Add(new Token(')'));
                            tokens.Add(new Token(')'));
                        }
                    }

                    ++i;
                }
            }
        }

        private static int Sign(string expr, ref int i)
        {
            var sign = 1;
            for (; i < expr.Length; ++i)
            {
                if (expr[i] == '+')
                    continue;

                if (expr[i] == '-')
                    sign *= -1;
                else
                    break;
            }

            return sign;
        }

        private static bool IsDigitOrComma(char c)
        {
            return char.IsDigit(c) || c == '.' || c == ',';
        }

        private static bool IsLeadingDigit(char c)
        {
            return char.IsDigit(c) || c == '.';
        }

        protected Token(Operator op)
        {
            Operator = op;
        }

        protected Token(char opc)
        {
            var op = _operators.FirstOrDefault(o => o.OpChar == opc);
            if (op != default)
                Operator = new Operator(op.Operator.Type);
            else
                throw new Exception(($"Invalid parsed operator, {opc}"));
        }

        protected Token(string number)
        {
            Operator = new Operator(OperatorType.Null);
            Operand = double.Parse(number);
        }

        protected Token(double number)
        {
            Operator = new Operator(OperatorType.Null);
            Operand = number;
        }

        public double Operand { get; }
        public Operator Operator { get; }
    }

    public enum OperatorType
    {
        Null,
        Mult,
        Div,
        Add,
        Sub,
        LParens,
        RParens
    }

    public class Operator
    {
        public Operator(OperatorType opType)
        {
            Type = opType;

            if (opType == OperatorType.Mult || opType == OperatorType.Div)
                Priority = 1;
            else if (opType == OperatorType.Add || opType == OperatorType.Sub)
                Priority = 0;
            else
                Priority = -1;
        }
        public OperatorType Type { get; }
        public int Priority { get; }
    }

    public class ParensTrack
    {
        private List<int> _parens = new List<int>();

        public void Open()
        {
            for (var i = 0; i < _parens.Count; ++i)
                _parens[i] = ++_parens[i];
        }

        public void AddTracker()
        {
            _parens.Add(0);
        }

        public bool Close()
        {
            for (var i = 0; i < _parens.Count; ++i)
                _parens[i] = --_parens[i];

            return CheckTracker();
        }

        public bool CheckTracker()
        {
            var i = 0;
            for (; i < _parens.Count; ++i)
                if (_parens[i] == 0)
                    break;

            if (i < _parens.Count)
            {
                _parens.RemoveAt(i);
                return true;
            }

            return false;
        }
    }
}
