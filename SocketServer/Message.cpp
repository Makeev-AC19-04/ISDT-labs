#include "pch.h"
#include "Message.h"

string GetLastErrorString(DWORD ErrorID = 0)
{
	if (!ErrorID)
		ErrorID = GetLastError();
	if (!ErrorID)
		return string();

	LPSTR pBuff = nullptr;
	size_t size = FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
		NULL, ErrorID, MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), (LPSTR)&pBuff, 0, NULL);
	string s(pBuff, size);
	LocalFree(pBuff);

	return s;
}

int Message::clientID = 0;

void Message::send(CSocket& s, int to, int from, int type, const string& data)
{
	Message m(to, from, type, data);
	m.send(s);
	//Message ms(MR_SUPSERVER, from, MT_DATA, to_string(to) + ";" + to_string(from) + ";" + data);
	//cout << to_string(to) + ";" + to_string(from) + ";" + data << endl;
	//ms.send(s);
}

//Message Message::send()

Message Message::send(int to, int type, const string& data)
{
	CSocket s;
	s.Create();
	if (!s.Connect("127.0.0.1", 12435))
	{
		throw runtime_error(GetLastErrorString());
	}
	Message m(to, clientID, type, data);
	m.send(s);
	//Message ms(MR_SUPSERVER, clientID, type, to_string(to) + ";" + to_string(clientID) + ";" + data);
	//cout << to_string(to) + ";" + to_string(clientID) + ";" + data << endl;
	//ms.send(s);
	if (m.receive(s) == MT_INIT)
	{
		clientID = m.header.to;
		cout << "clientID is " + to_string(clientID) + "\n";
	}
	//else if (m.receive(s) == MT_INIT_SUPSERVER)
	//{
	//	clientID = m.header.to;
	//	cout << "SupServer clientID is " + to_string(clientID) + "\n";
	//}
	return m;
}

