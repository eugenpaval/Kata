import unittest
import KnightPath as k

class testKnightPath(unittest.TestCase):
    def testMultiple(self):
        arr = [['a1', 'c1', 2], ['a1', 'f1', 3], ['a1', 'f3', 3], ['a1', 'f4', 4], ['a1', 'f7', 5]]
        for x in arr:
            z = k.knight(x[0], x[1])
            self.assertEqual(z, x[2], "{} to {}: expected {}, got {}".format(x[0], x[1], x[2], z))