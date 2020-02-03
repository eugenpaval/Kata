def getUniqueSquares(square):
    if square == 0:
        return set()

    n = int(square**.5)
    for k in range(n, 0, -1):
        diff = square
        m = k
        while diff > 0:
            diff -= m**2
            candidates = getUniqueSquares(diff)
            candidates.add(m)
            
            sumOfSquares = sum(map(lambda x: x**2, candidates))
            if sumOfSquares == square:
                return candidates

            m = int(diff**.5)


    return set()

def decompose(n):
    for x in range(n-1, 1, -1):
        f = getUniqueSquares(n**2 - x**2)
        if len(f) > 0 and x not in f:
            f.add(x)
            r = list(f)
            r.sort()
            return r

    return None