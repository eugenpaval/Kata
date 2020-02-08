import unittest
from LineSafari import line

class LineSafariTests(unittest.TestCase):
    def test1(self):
        grid = ["           ",
        "X---------X",
        "           ",
        "           "]
        self.assertEqual(line(grid), True)
        