namespace InterpreterKata
{
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class InterpreterTests
    {
        private static void check(ref Interpreter interpret, string inp, double? res)
        {
            double? result = -9999.99;
            try
            {
                result = interpret.input(inp);
            }
            catch (Exception)
            {
                result = null;
            }

            if (result != res) 
                Assert.Fail("input(\"" + inp + "\") == <" + res + "> and not <" + result + "> => wrong solution, aborted!"); 
            else 
                Console.WriteLine("input(\"" + inp + "\") == <" + res + "> was ok");
        }

        [Test]
        public void BasicArithmeticTests()
        {
            Interpreter interpret = new Interpreter();
            check(ref interpret, "1 + 1", 2);
            check(ref interpret, "2 - 1", 1);
            check(ref interpret, "2 * 3", 6);
            check(ref interpret, "8 / 4", 2);
            check(ref interpret, "7 % 4", 3);
        }

        [Test]
        public void VariablesTests()
        {
            Interpreter interpret = new Interpreter();
            check(ref interpret, "x = 1", 1);
            check(ref interpret, "x", 1);
            check(ref interpret, "x + 3", 4);
            check(ref interpret, "y", null);
        }

        [Test]
        public void FunctionsTests()
        {
            Interpreter interpret = new Interpreter();
            check(ref interpret, "fn avg x y => (x + y) / 2", null);
            check(ref interpret, "avg 4 2", 3);
            check(ref interpret, "avg 7", null);
            check(ref interpret, "avg 7 2 4", null);
        }

        [Test]
        public void ConflictsTests()
        {
            Interpreter interpret = new Interpreter();
            check(ref interpret, "fn x x => 0", null);
            check(ref interpret, "fn avg => 0", null);
            check(ref interpret, "avg = 5", null);
        }

        [Test]
        public void Eval1()
        {
            var interpreter = new Interpreter();
            var a = interpreter.input("a = 40");
            Assert.AreEqual(a, 40);

            var b = interpreter.input("b = 28");
            Assert.AreEqual(b, 28);

            var c = interpreter.input("c = 10");
            Assert.AreEqual(c, 10);

            var result =  interpreter.input("(a-(b-(c-1)*2)*3)");
            Assert.AreEqual(result, 10);
        }

        [Test]
        public void Eval2()
        {
            var interpreter = new Interpreter();
            var fn = interpreter.input("fn avg x y => (x+y)/2");
            Assert.IsNull(fn);

            var result = interpreter.input("16 - avg 10 20");
            Assert.AreEqual(result, 1);
        }

        [Test]
        public void Eval3()
        {
            var interpreter = new Interpreter();
            var fn = interpreter.input("fn avg x y => (x+y)/2");
            var a = interpreter.input("a = 9");
            var b = interpreter.input("b = c = 20");
            Assert.IsNull(fn);

            var result = interpreter.input("16 - avg c=a+1 b");
            Assert.AreEqual(result, 1);
        }

        [Test]
        public void Eval4()
        {
            var interpreter = new Interpreter();

            interpreter.input("fn avg x y => (x+y)/2");
            interpreter.input("fn double x => x*2");

            var result = interpreter.input("16 - avg 10 double 10");
            Assert.AreEqual(result, 1);
        }

        [Test]
        public void Eval5()
        {
            var interpreter = new Interpreter();
            Assert.Catch<Exception>(() => interpreter.input("fn F x => x + a"));
        }

        [Test]
        public void Eval6()
        {
            var interpreter = new Interpreter();
            Assert.Catch<Exception>(() => interpreter.input("1 2"));
        }

        [Test]
        public void Eval7()
        {
            var interpreter = new Interpreter();
            interpreter.input("fn f => 1");
            Assert.AreEqual(interpreter.input("f"), 1);
        }

        [Test]
        public void Eval8()
        {
            var interpreter = new Interpreter();
            interpreter.input("fn echo x => x");
            interpreter.input("fn avg x y => (x+y)/2");
            Assert.AreEqual(interpreter.input("avg echo 4 echo 2"), 3);
        }
    }
}