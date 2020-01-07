import unittest
import LongestPalindromicString as lps

class self_self_1(unittest.TestCase):
    def testOddLength(self):
        self.assertEqual(lps.longest_palindrome('babad'), 'bab')
        self.assertEqual(lps.longest_palindrome('madam'), 'madam')
        self.assertEqual(lps.longest_palindrome('dde'), 'dd')
        self.assertEqual(lps.longest_palindrome('ababbab'), 'babbab')
        self.assertEqual(lps.longest_palindrome('abababa'), 'abababa')
        print('<COMPLETEDIN::>')

    def testEvenLength(self):
        self.assertEqual(lps.longest_palindrome('banana'), 'anana')
        self.assertEqual(lps.longest_palindrome('abba'), 'abba')
        self.assertEqual(lps.longest_palindrome('cbbd'), 'bb')
        self.assertEqual(lps.longest_palindrome('zz'), 'zz')
        self.assertEqual(lps.longest_palindrome('dddd'), 'dddd')
        print('<COMPLETEDIN::>')

    def testEdge(self):
        self.assertEqual(lps.longest_palindrome(''), '')
        self.assertEqual(lps.longest_palindrome('ttaaftffftfaafatf'), 'aaftffftfaa')
        self.assertEqual(lps.longest_palindrome('bbaaacc'), 'aaa')
        print('<COMPLETEDIN::>')

    def testEdgeSingle(self):
        self.assertEqual(lps.longest_palindrome('m'), 'm')

    def testEdgeMultiple(self):
        self.assertEqual(lps.longest_palindrome('abcdefghijklmnopqrstuvwxyz'), 'a')

if __name__ == '__main__':
    unittest.main()
