
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
smallRoi = resized_List[0].astype(np.float32 ) # Size = (2500,400) #.reshape(-1,400)
print type(instance.sampleList) #is ndarray
sample = smallRoi.reshape(1, 300*300)#300*300*3, achtung bei grauweirt müsste das *3 weg!!!!!!!!!!!!
instance.sampleList =  np.empty((0, 300*300))
instance.sampleList = np.append(instance.sampleList, sample, 0)
#hier anders hinzufügen????????????????????????????

listHelperResponse=[]
cv2.imshow('DigitWindow',resized_List[0])
xxx=cv2.waitKey(0) # focus have to be on an image!!!
instance.responseList.append(int(chr(xxx)))
instance.responseList = np.array(instance.responseList, np.float32)
instance.responseList = instance.responseList.reshape((instance.responseList.size,1))


# method add training data and save
knn = cv2.ml.KNearest_create()
knn.train(instance.sampleList,cv2.ml.ROW_SAMPLE,instance.responseList)#" " "instance.sampleList" " "

print "training complete"

np.savetxt('generalsamples.data',instance.sampleList)
np.savetxt('generalresponses.data',instance.responseList)


#roismall = cv2.resize(roi,(10,10))
#roismall = roismall.reshape((1,100))
#roismall = np.float32(roismall)
retval = knn.findNearest(instance.sampleList[0], k = 1)
print "retval" + retval
#knn.train(train,train_labels)



