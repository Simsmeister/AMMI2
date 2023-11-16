# Import necessary libraries
import mediapipe as mp
import cv2
import threading
import time
import global_vars 
import struct
import math

# the capture thread captures images from the WebCam on a separate thread (for performance)
class CaptureThread(threading.Thread):
    cap = None
    ret = None
    frame = None
    isRunning = False
    counter = 0
    timer = 0.0
    def run(self):
        self.cap = cv2.VideoCapture(0) # sometimes it can take a while for certain video captures
        if global_vars.USE_CUSTOM_CAM_SETTINGS:
            self.cap.set(cv2.CAP_PROP_FPS, global_vars.FPS)
            self.cap.set(cv2.CAP_PROP_FRAME_WIDTH,global_vars.WIDTH)
            self.cap.set(cv2.CAP_PROP_FRAME_HEIGHT,global_vars.HEIGHT)

        time.sleep(1)
        
        print("Opened Capture @ %s fps"%str(self.cap.get(cv2.CAP_PROP_FPS)))
        while not global_vars.KILL_THREADS:
            self.ret, self.frame = self.cap.read()
            self.isRunning = True
            if global_vars.DEBUG:
                self.counter = self.counter+1
                if time.time()-self.timer>=3:
                    print("Capture FPS: ",self.counter/(time.time()-self.timer))
                    self.counter = 0
                    self.timer = time.time()

# Jumping Jack detection logic
class JumpingJackDetector:
    def __init__(self):
        self.N = 0
        self.theta1 = 0
        self.setp = []
        self.r = 0

    def update(self, landmarks):
        if bool(landmarks):
            abx = landmarks[14][0] - landmarks[12][0]
            aby = landmarks[14][1] - landmarks[12][1]
            acx = landmarks[24][0] - landmarks[12][0]
            acy = landmarks[24][1] - landmarks[12][1]
            theta = math.acos((abx * acx + aby * acy) / ((math.sqrt(abx**2 + aby**2)) * (math.sqrt(acx**2 + acy**2))))

            if theta < math.pi/4:
                self.theta1 = theta
            if theta > 3 * math.pi/4:
                if self.theta1 < math.pi/4:
                    self.N += 1
                    self.theta1 = theta

            # To reset the counter
            r = math.sqrt((landmarks[12][0] - landmarks[19][0]) ** 2 + (landmarks[12][1] - landmarks[19][1]) ** 2)
            if r < 20:
                self.N = 0

        return self.N

# BodyThread class with integrated Jumping Jack detection
class BodyThread(threading.Thread):
    data = ""
    dirty = True
    pipe = None
    timeSinceCheckedConnection = 0
    timeSincePostStatistics = 0
    jumping_jack_detector = JumpingJackDetector()

    def run(self):
        mp_drawing = mp.solutions.drawing_utils
        mp_pose = mp.solutions.pose
        
        capture = CaptureThread()
        capture.start()

        with mp_pose.Pose(min_detection_confidence=0.80, min_tracking_confidence=0.5, model_complexity=global_vars.MODEL_COMPLEXITY, static_image_mode=False, enable_segmentation=True) as pose: 
            
            while not global_vars.KILL_THREADS and capture.isRunning == False:
                print("Waiting for camera and capture thread.")
                time.sleep(0.5)
            print("Beginning capture")
                
            while not global_vars.KILL_THREADS and capture.cap.isOpened():
                ti = time.time()

                # Fetch stuff from the capture thread
                ret = capture.ret
                image = capture.frame
                                
                # Image transformations and stuff
                image = cv2.flip(image, 1)
                image.flags.writeable = global_vars.DEBUG
                
                # Detections
                results = pose.process(image)
                tf = time.time()
                
                # Rendering results
                if global_vars.DEBUG:
                    if time.time()-self.timeSincePostStatistics >= 1:
                        print("Theoretical Maximum FPS: %f" % (1 / (tf - ti)))
                        self.timeSincePostStatistics = time.time()
                        
                    if results.pose_landmarks:
                        mp_drawing.draw_landmarks(image, results.pose_landmarks, mp_pose.POSE_CONNECTIONS, 
                                                mp_drawing.DrawingSpec(color=(255, 100, 0), thickness=2, circle_radius=4),
                                                mp_drawing.DrawingSpec(color=(255, 255, 255), thickness=2, circle_radius=2),
                                                )
                    cv2.imshow('Body Tracking', image)
                    cv2.waitKey(3)

                if self.pipe is None and time.time() - self.timeSinceCheckedConnection >= 1:
                    try:
                        self.pipe = open(r'\\.\pipe\UnityMediaPipeBody', 'r+b', 0)
                    except FileNotFoundError:
                        print("Waiting for Unity project to run...")
                        self.pipe = None
                    self.timeSinceCheckedConnection = time.time()
                    
                if self.pipe is not None:
                    # Set up data for piping
                    self.data = ""
                    i = 0
                    if results.pose_world_landmarks:
                        hand_world_landmarks = results.pose_world_landmarks
                        for i in range(0, 33):
                            self.data += "{}|{}|{}|{}\n".format(i, hand_world_landmarks.landmark[i].x, hand_world_landmarks.landmark[i].y, hand_world_landmarks.landmark[i].z)
                    
                    # Get Jumping Jack count
                    jumping_jack_count = self.jumping_jack_detector.update(results.pose_landmarks.landmark)
                    self.data += "JumpingJackCount|{}\n".format(jumping_jack_count)

                    s = self.data.encode('utf-8') 
                    try:     
                        self.pipe.write(struct.pack('I', len(s)) + s)   
                        self.pipe.seek(0)    
                    except Exception as ex:  
                        print("Failed to write to pipe. Is the unity project open?")
                        self.pipe = None
                        
                #time.sleep(1/20)
                        
        self.pipe.close()
        capture.cap.release()
        cv2.destroyAllWindows()

# Rest of your code remains unchanged

