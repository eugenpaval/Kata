#TheLift.py
class Elevator(object):
    capacity: int
    destinations: [int]
    direction: str

    def __init__(self, capacity):
        self.capacity = capacity

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
        pass
        
    def theLift(self):
        return []