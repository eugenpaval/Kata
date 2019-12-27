import unittest

class Test_test_1(unittest.TestCase):
    def test_A(self):
        test.describe('Example Tests')
        #test.assert_equals(solution ([9]), 9)
        test.assert_equals(solution ([6, 9, 21]), 9)
        #test.assert_equals(solution ([1, 21, 55]), 3)

if __name__ == '__main__':
    unittest.main()
