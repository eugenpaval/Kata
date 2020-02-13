from collections import deque

def fib(n):
    n = n - 1 
    numbers = deque()
    calculate = {0: 0, 1: 1}

    numbers.append(n)
    while len(numbers) > 0:
        x = numbers.popleft()
        if x > 1:
            calculate[x] = 0
            x = x // 2 if x % 2 == 0 else x // 2 + 1
            if x not in calculate:
                numbers.append(x)
            if x-1 > 1 and x-1 not in calculate:
                numbers.append(x-1)

    for f in [k for k in sorted(calculate.keys()) if k > 1]:
        index = f // 2 if f % 2 == 0 else f // 2 + 1
        calculate[f] = calculate[index] ** 2 + calculate[index-1] ** 2 if f % 2 == 1 else calculate[index] * (2 * calculate[index-1] + calculate[index])

    return calculate[n]
    