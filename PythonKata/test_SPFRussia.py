import unittest
import SPFRussia as spf

#Topology
#      b--d--g                                    
#     /   |   \                               
#    /    |    \                             
#   a-----e     h                                                                        
#    \     \   /                              
#     \     \ /                              
#      c-----f  

topology = {'a' : {'b': 10, 'c': 20, 'e':20},
            'b' : {'a': 10, 'd': 20},
            'c' : {'a': 10, 'f': 20},
            'd' : {'b': 10, 'e': 20, 'g': 20},
            'e' : {'a': 10, 'd': 20, 'f': 20},                      
            'f' : {'c': 10, 'e': 20, 'h': 20},
            'g' : {'d': 10, 'h': 20},
            'h' : {'g': 10, 'f': 20},
}

class Test_SPFRussia(unittest.TestCase):
    def testPerformTestAC(self):
        self.assertEqual(spf.shortestPath(topology, 'a', 'c'), [['a','c']])

    def testPerformTestAE(self):
        self.assertEqual(spf.shortestPath(topology, 'a', 'e'), [['a','e']])

    def testPerformTestAF(self):
        self.assertEqual(spf.shortestPath(topology, 'a', 'f'), [['a', 'c', 'f'], ['a', 'e', 'f']])        

    def testRandom1(self):
        self.topology = {'a': {'b': 10, 'c': 20, 'd': 20}, 
                         'b': {'a': 10, 'c': 20, 'd': 20, 'e': 20, 'f': 20, 'g': 20}, 
                         'c': {'a': 10, 'b': 20, 'e': 20}, 
                         'd': {'a': 10, 'b': 20, 'f': 20}, 
                         'e': {'b': 10, 'c': 20, 'g': 20}, 
                         'f': {'b': 10, 'd': 20, 'g': 20}, 
                         'g': {'b': 10, 'e': 20, 'f': 20}}
        self.assertEqual(spf.shortestPath(self.topology, 'c', 'g'), [['c', 'b', 'g'], ['c', 'e', 'g']])