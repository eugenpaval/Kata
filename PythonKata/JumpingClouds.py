#!/bin/python3

import math
import os
import random
import re
import sys

# Complete the jumpingOnClouds function below.
def jumpingOnClouds(c):
    moves = 0
    i = 0

    while i < len(c) - 2:
        i += 2 if c[i+2] == 0 else 1
        moves += 1

    if i < len(c) - 1:
        moves += 1

    return moves

if __name__ == '__main__':
    fptr = open(os.environ['OUTPUT_PATH'], 'w')

    n = int(input())

    c = list(map(int, input().rstrip().split()))

    result = jumpingOnClouds(c)

    fptr.write(str(result) + '\n')

    fptr.close()
