#!/bin/python3

import math
import os
import random
import re
import sys
from bisect import bisect_left

# Complete the arrayManipulation function below.
def arrayManipulation(n, queries):
    v = Values()
    m = 0
    for q in queries:
        m = max(v.insert(q), m)

    return max

class Values:
    def __init__(self):
        self.indices = []
        self.values = {}

    def insert(self, query):
        l = query[0] - 1
        r = query[1] - 1
        v = query[2]
        m = v
        
        lo = bisect_left(self.indices, l)
        if lo == len(self.indices) or lo != self.indices[lo]:
            self.indices.insert(lo, l)
            self.values[l] = 0
        
        hi = bisect_left(self.indices, r)
        if hi == len(self.indices) or hi != self.indices[hi]:
            if hi < len(self.indices) - 1:
                self.indices.insert(hi, r + 1)
                self.values[r+1] = self.values[self.indices[hi-1]]
            else:
                self.indices.insert(hi, r)
                self.values[r] = self.values[self.indices[hi-1]]

        for i in range(lo, hi + 1):
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
