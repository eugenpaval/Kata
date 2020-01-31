import sys
from collections import Counter, deque

class SPFRussia:
    def __init__(self, topology):
        self._topology = topology
        self._counters = {}
        self._visited = set()

        for c in self._topology.keys():
            self._counters[c] = Counter()

    def findPaths(self, start, end):
        work = deque()
        work.append((start, start, 0))

        while len(work) > 0:
            cnode, prevnode, distance = work.popleft()
            self._visited.add(cnode)
            if self._counters[cnode][prevnode] == 0 or self._counters[cnode][prevnode] > distance:
                self._counters[cnode][prevnode] = distance

            for node in self._topology[cnode].keys():
                if node not in self._visited:
                    work.append((node, cnode, distance + self._topology[cnode][node]))
        
        return self.composePaths(start, end)

    def composePaths(self, start, end):
        result = []
        counter = self._counters[end]
        minDistance = min(list(map(lambda x: x[1], counter.items())))
        minPathNodes = list(map(lambda x: x[0], filter(lambda x: x[1] == minDistance, counter.items())))

        for n in minPathNodes:
            path = self.composePathForNode(start, n)
            path.extend(end)
            result.append(path)

        return result


    def composePathForNode(self, start, node):
        counter = self._counters[node]
        if list(counter.items())[0][1] == 0:
            return [list(counter.items())[0][0]]

        minDistance = min(list(map(lambda x: x[1], counter.items())))
        minPathNodes = list(map(lambda x: x[0], filter(lambda x: x[1] == minDistance, counter.items())))

        if start in minPathNodes:
            minPathNodes = [start]
            
        result = []
        for n in minPathNodes:
            newPath = self.composePathForNode(start, n)
            newPath.extend(node)
            result.extend(newPath)

        return result

def shortestPath(topology, startPoint, endPoint):
    spfRussia = SPFRussia(topology)
    
    result = spfRussia.findPaths(startPoint, endPoint)
    return result