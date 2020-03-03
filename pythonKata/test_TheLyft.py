#test_TheLyft
import unittest
from TheLift import Dinglemouse

class testTheLyftSuite(unittest.TestCase):
    def test1(self):
        # Floors:    G     1      2        3     4      5      6         Answers:
        tests = [[ ( (),   (),    (5,5,5), (),   (),    (),    () ),     [0, 2, 5, 0]          ],
                [ ( (),   (),    (1,1),   (),   (),    (),    () ),     [0, 2, 1, 0]          ],
                [ ( (),   (3,),  (4,),    (),   (5,),  (),    () ),     [0, 1, 2, 3, 4, 5, 0] ],
                [ ( (),   (0,),  (),      (),   (2,),  (3,),  () ),     [0, 5, 4, 3, 2, 1, 0] ]]
  
        for queues, answer in tests:
            lift = Dinglemouse(queues, 5)
            self.assertEqual(lift.theLift(), answer)
    
    