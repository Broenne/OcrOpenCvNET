
#http://docs.opencv.org/3.1.0/d5/de5/tutorial_py_setup_in_windows.html#gsc.tab=0
import cv2
import numpy as np
import os

print "This line will be printed."
path = os.getcwd()
print path


img = cv2.imread('C:/apps/OcrOpenCvNET/MeterImage/MeterImage/BilderMartin/pic2/Snipet_WP_20160226_017.jpg')
cv2.imshow('ImageWindow',img)
cv2.waitKey(0)