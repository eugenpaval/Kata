def decompose(n):
    for x in range(n-1, 0, -1):
        result = list(getPossibleNumbers(n**2, x))
        result.sort()

        if result == []:
            break

        sumSquares = sum(map(lambda x: x**2, result))
        if sumSquares == n ** 2:
            return result

    return None

def getPossibleNumbers(square, candidate):
    if candidate == 0:
        return set()
    if candidate == 1:
        return set([1])

    diff = square - candidate**2
    nc = int(diff**.5)
    next = getPossibleNumbers(diff, nc)
    next.add(candidate)

    return next
