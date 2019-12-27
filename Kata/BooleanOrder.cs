using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Kata
{
    public class BooleanOrder
    {
        private readonly Dictionary<char, Func<bool, bool, bool>> _operatorsImpl = new Dictionary<char, Func<bool, bool, bool>>
        {
            ['&'] = (x, y) => x && y,
            ['|'] = (x, y) => x || y,
            ['^'] = (x, y) => x ^ y
        };

        private readonly bool[] _operands;
        private readonly char[] _operators;

        public BooleanOrder(string operands, string operators)
        {
            _operands = operands.Select(c => c == 't').ToArray();
            _operators = operators.Select(c => c).ToArray();
        }

        public BigInteger Solve()
        {
            // Your code here
            return BigInteger.Zero;
        }
    }
}
