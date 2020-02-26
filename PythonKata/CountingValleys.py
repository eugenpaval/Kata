#!/bin/python3
import os

# Complete the countingValleys function below.
def countingValleys(n, s):
    v = 0
    ca, pa = 0, 0
    for step in s:
        ca += 1 if step == "U" else -1
        if ca == 0 and pa < 0:
            v += 1
        pa = ca

    return v

if __name__ == '__main__':
    fptr = open(os.environ['OUTPUT_PATH'], 'w')

    n = int(input())

    s = input()

    result = countingValleys(n, s)

    fptr.write(str(result) + '\n')

    fptr.close()
