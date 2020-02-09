def line(grid):
    solve = GridPath(grid)
    pathFound = False
    cPos = solve.StartPos

    if cPos == None:
        return False

    # starting position

    return pathFound

class GridPath:
    def __init__(self, grid):
        self.Grid = grid
        self.Visited = set()
        self.SizeX, self.SizeY = len(grid[0]), len(grid)
        self.StartPos = self.startPos()
        self.Grid[self.StartPos[1]] = "".join(["+" if i == self.StartPos[0] else self.Grid[self.StartPos[1]][i] for i in range(self.SizeX)])
        self.Moves = \
            {
                "-": [(-1, 0), (1, 0)],
                "|": [(0, -1), (0, 1)],
                "+": [(-1,0), (1, 0), (0, -1), (0, 1)]
            }
        self.Forbidden = \
            {
                "-": lambda x, deltaX, deltaY: x not in " |",
                "|": lambda x, deltaX, deltaY: x not in " -",
                "+": lambda x, deltaX, deltaY: deltaY == 0 and self.getArtifact(newPos) not in " |") or (deltaX == 0 and self.getArtifact(newPos) not in " -"
            }

    def whereToGoNext(self, cPos):
        result = []

        cArtifact = self.getArtifact(cPos)
        moves = self.Moves[cArtifact]
        for delta in moves:
            x, y = delta
            if x < 0 or x >= self.SizeX or y < 0 or y >= self.SizeY:
                continue
            newPos = (cPos[0] + delta[0], cPos[1] + delta[1])
            if self.Forbidden[cArtifact](cArtifact, deltaX, deltaY):
                result.append(newPos)

        if b == "-":
            for  delta in [(-1, 0), (1, 0)]:
                x, y = delta
                if x < 0 or x >= self.SizeX or y < 0 or y >= self.SizeY:
                    continue
                newPos = (cPos[0] + delta[0], cPos[1] + delta[1])
                if self.getArtifact(newPos) not in " |":
                    result.append(newPos)
        elif b == "|":
            for delta in [(0, -1), (0, 1)]:
                x, y = delta
                if x < 0 or x >= self.SizeX or y < 0 or y >= self.SizeY:
                    continue
                newPos = (cPos[0] + delta[0], cPos[1] + delta[1])
                if self.getArtifact(newPos) not in " -":
                    result.append(newPos)
        elif b == "+":
            for delta in [(-1,0), (1, 0), (0, -1), (0, 1)]:
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

        