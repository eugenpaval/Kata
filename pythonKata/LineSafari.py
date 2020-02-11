def line(grid):
    maze = GridPath(grid)
    return maze.doesItHaveOnlyOnePath()

class GridPath:
    def __init__(self, grid):
        self.Grid = grid
        self.SizeX, self.SizeY = len(grid[0]), len(grid)
        self.OnePath = 0
        self.Reachable = {}
        self.StartPos = self.startPos()
        self.Grid[self.StartPos[1]] = "".join(["+" if i == self.StartPos[0] else self.Grid[self.StartPos[1]][i] for i in range(self.SizeX)])
        self.Moves = \
            {
                "-": [(-1, 0), (1, 0)],
                "|": [(0, -1), (0, 1)],
                " ": []
            }
        self.Forbidden = \
            {
                "-": lambda artifact, deltaX, deltaY: artifact not in " |",
                "|": lambda artifact, deltaX, deltaY: artifact not in " -",
            }

    def startPos(self):
        for i in range(0, self.SizeY):
            xPos = self.Grid[i].find("X")
            if xPos != -1:
                return (xPos, i)
        return None

    def getArtifact(self, pos):
        if pos[0] < 0 or pos[0] >= self.SizeX or pos[1] < 0 or pos[1] >= self.SizeY:
            return " "
        return self.Grid[pos[1]][pos[0]]

    def findPaths(self, pPos, cPos, alreadyVisited):
        if self.OnePath < 1:
            result = 0
            alreadyVisited.append(cPos)
            self.Reachable[cPos] = 1 if cPos not in self.Reachable else self.Reachable[cPos] + 1
            for p in self.whereToGoNext(pPos, cPos):
                if self.getArtifact(p) == 'X':
                    result = 1
                    break
                
                if p not in alreadyVisited:
                    result += self.findPaths(cPos, p, alreadyVisited)
                    if result > 1:
                        self.OnePath = 2
                        break
                else:
                    self.Reachable[p] += 1

            alreadyVisited.pop()
        
        return result

    def doesItHaveOnlyOnePath(self):
        result = self.findPaths(self.StartPos, self.StartPos, [])

        if result == 1:
            for c in self.Reachable.values():
                if c > 1:
                    return False
            else:
                return True

        return False

    def whereToGoNext(self, pMove, pos):
        dx, dy = pMove
        result = []
        pArtifact = " "
        artifact = self.getArtifact(pos)

        if artifact == "+":
            if dx != 0:
                for m in [(0, -1), (0, 1)]:
                    p = (0, pos[1] + m[1])
                    artifact = self.getArtifact(p)
                    if artifact in "|+" and artifact != pArtifact:
                        result.append(p)
                    pArtifact = artifact
            elif dy != 0:
                for m in [(-1, 0), (1, 0)]:
                    p = (pos[0] + m[0], 0)
                    artifact = self.getArtifact(p)
                    if artifact in "-+" and artifact != pArtifact:
                        result.append(p)
                    pArtifact = artifact
        elif artifact in "-|":
            for delta in [m for m in self.Moves[artifact] if m != pMove]:
                newX, newY = pos[0] + delta[0], pos[1] + delta[1]
                
                nArtifact = self.getArtifact((newX, newY))
                if self.Forbidden[artifact](nArtifact, delta[0], delta[1]):
                    result.append((newX, newY))

        return result[0] if len(result) == 1 else ()