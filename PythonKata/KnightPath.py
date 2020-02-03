import sys
from collections import deque

def knight(p1, p2):
    visited = {}
    if p1 == p2:
        return 0

    work = deque()
    work.append(p1)
    visited[p1] = 0
    minMoves = sys.maxsize

    while len(work) > 0:
        cpos = work.popleft()
        
        if visited[cpos] >= minMoves:
            continue

        moves = visited[cpos] + 1
        for node in adjacent(cpos):
            if node not in visited or visited[node] > moves:
                visited[node] = moves
                if node == p2:
                    minMoves = moves
                else:
                    work.append(node)
    
    return visited[p2]

def algToMatrixCoord(notation):
    return (8 - int(notation[1]), ord(notation[0]) - 97)

def matrixCoordToAlg(coord):
    return f"{chr(coord[1] + 97)}{8-coord[0]}"

def adjacent(notation):
    start = algToMatrixCoord(notation)
    
    result = []
    for move in [(-1, -2), (-1, 2), (1, -2), (1, 2), (-2, -1), (-2, 1), (2, -1), (2, 1)]:
        yEnd = start[0] + move[0]
        xEnd = start[1] + move[1]

        if xEnd >= 0 and xEnd < 8 and yEnd >= 0 and yEnd < 8:
            result.append((yEnd, xEnd))
    
    return list(map(lambda x: matrixCoordToAlg(x), result))