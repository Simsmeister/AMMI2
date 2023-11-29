#pipe server
import sys
from body import BodyThread
import time
import struct
import global_vars
from sys import exit
import cv2



thread = BodyThread()
thread.start()



if cv2.waitKey(10) & 0xFF == ord('q'):
    print('exiting..')
    global_vars.KILL_THREADS = True
    time.sleep(0.5)
    exit()
