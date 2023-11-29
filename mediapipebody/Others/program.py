## import packages
import sys
import cv2
from utils import *
#from utils import score_table
import mediapipe as mp
from types_of_exercise import TypeOfExercise


## drawing body
mp_drawing = mp.solutions.drawing_utils
mp_pose = mp.solutions.pose

## setting the video source
cap = cv2.VideoCapture(0)  # webcam

exercises = {
    1: "Skipping",
    2: "sit-up",
    3: "walk",
    4: "squat",
    5: "pull-up",
    6: "push-up"
}

for number, exercise in exercises.items():
    print(f"{number}. {exercise}")

user_input = input("Pick an exercise by typing the corresponding number: ")

picker = str(exercises[int(user_input)])

cap.set(3, 800)  # width
cap.set(4, 480)  # height



## setup mediapipe
with mp_pose.Pose(min_detection_confidence=0.5,
                  min_tracking_confidence=0.5) as pose:

    counter = 0  # movement of exercise
    status = True  # state of move
    while cap.isOpened():
        ret, frame = cap.read()

        frame = cv2.resize(frame, (800, 480), interpolation=cv2.INTER_AREA)
        ## recolor frame to RGB
        frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        frame.flags.writeable = False
        ## make detection
        results = pose.process(frame)
        ## recolor back to BGR
        frame.flags.writeable = True
        frame = cv2.cvtColor(frame, cv2.COLOR_RGB2BGR)

        try:
            landmarks = results.pose_landmarks.landmark
            counter, status = TypeOfExercise(landmarks).calculate_exercise(
                picker, counter, status)
        except:
            pass

        #score_table(picker, counter, status)



        ## render detections (for landmarks)
        mp_drawing.draw_landmarks(
            frame,
            results.pose_landmarks,
            mp_pose.POSE_CONNECTIONS,
            mp_drawing.DrawingSpec(color=(255, 255, 255),
                                   thickness=2,
                                   circle_radius=2),
            mp_drawing.DrawingSpec(color=(174, 139, 45),
                                   thickness=2,
                                   circle_radius=2),
        )

        cv2.imshow('Video', frame)
        if cv2.waitKey(10) & 0xFF == ord('q'):
            break


    cap.release()
    cv2.destroyAllWindows()
