import unittest
import SmallestPossibleSum as sps

class Test_test_SmallestPossibleSum(unittest.TestCase):
    def test_solution1(self):
        self.assertEqual(sps.solution([9]), 9)

    def test_solution2(self):
        self.assertEqual(sps.solution([6, 9, 21]), 9)

    def test_solution3(self):
        self.assertEqual(sps.solution([1, 21, 55]), 3)

    def test_solutionr1(self):
        self.assertEqual(sps.solution([30,12]), 12)

if __name__ == '__main__':
    unittest.main()
