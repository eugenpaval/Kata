def solution(inputArray):
    # assume the array is sorted, if not I can always sort it externally
    for i in range(len(inputArray), 0, -1):
        while i - 1 > 0 and inputArray[i] > inputArray[i - 1]:
            inputArray[i] -= inputArray[i - 1]

    return sum(inputArray)

