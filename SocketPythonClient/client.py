import threading
from dataclasses import dataclass
import socket, struct, time
from msg import *


def ProcessMessages():
    while True:
        m = Message.SendMessage(MR_BROKER, MT_GETDATA)
        if m.Header.Type == MT_DATA:
            print("New message: " + m.Data + "\nFrom: " + str(m.Header.From))
        else:
            time.sleep(1)


def Client():
        print("Python client has started\n")
        Message.SendMessage(MR_BROKER, MT_INIT)
        t = threading.Thread(target=ProcessMessages)
        t.start()
        while True:
                print("Menu:\n1.Send to all\n2.Send to one\n3. Exit")
                menu = int(input())
                if menu == 1:
                    print("Enter your message")
                    message = input()
                    Message.SendMessage(MR_ALL, MT_DATA, message)
                elif menu == 2:
                    print("Enter client's id")
                    id = int(input())
                    print("Enter your message")
                    message = input()
                    Message.SendMessage(id, MT_DATA, message)
                elif menu == 3:
                    Message.SendMessage(MR_BROKER, MT_EXIT)
                    quit()
                    break


Client()
