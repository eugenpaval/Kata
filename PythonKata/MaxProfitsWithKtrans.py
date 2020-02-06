import sys

def maxProfitsWithKTrans(prices, k):
    if len(prices) == 0:
        return 0

    profits = [[0 for i in range(0, len(prices))] for j in range(0, k + 1)]
    for t in range(1, k + 1):
        maxThusFar = -sys.maxsize
        for d in range(1, len(prices)):
            maxThusFar = max(maxThusFar, profits[t-1][d-1] - prices[d-1])
            profits[t][d] = max(profits[t][d-1], maxThusFar + prices[d])

    return profits[k][len(prices) - 1]