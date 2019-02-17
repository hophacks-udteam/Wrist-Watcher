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


pickle_in = open("..//Resources//finalized_model.sav",'rb')
clf = pickle.load(pickle_in,encoding = 'latin-1')

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
        data[6] = clf.predict([data[0:6]])[0]
		
		#modify data as needed
        
        data = ",".join([str(x) for x in data])
        #print(data)
        socket.send_string(data)
    except KeyboardInterrupt:
        break
