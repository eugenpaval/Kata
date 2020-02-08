from collections import deque

def line(grid):
    gridSizeX = len(grid[0])
    pathFound = False
    forks = deque()
    cPos = startPos(grid)
    grid[cPos[1]] = "".join(["+" if i == cPos[0] else grid[cPos[1]][i] for i in range(gridSizeX)])

    if cPos == None:
        return False

    # starting position
    forks.append(cPos)
    while len(forks) > 0:
        path = forks.popleft()
        for p in whereToGoNext(grid, cPos, path):
            if getArtifact(grid, p) == "X":
                if pathFound == True:
                    return False

                pathFound = True
            forks.append(p)
        cPos = path

    return pathFound

def whereToGoNext(grid, pPos, cPos):
    sizeX, sizeY = len(grid[0]), len(grid)
    result = []

    dPos = (pPos[0] - cPos[0], pPos[1] - cPos[1])
    b = getArtifact(grid, cPos)

    if b == "-":
        movesOnX = [(-1, 0), (1, 0)]
        delta = list(filter(lambda p: dPos != p, movesOnX))[0]
        newPos = (cPos[0] + delta[0], cPos[1] + delta[1])
        if getArtifact(grid, newPos) not in " |":
            result.append(newPos)
    elif b == "|":
        movesOnY = [(0, -1, (0, 1))]
        delta = list(filter(lambda p: dPos != p, movesOnY))[0]
        newPos = (cPos[0] + delta[0], cPos[1] + delta[1])
        if getArtifact(grid, newPos) not in " -":
            result.append(newPos)
    elif b == "+":
        possibleMoves = [d for d in [(-1,0), (1, 0), (0, -1), (0, 1)] if d != dPos]
        for delta in possibleMoves:
            deltaX, deltaY = delta
            newPos = (cPos[0] + deltaX, cPos[1] + deltaY)
            x, y = newPos
            if x < 0 or x >= sizeX or y < 0 or y >= sizeY:
                continue
            if (deltaY == 0 and getArtifact(grid, newPos) not in " |") or (deltaX == 0 and getArtifact(grid, newPos) not in " -"):
                result.append(newPos)

    return result

def startPos(grid):
    for i in range(0, len(grid)):
        xPos = grid[i].find("X")
        if xPos != -1:
            return (xPos, i)
    return None

def getArtifact(grid, pos):
    return grid[pos[1]][pos[0]]

        