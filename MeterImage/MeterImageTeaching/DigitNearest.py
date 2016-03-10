# coding=utf-8

import cv2
import numpy as np

class DigitNearest(object):
    """description of class"""
    #sampleList
    #responseList
    sampleList=[]
    responseList=[]
    size = 300
    helperForTest =[]
    #def __init__():


    def train(self, resizedList):
            print "In train method"           
            self.sampleList =  np.empty((0, self.size*self.size), np.float32) #ACHTUNG, die größe ist hier quatsch. wie veile will ich trainieren?
            for resized in resizedList : 
                smallRoi = resized.astype(np.float32 ) 
                sample = smallRoi.reshape(1, self.size*self.size)
                self.helperForTest.append(sample) #HELPER
                #print "length" + str(len(helperForTest))    
                self.sampleList = np.append(self.sampleList, sample, 0)
                # print 'instance.sampleList[0]' + str(type(instance.sampleList[0]))

                cv2.imshow('DigitWindow',resized)
                xxx=cv2.waitKey(0) # focus have to be on an image!!!
                self.responseList.append(int(chr(xxx)))

            self.responseList = np.array(self.responseList, np.float32)
            self.responseList = self.responseList.reshape((self.responseList.size,1))    

            np.savetxt('generalsamples.data', self.sampleList)
            np.savetxt('generalresponses.data', self.responseList)
        
