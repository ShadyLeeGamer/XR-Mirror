import threading
from CameraFeed import camera_feed
from humanSegmentation import humanSegmentation

import time
import cv2 as cv

threading.Thread(target=humanSegmentation.start).start()

while True:
    # Pass cam feed to trackers
    image = camera_feed.get_frame()
    humanSegmentation.setImg(image)
    #time.sleep(0.001)
