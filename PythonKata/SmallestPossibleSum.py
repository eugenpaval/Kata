def solution(inputArray):
    l = len(inputArray)

    if l == 0:
        return 0
    if l == 1:
        return inputArray[0]

    gcd = steinGCD(inputArray[0], inputArray[1])
    for i in range(2, l):
        gcd = steinGCD(gcd, inputArray[i])

    return gcd * l

def steinGCD(a, b):
    if a == b:
        return a

    if a == 0 or b == 0:
        return max(a,b)

    k = 0
    while (a | b) & 1 == 0:
        a >>= 1
        b >>= 1
        k += 1

    while (a & 1) == 0:
        a >>= 1

    while True:
        while (b & 1) == 0:
            b >>= 1

        if a > b:
            t = a
            a = b
            b = t

        b -= a
        if b == 0:
            break

    return a << k