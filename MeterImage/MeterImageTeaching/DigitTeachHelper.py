import cv2

def resize_One_Image( roi ):
    resized_image = cv2.resize(roi, (300, 300)) 
    #cv2.imshow('DigitWindow',resized_image)
    #cv2.waitKey(0)
    #cv2.destroyAllWindows()
    return resized_image;
