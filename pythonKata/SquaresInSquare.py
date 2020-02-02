from functools import reduce
from collections import deque

def decompose(n):
    for x in range(n, 0, -1):
        result = getPossibleNumbers(x, x-1)

        if result == []:
            break

        sumSquares = reduce(lambda x, y: x + y**2, result)
        if sumSquares == n ** 2:
            break

    return None

def getPossibleNumbers(target, candidate):
    if candidate == 0:
        return []

    r = int((target**2 - candidate**2)**.5)
    next = getPossibleNumbers(r, r-1)
    next.append(candidate)
    
    return next

