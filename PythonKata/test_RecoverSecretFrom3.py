import unittest
import RecoverSecretFrom3 as rs

class Test_test_1(unittest.TestCase):
    def testR1(self):
        secret = "whatisup"
        triplets = [
                      ['t','u','p'],
                      ['w','h','i'],
                      ['t','s','u'],
                      ['a','t','s'],
                      ['h','a','p'],
                      ['t','i','s'],
                      ['w','h','s']
                    ]

        self.assertEqual(rs.recoverSecret(triplets), secret)

    def test_R2(self):
        triplets = [
            ['t', 's', 'f'], 
            ['a', 's', 'u'], 
            ['m', 'a', 'f'],
            ['a', 'i', 'n'], 
            ['s', 'u', 'n'], 
            ['m', 'f', 'u'],
            ['a', 't', 'h'],
            ['t', 'h', 'i'],
            ['h', 'i', 'f'],
            ['m', 'h', 'f'],
            ['a', 'u', 'n'],
            ['m', 'a', 't'],
            ['f', 'u', 'n'],
            ['h', 's', 'n'],
            ['a', 'i', 's'],
            ['m', 's', 'n'],
            ['m', 's', 'u']
            ]
        self.assertEqual(rs.recoverSecret(triplets), "mathisfun")

if __name__ == '__main__':
    unittest.main()
