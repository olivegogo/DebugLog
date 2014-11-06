#define useWriteFileLog
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEngine;
//-----------------------------------使用方法-----------------------------------//*
/*
 * DebugLog.Log("hello world");
 * DebugLog.Log("herro world:{0},{1},{2:X}","haha",1000,1000);
 * string file_name = "test.txt";
 * FileLogDebug.WriteLog(FLTEnum.SysLog, "file_name:{0}", file_name);
*/
//------------------------------------------------------------------------------//

public class DebugLog : MonoBehaviour
{
	public enum LogType
	{
		Normal,
		Warning,
		Error,
		Num
	}
	private static bool[] CheckShowByLogType = new bool[(int)LogType.Num];
	private static string outStr;
	private static int lineCount = 0;
	private static bool m_bIsInit = false;
	public void Awake()
	{
		Init();
	}
	public static void Init()
	{
		if (!m_bIsInit)
		{
			m_bIsInit = true;
			for (int i = 0; i < CheckShowByLogType.Length; ++i)
			{
				CheckShowByLogType[i] = true;
			}
		}
	}
	private static bool m_bIsEnable = false;
	public static void SetEnableState(bool isEnable)
	{
		m_bIsEnable = isEnable;

	}

	public static void SetShowType(LogType type, bool isShow)
	{
		CheckShowByLogType[(int)type] = isShow;
	}

	static void LogOut(string str, LogType type)
	{
		if (m_bIsEnable)
		{
			Init();

			if (!CheckShowByLogType[(int)type])
				return;
			ClearStr();

			outStr += "<=========" + lineCount + "=========>\n" + str + "\n";
			lineCount++;

			//str = string.Format("[{0:0.00}]{1}", Time.realtimeSinceStartup, str);
				if (type == LogType.Normal)
					UnityEngine.Debug.Log(str);
				else if (type == LogType.Warning)
					UnityEngine.Debug.LogWarning(str);
				else if (type == LogType.Error)
					UnityEngine.Debug.LogError(str);
#if useWriteFileLog
			FileLogDebug.WriteLog(FLTEnum.SysLog, "[{0}:{2}]{1}", type.ToString(), str, Time.realtimeSinceStartup);
#endif
		}

	}
	static void ClearStr()
	{
		if (lineCount > 10)
		{
			outStr = "";
			lineCount = 0;
		}
	}
	public static string Bytes2Hex(byte[] bytes)
	{
		string rec_str = "==>";
		for (int i = 0; i < bytes.Length; ++i)
		{
			rec_str += " " + String.Format("{0:X2}", bytes[i]);
			if (i % 16 == 15)
				rec_str += "\n==>";
		}

		return rec_str;
	}




	public static void Log(string format, params object[] args)
	{
		if (m_bIsEnable)
			LogOut(String.Format(format, args), LogType.Normal);
	}
	public static void LogW(string format, params object[] args)
	{
		if (m_bIsEnable)
			LogOut(String.Format(format, args), LogType.Warning);
	}
	public static void LogE(string format, params object[] args)
	{
		if (m_bIsEnable)
			LogOut(String.Format(format, args), LogType.Error);
	}
	public static void FileLog(FLTEnum type, string format, params object[] args)
	{
		FileLogDebug.WriteLog(type, format, args);
	}
	public static bool isShowMessage;
	public bool isShowMessageOnScreen = false;
	void OnGUI()
	{
		if (m_bIsEnable)
		{
			isShowMessage = isShowMessageOnScreen;
			if (isShowMessageOnScreen == true)
			{
				GUI.color = Color.green;
				GUI.Label(new Rect(0, 30, 400, 700), outStr);
			}

		}
	}
}
public enum FLTEnum
{
	NetLog,
	SysLog,
	Num,
}



public class FileLogDebug
{

	static Dictionary<FLTEnum, FileStream> fsDic = null;


	static FileLogDebug _fileLogDebug = null;
	public static FileLogDebug GetInstance()
	{
		if (_fileLogDebug == null)
			_fileLogDebug = new FileLogDebug();

		isCloseFile = false;

		return _fileLogDebug;
	}

	private static bool m_bIsEditorLog = false;
	private static bool m_bIsBuildLog = false;
	public static void SetIsEditorState(bool bIsEditor,bool isBuildLog)
	{
		m_bIsEditorLog = bIsEditor;
		m_bIsBuildLog = isBuildLog;
	}

	FileLogDebug()
	{
#if useWriteFileLog
		fsDic = new Dictionary<FLTEnum, FileStream>();
#endif
	}

	FileStream OpenLogFile(FLTEnum type)
	{
//#if UNITY_EDITOR
		if (m_bIsBuildLog)
		{
			string logpath = Application.persistentDataPath + "/log";
			string filepath = logpath + "/" + type.ToString() + ".log";

			if (!File.Exists(filepath))
			{
				Directory.CreateDirectory(logpath);
				return new FileStream(filepath, FileMode.Create);
			}
			else
			{
				//string stradd = File.GetLastWriteTime(filepath).ToString("yyyyMMdd-HHmmss");
				//string filepath2 = logpath+"/"+ type.ToString() + "_" + stradd + ".log";
				//File.Copy(filepath, filepath2);

				return new FileStream(filepath, FileMode.Create);
			}

		}		
		else if(m_bIsEditorLog)
		{
			//Application.persistentDataPath
			//Application.dataPath
			string logpath = Application.dataPath + "/../log";
			string filepath = logpath + "/" + type.ToString() + ".log";

			if (!File.Exists(filepath))
			{
				Directory.CreateDirectory(logpath);
				return new FileStream(filepath, FileMode.Create);
			}
			else
			{
				string stradd = File.GetLastWriteTime(filepath).ToString("yyyyMMdd-HHmmss");
				string filepath2 = logpath + "/" + type.ToString() + "_" + stradd + ".log";
				File.Copy(filepath, filepath2);

				return new FileStream(filepath, FileMode.Create);
			}

		}
		 
//#else
//#endif

		return null;
	}

	static public void WriteLog(FLTEnum type, string format, params object[] args)
	{
#if useWriteFileLog
		if (GetInstance() != null)
			GetInstance().WriteLogFile(type, String.Format(format, args));
#endif
	}

	void WriteLogFile(FLTEnum type, string value)
	{
		if (isCloseFile || fsDic == null)
			return;
#if useWriteFileLog

		//System.Console.WriteLine(value);


		if (!fsDic.ContainsKey(type))
		{
			fsDic[type] = OpenLogFile(type);
		}

		if (fsDic[type] != null)
		{
			if (value[value.Length - 1] != '\n')
				value += '\n';

			byte[] buff_default = System.Text.Encoding.Default.GetBytes(value);
			//string ss =  new string(System.Text.Encoding.GetEncoding("utf-8").GetChars(buff_default));

			byte[] info = buff_default;
			//byte[] info = new UTF8Encoding(true).GetBytes(value);
			//byte[] info = new ASCIIEncoding().GetBytes(value);
			fsDic[type].Write(info, 0, info.Length);

			fsDic[type].Flush();

		}
#endif
	}
	private static bool isCloseFile = false;
	static public void CloseLogFiles()
	{
		isCloseFile = true;
#if useWriteFileLog
		//for (int i = 0; i < (int)FLTEnum.Num; ++i)
		//	fsList[i].Close();
		foreach (KeyValuePair<FLTEnum, FileStream> ele in fsDic)
		{
			if (ele.Value != null)
				ele.Value.Close();
		}
#endif
	}

}