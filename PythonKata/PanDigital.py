class Pandigital(object):
    """Numbers in base 10 with all digits being unique"""

    def get_sequence(offset, size):
        if size == 0:
            return []

        digits = makeDigits(offset)
        digits = getFirstPandigital(digits)
        digitsToPermutate = subSetToPermutate(digits, size)
        fixedDigits = digits[:len(digitsToPermutate)]
        
        result = []
        while size > 0:
            result.append(fixedDigits.append(digitsToPermutate))
            digitsToPermutate = permutate(digitsToPermutate)

            size -= size

        return result

    def getFirstPandigital(digits):
        used = [False, False, False, False, False, False, False, False, False, False]
        for i in range(0, 9):
            if digits[i] != 0:
                break
            digits[i] = i
            used[i] = True

        if digits[0] == 0:
            t = digits[0]
            digits[0] = digits[1]
            digits[1] = t

        while i < 10:
            if i + 1 < 10 and digits[i + 1] == 9:
                digits[i] = nextDigit(used, digits[i])
            else:
                digits[i] = nextDigit(used, 0)

            used[digits[i]] = True
            i += 1

        return digits


    def nextDigit(used, d):
        for i in range(d, 10):
            if used[i] == False:
                return i
        return -1

    def permutate(digits, count):
        if count == 0:
            return []

        if count == 1:
            return digits

        result = []
        for d in digits:
            if count == 0:
                break

            result.append(d)
            result.append(permutate(digits[1:]), --count)

        return result
    
    def subSetToPermutate(digits, size):
        for i in range(len(digits) - 1, 0, -1):
            pd = digits[:-i]
            p = numberOfnumberOfPermutationsAllowed(pd)

            if p >= size:
                return pd

        return digits

    def numberOfPermutationsAllowed(digits):
        l = len(digits) - 1 if len(digits) > 0 else 0
        p = list(filter(lambda x: digits[0] >= x, digits)).count
        f = factorial(l)

        return p * f if f > 0 else 1


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

    def factorial(n):
        if n == 0:
            return 0

        f = 1;
        for i in range(2, n+1):
            f *= i

        return f
