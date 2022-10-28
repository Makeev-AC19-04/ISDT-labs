from http.server import HTTPServer, CGIHTTPRequestHandler
import threading

def ProcessServer():
    server_address = ("", 8000)
    print("Web interface started")
    HTTPServer(server_address, CGIHTTPRequestHandler).serve_forever()

w = threading.Thread(target=ProcessServer)
w.start()
#ProcessServer()