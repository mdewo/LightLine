using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public delegate void Callback();
public delegate void Callback<T>(T arg1);
public delegate void Callback<T, U>(T arg1, U arg2);

static public class EventManager
{
	private static Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();

	static public void AddListener(string eventType, Callback handler)
	{
		lock (eventTable)
		{
			if (!eventTable.ContainsKey(eventType))
			{
				eventTable.Add(eventType, null);
			}
			eventTable[eventType] = (Callback)eventTable[eventType] + handler;
		}
	}

	static public void RemoveListener(string eventType, Callback handler)
	{
		lock (eventTable)
		{
			if (eventTable.ContainsKey(eventType))
			{
				eventTable[eventType] = (Callback)eventTable[eventType] - handler;

				if (eventTable[eventType] == null)
				{
					eventTable.Remove(eventType);
				}
			}
		}
	}

	static public void Invoke(string eventType)
	{
		Delegate d;
		if (eventTable.TryGetValue(eventType, out d))
		{
			Callback callback = (Callback)d;

			if (callback != null)
			{
				callback();
			}
		}
	}
}

static public class EventManager<T>
{
	private static Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();

	static public void AddListener(string eventType, Callback<T> handler)
	{
		lock (eventTable)
		{
			if (!eventTable.ContainsKey(eventType))
			{
				eventTable.Add(eventType, null);
			}
			eventTable[eventType] = (Callback<T>)eventTable[eventType] + handler;
		}
	}

	static public void RemoveListener(string eventType, Callback<T> handler)
	{
		lock (eventTable)
		{
			if (eventTable.ContainsKey(eventType))
			{
				eventTable[eventType] = (Callback<T>)eventTable[eventType] - handler;

				if (eventTable[eventType] == null)
				{
					eventTable.Remove(eventType);
				}
			}
		}
	}

	static public void Invoke(string eventType, T arg1)
	{
		Delegate d;
		if (eventTable.TryGetValue(eventType, out d))
		{
			Callback<T> callback = (Callback<T>)d;

			if (callback != null)
			{
				callback(arg1);
			}
		}
	}
}

static public class EventManager<T, U>
{
	private static Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();

	static public void AddListener(string eventType, Callback<T, U> handler)
	{
		lock (eventTable)
		{
			if (!eventTable.ContainsKey(eventType))
			{
				eventTable.Add(eventType, null);
			}
			eventTable[eventType] = (Callback<T, U>)eventTable[eventType] + handler;
		}
	}

	static public void RemoveListener(string eventType, Callback<T, U> handler)
	{
		lock (eventTable)
		{
			if (eventTable.ContainsKey(eventType))
			{
				eventTable[eventType] = (Callback<T, U>)eventTable[eventType] - handler;

				if (eventTable[eventType] == null)
				{
					eventTable.Remove(eventType);
				}
			}
		}
	}

	static public void Invoke(string eventType, T arg1, U arg2)
	{
		Delegate d;
		if (eventTable.TryGetValue(eventType, out d))
		{
			Callback<T, U> callback = (Callback<T, U>)d;

			if (callback != null)
			{
				callback(arg1, arg2);
			}
		}
	}
}