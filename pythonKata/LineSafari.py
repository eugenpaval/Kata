from collections import deque

def line(grid):
    solve = GridPath(grid)
    pathFound = False
    forks = deque()
    cPos = solve.StartPos

    if cPos == None:
        return False

    # starting position
    forks.append(cPos)
    while len(forks) > 0:
        path = forks.popleft()
        for p in solve.whereToGoNext(cPos, path):
            if solve.getArtifact(p) == "X":
                if pathFound == True:
                    return False
                pathFound = True
            else:
                forks.append(p)
        cPos = path

    return pathFound

class GridPath:
    def __init__(self, grid):
        self.Grid = grid
        self.Reacheable = {}
        self.SizeX, self.SizeY = len(grid[0]), len(grid)
        self.StartPos = self.startPos()
        self.Grid[self.StartPos[1]] = "".join(["+" if i == self.StartPos[0] else self.Grid[self.StartPos[1]][i] for i in range(self.SizeX)])

    def whereToGoNext(self, pPos, cPos):
        result = []

        dPos = (pPos[0] - cPos[0], pPos[1] - cPos[1])
        b = self.getArtifact(cPos)

        if b == "-":
            movesOnX = [(-1, 0), (1, 0)]
            delta = list(filter(lambda p: dPos != p, movesOnX))[0]
            newPos = (cPos[0] + delta[0], cPos[1] + delta[1])
            if self.getArtifact(newPos) not in " |":
                result.append(newPos)
        elif b == "|":
            movesOnY = [(0, -1), (0, 1)]
            delta = list(filter(lambda p: dPos != p, movesOnY))[0]
            newPos = (cPos[0] + delta[0], cPos[1] + delta[1])
            if self.getArtifact(newPos) not in " -":
                result.append(newPos)
        elif b == "+":
            possibleMoves = [d for d in [(-1,0), (1, 0), (0, -1), (0, 1)] if d != dPos]
            for delta in possibleMoves:
                deltaX, deltaY = delta
                newPos = (cPos[0] + deltaX, cPos[1] + deltaY)
                x, y = newPos
                if x < 0 or x >= self.SizeX or y < 0 or y >= self.SizeY:
                    continue
                if (deltaY == 0 and self.getArtifact(newPos) not in " |") or (deltaX == 0 and self.getArtifact(newPos) not in " -"):
                    result.append(newPos)

        return result

    def startPos(self):
        for i in range(0, self.SizeY):
            xPos = self.Grid[i].find("X")
            if xPos != -1:
                return (xPos, i)
        return None

    def getArtifact(self, pos):
        return self.Grid[pos[1]][pos[0]]

        