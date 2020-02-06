import unittest
from MaxProfitsWithKtrans import maxProfitsWithKTrans

class MaxProfitWithKTrans(unittest.TestCase):
    def test1(self):
        self.assertEqual(maxProfitsWithKTrans([], 1), 0)

    def test2(self):
        self.assertEqual(maxProfitsWithKTrans([1, 10], 1), 9)

    def test3(self):
        self.assertEqual(maxProfitsWithKTrans([5, 11, 3, 50, 60, 90], 3), 93)

    def test4(self):
        self.assertEqual(maxProfitsWithKTrans([5, 11, 3, 50, 60, 90], 3), 93)

    def test5(self):
        self.assertEqual(maxProfitsWithKTrans([1, 100, 101, 200, 201, 300, 301, 400, 401, 500], 5), 499)