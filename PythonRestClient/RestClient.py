import cgi
import threading
from dataclasses import dataclass
import socket, struct, time
from http.server import HTTPServer, BaseHTTPRequestHandler
from msg import *
import requests

clientId = 0
clientmsg = ''

def ProcessMessages():
    global clientId
    while True:
        #m = Message.SendMessage(MR_BROKER, MT_GETDATA)
        #m = requests.get("http://localhost:8989", data={})
        #clientId = m.Header.To
        a = SendRequest({'to':MR_BROKER, 'from':clientId, 'type': MT_GETDATA, 'data':''})
        if int(a['type']) == MT_DATA:
            print("New message: " + a['data'] + "\nFrom: " + a['from'])
        else:
            time.sleep(1)


def SendRequest(params):
    URL = "http://localhost:8989"
    r = requests.get(url=URL, json=params)
    return r.json()

def GetHistory():
    global clientId
    a = SendRequest({'to': MR_BROKER, 'from': clientId, 'type': MT_LAST_MESSAGES, 'data': ""})

def SendInit():
    global clientId
    id = SendRequest({'to':MR_BROKER, 'from':'', 'type': MT_INIT, 'data':''})
    clientId = int(id['to'])
    print("Client's id is " + str(clientId))

def Send(id, message):
    global clientId
    a = SendRequest({'to':id, 'from':clientId, 'type': MT_DATA, 'data':message})

def SendAll(message):
    global clientId
    a = SendRequest({'to': MR_ALL, 'from': clientId, 'type': MT_DATA, 'data': message})

def SendExit():
    global clientId
    a = SendRequest({'to': MR_BROKER, 'from': clientId, 'type': MT_EXIT, 'data': ''})

def Client():
        global clientId
        print("Python client has started\n")
        SendInit()
        GetHistory()
        w = threading.Thread(target=ProcessMessages)
        w.start()
        while True:
            print("Menu:\n1.Send to all\n2.Send to one\n3. Exit")
            menu = int(input())
            if menu == 1:
                print("Enter your message")
                message = input()
                SendAll(message)
                #Message.SendMessage(MR_ALL, MT_DATA, message)
            elif menu == 2:
                print("Enter client's id")
                id = int(input())
                print("Enter your message")
                message = input()
                Send(id, message)
                #Message.SendMessage(id, MT_DATA, message)
            elif menu == 3:
                SendExit()
                #Message.SendMessage(MR_BROKER, MT_EXIT)
                quit()
                break

Client()
