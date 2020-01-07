import unittest
import Multiples3or5

class Multiples3or5Tests(unittest.TestCase):
    def test10(self):
        self.assertEqual(78, Multiples3or5.solution(20))
