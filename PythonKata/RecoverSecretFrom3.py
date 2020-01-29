class RecoverSecretFrom3:

    def __init__(self, triplets: [[]]):
        self._triplets = triplets
        self._before = {}
        self._full = {}

    def recover(self):
        for t in self._triplets:
            self._before.setdefault(t[0], set())
            self._before.setdefault(t[1], set(t[0]))
            self._before.setdefault(t[2], set(t[0:2]))

            self._before[t[1]] = self._before[t[1]] | set(t[0])
            self._before[t[2]] = self._before[t[2]] | set(t[0:2])

        for k in self._before.keys():
            self._before[k] |= self.predecessors(k)
            self._full[k] = self._before[k]
        
        sortedDep = list(self._before.items())
        sortedDep.sort(key = lambda x: len(x[1]))

        result = "".join(list(map(lambda x: x[0], sortedDep)))
        return result

    def predecessors(self, k):
        if k in self._full:
            return self._full[k]

        result = set()
        for p in self._before[k]:
            result |= set([p])
            result |= self.predecessors(p)

        return result
    
def recoverSecret(triplets):
    recovery = RecoverSecretFrom3(triplets)
    return recovery.recover()
