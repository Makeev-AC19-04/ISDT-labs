#pragma once

enum MessageTypes
{
	MT_INIT,
	MT_EXIT,
	MT_GETDATA,
	MT_DATA,
	MT_NODATA,
	MT_CONFIRM,
	MT_LONGWAITING,
	MT_INIT_SUPSERVER,
	MT_LAST_MESSAGES,
	MT_HISTORY
};

enum MessageRecipients
{
	MR_BROKER = 10,
	MR_ALL = 50,
	MR_USER = 100,
	MR_SUPSERVER = 300
};

struct MessageHeader
{
	int to;
	int from;
	int type;
	int size;
};

class Message
{
public:
	MessageHeader header = {0};
	string data;
	static int clientID;

	Message() {}
	Message(int to, int from, int type = MT_DATA, const string& data = "")
	{
		this->data = data;
		header = {to, from, type, int(data.length())};
	}
	
	void send(CSocket& s)
	{
		s.Send(&header, sizeof(MessageHeader));
		if (header.size)
		{
			s.Send(data.c_str(), (int)header.size);
		}
	}

	int receive(CSocket& s)
	{
		if (!s.Receive(&header, sizeof(MessageHeader)))
		{
			return MT_NODATA;
		}
		if (header.size)
		{
			vector <char> v(header.size);
			s.Receive(&v[0], (int)header.size);
			data = string(&v[0], header.size);
		}
		return header.type;
	}

	static void send(CSocket& s, int to, int from, int type = MT_DATA, const string& data = "");
	static Message send(int to, int type = MT_DATA, const string& data = "");
};

