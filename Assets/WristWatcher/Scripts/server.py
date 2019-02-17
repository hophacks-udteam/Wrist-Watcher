#
#   Hello World server in Python
#   Binds REP socket to tcp://*:5555
#   Expects b"Hello" from client, replies with b"World"
#

import time
import sys
import zmq
import numpy as np
import pickle
import sklearn
from sklearn.ensemble import RandomForestClassifier


pickle_inL = open("..//Resources//left_classifier.pickle",'rb')
pickle_inR = open("..//Resources//right_classifier.pickle",'rb')
clfL = pickle.load(pickle_inL,encoding = 'latin-1')
clfR = pickle.load(pickle_inR,encoding = 'latin-1')

context = zmq.Context()
socket = context.socket(zmq.REP)
socket.bind("tcp://*:5555")

print("Server starting")
while True:
    try:
        #  Wait for next request from client
        message = socket.recv()
        #print("Received request: %s" % message)
        message = str(message,'utf-8')
        data = [float(x) for x in message.split(',')]
        #print(data)
        leftHand=[data[0],data[2],data[3],data[6]]
        rightHand=[data[1],data[4],data[5],data[7]]
        data[9] = clfL.predict([leftHand])[0]
        data[10] = clfR.predict([rightHand])[0]
        
		#modify data as needed
        #print(leftHand)
        #print(rightHand)
        data = ",".join([str(x) for x in data])
        #print(data)
        socket.send_string(data)
    except KeyboardInterrupt:
        break
