def solution(number):
    d3 = int((number-1) / 3)
    d5 = int((number-1) / 5)
    d15 = int((number-1) / 15)

    sum3 = 3 * d3 * (d3+1) / 2
    sum5 = 5 * d5 * (d5+1) / 2
    sum15 = 15 * d15 * (d15+1) / 2

    return sum3 + sum5 - sum15