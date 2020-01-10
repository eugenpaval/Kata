class Pandigital(object):
    """Numbers in base 10 with all digits being unique"""

    def get_sequence(offset, size):
        if size == 0:
            return []

        used = [False, False, False, False, False, False, False, False, False, False]
        digits = makeDigits(offset)

        if digits[0] == 0:
            digits[0] = 1
            used[1] = True




    def permutations(listToPermutate, size):
        if size == 0:
            return listToPermutate, 0

        l = len(listToPermutate)
        if l == 1:
            return listToPermutate[0], size

        for i in range(0, l):
            newList = listToPermutate.copy()
            d = newList.pop(i)
            result = permutations(newList, --size)

            for cList in result:
                cList.insert(0, d)

        return result, size
            
    def findNextUnusedDigit(d, usedDigits):


    def makeDigits(number):
        result = [0,0,0,0,0,0,0,0,0,0]
        i = 9
        while number > 0:
            number //= 10
            result[i] = number % 10
            i -= 1

        return result

    def makeNumber(digits):
        n = 0
        for i in range(0, 10):
            n += digits[i] ** (9 - i)

        return n