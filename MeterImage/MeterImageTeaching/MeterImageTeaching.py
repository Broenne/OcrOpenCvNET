
#http://docs.opencv.org/3.1.0/d5/de5/tutorial_py_setup_in_windows.html#gsc.tab=0
import cv2
import numpy as np
import os
from DigitTeachHelper import resize_One_Image

print "This line will be printed."
path = os.getcwd()
print path


img = cv2.imread('C:/apps/OcrOpenCvNET/MeterImage/MeterImage/BilderMartin/pic2/Snipet_WP_20160226_017.jpg')

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
    resized_List.append(resized_List) 

#image to float





