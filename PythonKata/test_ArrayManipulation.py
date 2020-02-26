import unittest
from ArrayManipulation import arrayManipulation

class TestArrayManipulation(unittest.TestCase):
    def test1(self):
        queries = [[1, 2, 100], [2, 5, 100], [3, 4, 100]]
        self.assertEqual(arrayManipulation(5, queries), 200)