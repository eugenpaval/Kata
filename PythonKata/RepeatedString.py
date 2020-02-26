#!/bin/python3

import math
import os
import random
import re
import sys
from functools import reduce

# Complete the repeatedString function below.
def repeatedString(s, n):
    countFullS = n // len(s)
    subS = s[0 : n % len(s)]

    return countFullS * reduce(lambda x, y: x + y, map(lambda x: 1 if x == "a" else 0, s), 0) + reduce(lambda x, y: x + y, map(lambda x: 1 if x == "a" else 0, subS), 0)

if __name__ == '__main__':
    fptr = open(os.environ['OUTPUT_PATH'], 'w')

    s = input()

    n = int(input())

    result = repeatedString(s, n)

    fptr.write(str(result) + '\n')

    fptr.close()
