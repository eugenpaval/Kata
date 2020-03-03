#TheLift.py
from bisect import bisect_right, bisect_left
from collections import deque

class Elevator(object):
    capacity: int
    destinations: [int]
    direction: str
    currentFloor: int

    def __init__(self, capacity):
        self.capacity = capacity
        self.direction = "UP"
        self.currentFloor = 0

    def nextFloorsStop(self):
        if self.direction =="UP": 
            floors = [f for f in self.destinations if f > self.currentFloor]
        else:
            floors = [f for f in reversed(self.destinations) if f < self.currentFloor]
        
        return deque(floors)

    def goUp(self):
        self.direction = "UP"

    def goDown(self):
        self.direction = "DOWN"

    def addPeopleToElevator(self, queue):
        if self.direction == "UP":
            queue = [f for f in queue if f > self.currentFloor]
        else:
            queue = [f for f in queue if f < self.currentFloor]

        while len(queue) > 0 and self.capacity > len(self.destinations):
            bisect_right(self.destinations, queue[0])
            del queue[0]

    def remPeopleFromElevator(self):
        for d in self.destinations:
            if d == self.currentFloor:
                del d

    def stopAtFloor(self, floor):
        self.currentFloor = floor

class Floors(object):
    queue: [[int]]

    def __init__(self, queues):
        self.queue = [[destination for destination in floor] for floor in queues]

class Building(object):
    def __init__(self, queues, capacity):
        self.floors = Floors(queues)
        self.elevator = Elevator(capacity)

class Dinglemouse(object):
    def __init__(self, queues, capacity):
        self.building = Building(queues, capacity)
        
    def theLift(self):
        return []