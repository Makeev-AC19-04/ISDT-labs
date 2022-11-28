#pragma once
#include <ctime>
//#include "crow.h"


class Session
{
public:
	int id;
	string name;
	queue<Message> messages;
	std::chrono::high_resolution_clock::time_point lastInteraction;

	CCriticalSection cs;
	Session(int _id, string _name, std::chrono::high_resolution_clock::time_point _lastInteracion)
		:id(_id), name(_name), lastInteraction(_lastInteracion)
	{
	}

	void add(Message& m)
	{
		CSingleLock lock(&cs, TRUE);
		messages.push(m);
	}

	void send(CSocket& s)
	{
		CSingleLock lock(&cs, TRUE);
		if (messages.empty())
		{
			Message::send(s, id, MR_BROKER, MT_NODATA);
		}
		else
		{
			messages.front().send(s);
			messages.pop();
		}
	}

	//crow::json::wvalue SendResponse()
	//{
	//	CSingleLock lock(&cs, TRUE);
	//	if (messages.empty())
	//	{
	//		//Message::send(s, id, MR_BROKER, MT_NODATA);
	//		crow::json::wvalue response({
	//		{"from", MR_BROKER},
	//		{"type", MT_NODATA}
	//			});
	//		return response;
	//	}
	//	else
	//	{
	//		crow::json::wvalue response({
	//		{"from", messages.front().clientID},
	//		{"type", messages.front().header.type},
	//		{"data", messages.front().data},
	//			});
	//		messages.pop();
	//		return response;
	//	}
	//	
	//}
};

