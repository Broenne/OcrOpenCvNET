
#http://docs.opencv.org/3.1.0/d5/de5/tutorial_py_setup_in_windows.html#gsc.tab=0
import cv2
import numpy as np
import os
from DigitTeachHelper import resize_One_Image
from DigitNearest import DigitNearest

print "This line will be printed."
path = os.getcwd()
print path


img = cv2.imread('C:/apps/OcrOpenCvNET/MeterImage/MeterImage/BilderMartin/pic2/Snipet_WP_20160226_017_gray.jpg',0)

filename = 'C:/apps/OcrOpenCvNET/MeterImage/MeterImage/BilderMartin/pic2/Snipet_WP_20160226_017_Rects.txt'
txt = open(filename)

print "Here's your file %r:" % filename
#print txt.readline()

# read rects and add to image, then crop
roiList=[]
for line in txt:        
        print line,
        rectArray=line.split(';') 
        top_Left_X = int(rectArray[0])
        top_Left_Y = int(rectArray[1])
        bottom_Right_X = int(rectArray[2])
        bottom_Right_Y = int(rectArray[3].rstrip('\n'))#backslash n weg
        cv2.rectangle(img,(top_Left_X,top_Left_Y),(bottom_Right_X,bottom_Right_Y),(0,255,0),3); 
        roi = img[top_Left_Y:bottom_Right_Y, top_Left_X:bottom_Right_X]#roi = im[y1:y2, x1:x2]
        roiList.append(roi)
        

cv2.imshow('ImageWindow',img)
cv2.waitKey(0)

#resize the image
resized_List=[]
for roi in roiList: 
    resized_image = resize_One_Image(roi)
    resized_List.append(resized_image) 

#image to float

#train
# http://stackoverflow.com/questions/9413216/simple-digit-recognition-ocr-in-opencv-python
instance = DigitNearest()
listHelperResponse=[]
helperForTest =[]
size = 300
instance.sampleList =  np.empty((0, size*size), np.float32)
for resized in resized_List : 
    smallRoi = resized.astype(np.float32 ) # Size = (2500,400) #.reshape(-1,400)
    print type(instance.sampleList) #is ndarray
    sample = smallRoi.reshape(1, size*size)#300*300*3, achtung bei grauweirt müsste das *3 weg!!!!!!!!!!!!
    helperForTest.append(sample) #HELPER
    print "length" + str(len(helperForTest))    
    instance.sampleList = np.append(instance.sampleList, sample, 0)
    # print 'instance.sampleList[0]' + str(type(instance.sampleList[0]))

    cv2.imshow('DigitWindow',resized)
    xxx=cv2.waitKey(0) # focus have to be on an image!!!
    instance.responseList.append(int(chr(xxx)))


instance.responseList = np.array(instance.responseList, np.float32)
instance.responseList = instance.responseList.reshape((instance.responseList.size,1))    

np.savetxt('generalsamples.data',instance.sampleList)
np.savetxt('generalresponses.data',instance.responseList)


#LOAD!!!!!!!!!!!!
samples = np.loadtxt('generalsamples.data', np.float32)
responses = np.loadtxt('generalresponses.data', np.float32)
responses = responses.reshape((responses.size, 1))

# method add training data and save
knn = cv2.ml.KNearest_create()
knn.train(samples,cv2.ml.ROW_SAMPLE,responses)#" " "instance.sampleList" " "#.ROW_SAMPLE ,.COL_SAMPLE

print "training complete"

for test in helperForTest :
    retval , results, neigh_resp, dists = knn.findNearest(test, k = 1)
    string = str(int((results[0][0])))
    print "dist: " + str(dists) + "response " + string
    print "result: ", results
    print "neighbours: ", neigh_resp
    print "distance: ", dists, "\n"


