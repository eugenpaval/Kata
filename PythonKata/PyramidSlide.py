def longest_slide_down(pyramid):
    if len(pyramid) == 0:
        return 0

    for level in range(len(pyramid) - 1, 0, -1):
        for i in range(0, len(pyramid[level]) - 1):
            value = max(pyramid[level][i] + pyramid[level-1][i], pyramid[level][i+1] + pyramid[level-1][i])
            pyramid[level-1][i] = value

    return pyramid[0][0]