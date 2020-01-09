import unittest
import HumanTimeFormat as ht

class Test_test_1(unittest.TestCase):
    def test_now(self):
        self.assertEqual(ht.format_duration(0), "now")

    def test_1(self):
        self.assertEqual(ht.format_duration(1), "1 second")

    def test_62(self):
        self.assertEqual(ht.format_duration(62), "1 minute and 2 seconds")

    def test_120(self):
        self.assertEqual(ht.format_duration(120), "2 minutes")

    def test_3600(self):
        self.assertEqual(ht.format_duration(3600), "1 hour")

    def test_3662(self):
        self.assertEqual(ht.format_duration(3662), "1 hour, 1 minute and 2 seconds")

if __name__ == '__main__':
    unittest.main()
