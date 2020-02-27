#!/bin/python3

import math
import os
import random
import re
import sys
from bisect import bisect_left

# Complete the arrayManipulation function below.
def arrayManipulation(n, queries):
    v = [0] * (n+1)
    for q in queries:
        v[q[0]-1] += q[2]
        if q[1] <= n:
            v[q[1]] -= q[2]

    m, x = 0, 0
    for value in v:
        x += value
        if m < x:
            m = x

    return m

def arrayManipulation2(n, queries):
    v = Values()
    m = 0
    for q in queries:
        m = max(v.insert(q), m)

    return m

class Values:
    def __init__(self):
        self.indices = [0]
        self.values = {0: 0}

    def insert(self, query):
        l = query[0] - 1
        r = query[1]
        v = query[2]
        m = v
        
        lo = bisect_left(self.indices, l)
        if lo == len(self.indices) or l != self.indices[lo]:
            self.indices.insert(lo, l)
            if lo != 0:
                self.values[l] = self.values[self.indices[lo-1]]
            else:
                self.values[l] = 0
        
        hi = bisect_left(self.indices, r)
        if hi == len(self.indices) or r != self.indices[hi]:
            self.indices.insert(hi, r)
            if hi < len(self.indices) - 1:
                self.values[r] = self.values[self.indices[hi-1]]
            else:
                self.values[r] = 0

        for i in range(lo, hi):
            self.values[self.indices[i]] += v
            m = max(m, self.values[self.indices[i]])

        return m

if __name__ == '__main__':
    fptr = open(os.environ['OUTPUT_PATH'], 'w')

    nm = input().split()

    n = int(nm[0])

    m = int(nm[1])

    queries = []

    for _ in range(m):
        queries.append(list(map(int, input().rstrip().split())))

    result = arrayManipulation(n, queries)

    fptr.write(str(result) + '\n')

    fptr.close()
