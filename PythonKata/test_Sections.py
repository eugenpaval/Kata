import unittest
import Sections

class Test_Sections(unittest.TestCase):
    def performTest(self, n, expected):
        actual = Sections.c(n)
        self.assertEqual(actual, expected)

    def testBasic1(self):
        self.performTest(423128, 0)

    def testBasic2(self):
        self.performTest(1369, 4)

    def testBasic3(self):
        self.performTest(2999824, 28)

    def testBasic4(self):
        self.performTest(11710084, 64)

    def testBasic5(self):
        self.performTest(1, 1)

    def testBasic6(self):
        self.performTest(4, 4)

    def testBasic7(self):
        self.performTest(4096576, 160)

    def testBasic8(self):
        self.performTest(2019, 0)
