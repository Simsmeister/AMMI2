# MediaPipe Body
import socket
import mediapipe as mp
from mediapipe.tasks import python
from mediapipe.tasks.python import vision
import numpy as np

import cv2
import threading
import time
import global_vars 
import struct
from utils import *
#from utils import score_table
import mediapipe as mp
from types_of_exercise import TypeOfExercise
#from program import counter

# the capture thread captures images from the WebCam on a separate thread (for performance)
class CaptureThread(threading.Thread):
    cap = None
    ret = None
    frame = None
    isRunning = False
    counter = 0
    timer = 0.0
    def run(self):
        self.cap = cv2.VideoCapture(global_vars.WEBCAM_INDEX) # sometimes it can take a while for certain video captures 4
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

# the body thread actually does the 
# processing of the captured images, and communication with unity
class BodyThread(threading.Thread):
    data = ""
    dirty = True
    pipe = None
    timeSinceCheckedConnection = 0
    timeSincePostStatistics = 0


    def compute_real_world_landmarks(self,world_landmarks,image_landmarks,image_shape):
        try:
            # pseudo camera internals
            # if you properly calibrated your camera tracking quality can improve...
            frame_height,frame_width, channels = image_shape
            focal_length = frame_width*.6
            center = (frame_width/2, frame_height/2)
            camera_matrix = np.array(
                                    [[focal_length, 0, center[0]],
                                    [0, focal_length, center[1]],
                                    [0, 0, 1]], dtype = "double"
                                    )
            distortion = np.zeros((4, 1))

            success, rotation_vector, translation_vector = cv2.solvePnP(objectPoints= world_landmarks, 
                                                                        imagePoints= image_landmarks, 
                                                                        cameraMatrix= camera_matrix, 
                                                                        distCoeffs= distortion,
                                                                        flags=cv2.SOLVEPNP_SQPNP)
            transformation = np.eye(4)
            transformation[0:3, 3] = translation_vector.squeeze()

            # transform model coordinates into homogeneous coordinates
            model_points_hom = np.concatenate((world_landmarks, np.ones((33, 1))), axis=1)

            # apply the transformation
            world_points = model_points_hom.dot(np.linalg.inv(transformation).T)

            return world_points
        except AttributeError:
            print("Attribute Error: shouldn't happen frequently")
            return world_landmarks 

    def run(self):
        # exercises = {
        # 1: "Skipping",
        # 2: "sit-up",
        # 3: "walk",
        # 4: "squat",
        # 5: "pull-up",
        # 6: "push-up"
        # }

        # for number, exercise in exercises.items():
        #     print(f"{number}. {exercise}")

        # user_input = input("Pick an exercise by typing the corresponding number: ")

        # picker = str(exercises[int(user_input)])
        mp_drawing = mp.solutions.drawing_utils
        mp_pose = mp.solutions.pose
        
        capture = CaptureThread()
        capture.start()


        # Connect to Unity Server
        self.unity_client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        try:
            self.unity_client_socket.connect(('127.0.0.1', 13000))
            print("Connected to Unity")
        except Exception as ex:
            print("Failed to connect to Unity:", ex)

        with mp_pose.Pose(min_detection_confidence=0.80, min_tracking_confidence=0.5, model_complexity = global_vars.MODEL_COMPLEXITY,static_image_mode = False,enable_segmentation = True) as pose: 
            counter_e = 0  # movement of exercise
            status = True  # state of move

            while not global_vars.KILL_THREADS and capture.isRunning==False:
                print("Waiting for camera and capture thread.")
                time.sleep(0.5)
            print("Beginning capture")
                
            while not global_vars.KILL_THREADS and capture.cap.isOpened():
                ti = time.time()

                # Fetch stuff from the capture thread
                ret = capture.ret
                image = capture.frame
                try:
                    landmarks = results.pose_landmarks.landmark
                except:
                    pass

                try:
                    data_from_unity = self.unity_client_socket.recv(1024).decode('utf-8')
                    if data_from_unity:
                        print("Received data from Unity:", data_from_unity)
                        counter_e, status = TypeOfExercise(landmarks).calculate_exercise(
                            data_from_unity, counter_e, status)


                except Exception as ex:
                    print("Error while receiving data from Unity:", ex)
                    pass

                image.flags.writeable = global_vars.DEBUG
                
                # Detections
                results = pose.process(image)
                tf = time.time()
                
                # Rendering results
                if global_vars.DEBUG:
                    if time.time()-self.timeSincePostStatistics>=1:
                        print("Theoretical Maximum FPS: %f"%(1/(tf-ti)))
                        self.timeSincePostStatistics = time.time()
                        
                    if results.pose_landmarks:
                        mp_drawing.draw_landmarks(image, results.pose_landmarks, mp_pose.POSE_CONNECTIONS, 
                                                mp_drawing.DrawingSpec(color=(255, 100, 0), thickness=2, circle_radius=4),
                                                mp_drawing.DrawingSpec(color=(255, 255, 255), thickness=2, circle_radius=2),
                                                )
                    #cv2.imshow('Body Tracking', image)
                    #cv2.waitKey(1)

                
                    
                    if results.pose_world_landmarks:
                        image_landmarks = results.pose_landmarks
                        world_landmarks = results.pose_world_landmarks

                        model_points = np.float32([[-l.x, -l.y, -l.z] for l in world_landmarks.landmark])
                        image_points = np.float32([[l.x * image.shape[1], l.y * image.shape[0]] for l in image_landmarks.landmark])
                        
                        body_world_landmarks_world = self.compute_real_world_landmarks(model_points,image_points,image.shape)
                        body_world_landmarks = results.pose_world_landmarks
                        
                        for i in range(0,33):
                            self.data += "FREE|{}|{}|{}|{}\n".format(i,body_world_landmarks_world[i][0],body_world_landmarks_world[i][1],body_world_landmarks_world[i][2])
                        for i in range(0,33):
                            self.data += "ANCHORED|{}|{}|{}|{}\n".format(i,-body_world_landmarks.landmark[i].x,-body_world_landmarks.landmark[i].y,-body_world_landmarks.landmark[i].z)

                self.unity_Counter = str(counter_e)  #Send this to unity also

                if self.unity_client_socket:
                    self.data = ""
                    i = 0

                    if results.pose_world_landmarks:


                        for i in range(0, 33):
                            self.data += "FREE|{}|{}|{}|{}\n".format(i, body_world_landmarks_world[i][0],
                                                                    body_world_landmarks_world[i][1],
                                                                    body_world_landmarks_world[i][2])
                        for i in range(0, 33):
                            self.data += "ANCHORED|{}|{}|{}|{}\n".format(i, -body_world_landmarks.landmark[i].x,
                                                                        -body_world_landmarks.landmark[i].y,
                                                                        -body_world_landmarks.landmark[i].z)
                            
                        
                    self.data += "UNITY_COUNTER|{}\n".format(self.unity_Counter)

                    s = self.data.encode('utf-8')
                    try:
                        self.unity_client_socket.sendall(struct.pack('I', len(s)) + s)
                    except Exception as ex:
                        print("Failed to send data to Unity:", ex)
                        self.unity_client_socket.close()
                        self.unity_client_socket = None




                        
                        
        self.unity_client_socket.close()
        capture.cap.release()
        cv2.destroyAllWindows()