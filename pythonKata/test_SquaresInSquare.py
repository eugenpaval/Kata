import unittest
import SquaresInSquare as sq

class test_SquareInSquares(unittest.TestCase):
    def test8(self):
        self.assertEqual(sq.decompose(8), None)

    def test5(self):
        self.assertEqual(sq.decompose(5), [3,4])

    def test11(self):
        self.assertEqual(sq.decompose(11), [1,2,4,10])

    def test50(self):
        self.assertEqual(sq.decompose(50), [1, 3, 5, 8, 49])

    def test625(self):
        self.assertEqual(sq.decompose(625), [2, 5, 8, 34, 624])