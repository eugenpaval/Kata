import unittest
from LineSafari import line

class LineSafariTests(unittest.TestCase):
    def test1(self):
        grid = ["    ",
        "X--X",
        "    ",
        "    "]
        self.assertEqual(line(grid), True)

    def test2(self):
        grid = ["     ",
        "  X  ",
        "  |  ",
        "  |  ",
        "  X  "]
        self.assertEqual(line(grid), True)

    def test3(self):
        grid = ["                    ",
        "     +--------+     ",
        "  X--+        +--+  ",
        "                 |  ",
        "                 X  ",
        "                    "]
        self.assertEqual(line(grid), True)

    def test4(self):
        grid = [\
        "                    ",
        "   +-------------+  ",
        "   |             |  ",
        " X-+      X------+  ",
        "                    "]
        self.assertEqual(line(grid), True)

    def test5(self):
        grid = [\
        "                 ",
        "   +--+          ",
        "   | +++---+     ",
        "X--+ +-+   X     "]
        self.assertEqual(line(grid), True)

    def test6(self):
        grid = ["X-----|----X"]
        self.assertEqual(line(grid), False)

    def test7(self):
        grid = [\
        "  +-+",
        "  | |",
        "X-+-+",
        "  |  ",
        "  X  "]
        self.assertEqual(line(grid), False)

    def test8(self):
        grid = [\
        "X-----+",  
        "      |",  
        "X-----+",
        "      |",  
        "------+"]
        self.assertEqual(line(grid), False)
        