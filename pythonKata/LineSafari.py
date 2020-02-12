def line(grid):
    maze = GridPath(grid)
    return maze.findAllPaths()

class GridPath:
    def __init__(self, grid):
        self.Grid = grid
        self.SizeX, self.SizeY = len(grid[0]), len(grid)
        self.Moves = \
        {
            "-": lambda moveToPrev: [m for m in [(-1, 0), (1, 0)] if m != moveToPrev],
            "|": lambda moveToPrev: [m for m in [(0, -1), (0, 1)] if m != moveToPrev],
            "+": lambda moveToPrev: [(0, -1), (0, 1)] if moveToPrev[0] != 0 else [(-1, 0), (1, 0)],
            " ": lambda moveToPrev: []
        }
        self.NextPosition = \
        {
            "-": self.nextPositionFromX,
            "|": self.nextPositionFromY,
            "+": self.nextPositionFromCross,
            "X": self.nextPositionFromSpot
        }
        self.Spots = self.findSpots()

    def findSpots(self):
        spots = []
        for i in range(0, self.SizeY):
            for xPos in range(len(self.Grid[i])):
                if self.Grid[i][xPos] == "X":
                    spots.append((xPos, i))
        
        return spots

    def getArtifact(self, pos):
        if pos[0] < 0 or pos[0] >= self.SizeX or pos[1] < 0 or pos[1] >= self.SizeY:
            return " "
        return self.Grid[pos[1]][pos[0]]

    def findAllPaths(self):
        result = 0
        for start in self.Spots:
            result += self.findPaths(start, start)
            if result == 1:
                return True

        return result == 1

    def findPaths(self, pPos, cPos):
        while True:
            currentArtifact = self.getArtifact(cPos)
            pMove = (pPos[0] - cPos[0], pPos[1] - cPos[1])
            p = self.NextPosition[currentArtifact](pMove, cPos)

            if p == ():
                return 0
            if self.getArtifact(p) == 'X':
                return 1

            pPos = cPos
            cPos = p

    def nextPositionFromX(self, pMove, cPos):
        deltaX, deltaY = self.Moves["-"](pMove)[0]
        newPosition = (cPos[0] + deltaX, cPos[1] + deltaY)
        nArtifact = self.getArtifact(newPosition)
        
        if nArtifact not in " |":
            return newPosition

        return ()

    def nextPositionFromY(self, pMove, cPos):
        deltaX, deltaY = self.Moves["|"](pMove)[0]
        newPosition = (cPos[0] + deltaX, cPos[1] + deltaY)
        nArtifact = self.getArtifact(newPosition)
        
        if nArtifact not in " -":
            return newPosition

        return ()

    def nextPositionFromCross(self, pMove, cPos):
        result = []
        allowedArtifacts = "|+" if pMove[0] != 0 else "-+"

        for m in self.Moves["+"](pMove):
            p = (cPos[0] + m[0], cPos[1] + m[1])
            artifact = self.getArtifact(p)

            if artifact in allowedArtifacts:
                result.append(p)

        return result[0] if len(result) == 1 else ()

    def nextPositionFromSpot(self, pMove, cPos):
        result = []
        for p in [m for m in [(-1, 0), (1, 0), (0, -1), (0, 1)] if m != pMove]:
            newPos = (cPos[0] + p[0], cPos[1] + p[1])
            nArtifact = self.getArtifact(newPos)
            if p[0] == 0:
                if nArtifact not in " -":
                    result.append(newPos)
            elif p[1] == 0:
                if nArtifact not in " |":
                    result.append(newPos)

        return result[0] if len(result) == 1 else ()
