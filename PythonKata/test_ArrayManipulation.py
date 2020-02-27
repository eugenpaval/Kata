import unittest
from ArrayManipulation import arrayManipulation

class TestArrayManipulation(unittest.TestCase):
    def test1(self):
        queries = [[1, 2, 100], [2, 5, 100], [3, 4, 100]]
        self.assertEqual(arrayManipulation(5, queries), 200)

    def test2(self):
        queries = [[5, 15, 100], [1, 50, 50], [40, 100, 10]]
        self.assertEqual(arrayManipulation(100, queries), 150)

    def test3(self):
        queries = [[1,5,3], [4,8,7], [6,9,1]]
        self.assertEqual(arrayManipulation(9, queries), 10)

    def test4(self):
        queries = [[19, 28, 100], [4, 23, 600], [5, 6, 900], [19, 33, 500], [1, 1, 300]]
        self.assertEqual(arrayManipulation(33, queries), 1500)