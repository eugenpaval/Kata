import unittest
from Debugger import Debugger, Meta

class Foo(object, metaclass = Meta):
    def __init__(self, x):
        self.x = x

    def bar(self, v):
        return (self.x, v)

class testDebuggerSuite(unittest.TestCase):
    def test1(self):
        a = Foo(1)
        a.bar(2)

        calls = Debugger.method_calls

        self.assertEqual(len(calls), 2)

        self.assertEqual(calls[0]['args'], (a, 1))

        self.assertEqual(calls[1]['args'], (a, 2))

        accesses = Debugger.attribute_accesses

        self.assertEqual(len(accesses), 3)

        self.assertEqual(accesses[0]['action'], 'set')
        self.assertEqual(accesses[0]['attribute'], 'x')
        self.assertEqual(accesses[0]['value'], 1)

        self.assertEqual(accesses[1]['action'], 'get')
        self.assertEqual(accesses[1]['attribute'], 'bar')

        self.assertEqual(accesses[2]['action'], 'get')
        self.assertEqual(accesses[2]['attribute'], 'x')