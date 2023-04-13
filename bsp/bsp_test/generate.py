#!/usr/bin/env python
#! -*- coding: UTF-8 -*-
"""
Generation data to test seamfind.c
In this code, all c language programs have been modified
to be work standalone.
"""

import math
# import numpy as np
import matplotlib.pyplot as plt
import random

class Generate():
    def  __init__(self, number):
        """ number: the number of data  """
        self.number = number
    
    def gaussian(self, peak, center, half_width):
        i_position = list(range(0, self.number))
        self.bright = [0 for i in range(0, self.number)]

        for i in i_position:
            location = (center-i)*(center-i)
            self.bright[i] = peak*math.exp(-location/(half_width*half_width))
            self.bright[i] = self.bright[i] + random.random()*0.05*peak
        
        # print(self.bright)
        
    def outfile(self):
        f = open('gdata.dat','w')
        f.write(str(self.bright))
        f.close()
    
    def plot_out(self):
        i_position = list(range(0, self.number))
        plt.scatter(i_position, self.bright, s=1)
        plt.show()



#>>>>>>>>>>>>>>>
# main program
#>>>>>>>>>>>>>>>
if __name__ == '__main__':
    it_generate = Generate(300)
    it_generate.gaussian(peak=700, center=150, half_width=50)
    it_generate.outfile()
    it_generate.plot_out()
