#define useWriteFileLog
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
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
	public static string OutStr
	{
		get { return outStr; }
	}
	private static int lineCount = 0;
	private static bool m_bIsInit = false;
	private static DateTime m_StartTime = DateTime.Now;
	private static bool UseColorMode = false;

	private static ColorEnum[] LogColorOrigin = new ColorEnum[]
	{
		ColorEnum.silver,
		ColorEnum.yellow,
		ColorEnum.red,
	};
	private static ColorEnum[] LogColor = new ColorEnum[]
	{
		ColorEnum.silver,
		ColorEnum.yellow,
		ColorEnum.red,
	};
	public void Awake()
	{
		//m_StartTime = DateTime.Now;
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
	public static bool IsEnable
	{
		get { return m_bIsEnable; }
	}
		

	public static void SetEnableState(bool isEnable,bool isUseColorMode = false)
	{
		m_bIsEnable = isEnable;
		UseColorMode = isUseColorMode;
	}
	public static void SetLogTypeColor(LogType type,ColorEnum clr)
	{
		if(type < LogType.Num)
		{
			LogColor[(int)type] = clr;
		}
	}
	public static void SetShowType(LogType type, bool isShow)
	{
		CheckShowByLogType[(int)type] = isShow;
	}

	public static double GetNowTimeFromStart()
	{
		return (DateTime.Now - m_StartTime).TotalMilliseconds;
	}
	public static string GetNowTimeFromStartString()
	{
		TimeSpan timeSpan = DateTime.Now - m_StartTime;
		return string.Format("{0}:{1}:{2}:{3}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Ticks);
	}
	/// <summary>
	/// GB2312转换成UTF8
	/// </summary>
	/// <param name="text"></param>
	/// <returns></returns>
	static string gb2312_utf8(string text)
	{
		//声明字符集   
		System.Text.Encoding utf8, gb2312;
		//gb2312   
		gb2312 = System.Text.Encoding.GetEncoding("gb2312");
		//utf8   
		utf8 = System.Text.Encoding.GetEncoding("utf-8");
		byte[] gb;
		gb = gb2312.GetBytes(text);
		gb = System.Text.Encoding.Convert(gb2312, utf8, gb);
		//返回转换后的字符   
		return utf8.GetString(gb);
	}

	/// <summary>
	/// UTF8转换成GB2312
	/// </summary>
	/// <param name="text"></param>
	/// <returns></returns>
	static string utf8_gb2312(string text)
	{
		//声明字符集   
		System.Text.Encoding utf8, gb2312;
		//utf8   
		utf8 = System.Text.Encoding.GetEncoding("utf-8");
		//gb2312   
		gb2312 = System.Text.Encoding.GetEncoding("gb2312");
		byte[] utf;
		utf = utf8.GetBytes(text);
		utf = System.Text.Encoding.Convert(utf8, gb2312, utf);
		//返回转换后的字符   
		return gb2312.GetString(utf);
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
				{
					if (UseColorMode)
					{
						UnityEngine.Debug.Log(str.AddColorRichText(LogColor[(int)type]));
					}
					else
					{
					//System.Text.Encoding.Default.GetString();
						UnityEngine.Debug.Log(str);
					}
				}
				else if (type == LogType.Warning)
				{
					if (UseColorMode)
					{
						UnityEngine.Debug.LogWarning(str.AddColorRichText(LogColor[(int)type]));
					}
					else
					{
						UnityEngine.Debug.LogWarning(str);
					}
				}
				else if (type == LogType.Error)
				{
					if (UseColorMode)
					{
						UnityEngine.Debug.LogError(str.AddColorRichText(LogColor[(int)type]));
					}
					else
					{
						UnityEngine.Debug.LogError(str);
					}
				}
#if useWriteFileLog
				//FileLogDebug.WriteLog(FLTEnum.SysLog, "[{0}:{2}]{1}", type.ToString(), str, GetNowTimeFromStart());
				FileLogDebug.WriteLog(FLTEnum.SysLog, "[{0}:{2}]{1}", type.ToString(), str, DateTime.Now);
#endif
		}

	}
	static void ClearStr()
	{
		if (lineCount > m_maxLineCount)
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


	public static string Bytes2HexStr(byte[] bytes)
	{
		string rec_str = "";
		for (int i = 0; i < bytes.Length; ++i)
		{
			rec_str += " " + String.Format("0x{0:X2},", bytes[i]);
			if (i % 16 == 15)
				rec_str += "\n";
		}
		return rec_str;
	}

	public static string Bytes2DecStr(byte[] bytes)
	{
		string rec_str = "";
		for (int i = 0; i < bytes.Length; ++i)
		{
			rec_str += " " + String.Format("{0},", bytes[i]);
			if (i % 16 == 15)
				rec_str += "\n";
		}
		return rec_str;
	}

	public static string String2HexStr(string str)
	{
		return Bytes2HexStr(System.Text.Encoding.ASCII.GetBytes(str));
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
	public void SetShowMsgOnScreen(bool isShow)
	{
		isShowMessageOnScreen = isShow;
	}
	public void OnGUI()
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
	private static int m_maxLineCount = 10;
	public static int MaxLineCount
	{
		get { return m_maxLineCount; }
		set { m_maxLineCount = value; }
	}
	public void SetMaxLineCount(int max)
	{
		m_maxLineCount = max;
	}

	static public void Assert(bool _bCondition)
	{
		if (!_bCondition)
		{
			throw new Exception();
		}
	}
	public static void LogFormat(string format, params object[] args)
	{
		DebugLog.Log(format,args);
	}
	public static void LogFormat(ColorEnum clr,string format, params object[] args)
	{
		SetLogTypeColor(LogType.Normal, clr);
		DebugLog.Log(format, args);
		SetLogTypeColor(LogType.Normal, LogColorOrigin[(int)LogType.Normal]);
	}

	public static void LogWarningFormat(string format, params object[] args)
	{
		DebugLog.LogW(format, args);
	}
	public static void LogWarningFormat(ColorEnum clr, string format, params object[] args)
	{
		SetLogTypeColor(LogType.Warning, clr);
		DebugLog.LogW(format, args);
		SetLogTypeColor(LogType.Warning, LogColorOrigin[(int)LogType.Warning]);
	}

	public static void LogErrorFormat(string format, params object[] args)
	{
		DebugLog.LogE(format, args);
	}
	public static void LogErrorFormat(ColorEnum clr, string format, params object[] args)
	{
		SetLogTypeColor(LogType.Error, clr);
		DebugLog.LogE(format, args);
		SetLogTypeColor(LogType.Error, LogColorOrigin[(int)LogType.Error]);
	}



}
static public class AddColorRichTextClass
{
	static public string AddColorRichText(this string strIn, Color color)
	{
		return string.Format("<color=#{0:x2}{1:x2}{2:x2}{3:x2}>{4}</color>", (int)(color.r * 255), (int)(color.g * 255), (int)(color.b * 255), (int)(color.a * 255), strIn);
	}
	static public string AddColorRichText(this string strIn, ColorEnum color)
	{
		return string.Format("<color={0}>{1}</color>", color.ToString(), strIn);
	}

}
public enum ColorEnum
{
	aqua,
	black,
	blue,
	brown,
	cyan,
	darkblue,
	fuchsia,
	green,
	grey,
	lightblue,
	lime,
	magenta,
	maroon,
	navy,
	olive,
	orange,
	purple,
	red,
	silver,
	teal,
	white,
	yellow,
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
	//static System.Text.Encoding TheDefaultEncoding = System.Text.Encoding.GetEncoding("gb2312");
	static System.Text.Encoding TheDefaultEncoding = System.Text.Encoding.Default;
	static public void SetDefaultEncoding(Encoding encoding)
	{
		TheDefaultEncoding = encoding;
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

			byte[] buff_default = TheDefaultEncoding.GetBytes(value);
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


///// <summary> 
///// FileEncoding 的摘要说明 
///// </summary> 
//namespace FileEncoding
//{
//	/// <summary> 
//	/// 获取文件的编码格式 
//	/// </summary> 
//	public class EncodingType
//	{
//		/// <summary> 
//		/// 给定文件的路径，读取文件的二进制数据，判断文件的编码类型 
//		/// </summary> 
//		/// <param name=“FILE_NAME“>文件路径</param> 
//		/// <returns>文件的编码类型</returns> 
//		public static Encoding GetType(string FILE_NAME)
//		{
//			FileStream fs = new FileStream(FILE_NAME, FileMode.Open, FileAccess.Read);
//			Encoding r = GetType(fs);
//			fs.Close();
//			return r;
//		}

//		/// <summary> 
//		/// 通过给定的文件流，判断文件的编码类型 
//		/// </summary> 
//		/// <param name=“fs“>文件流</param> 
//		/// <returns>文件的编码类型</returns> 
//		public static Encoding GetType(FileStream fs)
//		{
//			byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
//			byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
//			byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM 
//			Encoding reVal = Encoding.Default;

//			BinaryReader r = new BinaryReader(fs, System.Text.Encoding.Default);
//			int i;
//			int.TryParse(fs.Length.ToString(), out i);
//			byte[] ss = r.ReadBytes(i);
//			if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
//			{
//				reVal = Encoding.UTF8;
//			}
//			else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
//			{
//				reVal = Encoding.BigEndianUnicode;
//			}
//			else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
//			{
//				reVal = Encoding.Unicode;
//			}
//			r.Close();
//			return reVal;

//		}
//		public static Encoding GetType(byte[] data)
//		{
//			Encoding reVal = Encoding.Default;
//			if (IsUTF8Bytes(data) || (data[0] == 0xEF && data[1] == 0xBB && data[2] == 0xBF))
//			{
//				reVal = Encoding.UTF8;
//			}
//			else if (data[0] == 0xFE && data[1] == 0xFF && data[2] == 0x00)
//			{
//				reVal = Encoding.BigEndianUnicode;
//			}
//			else if (data[0] == 0xFF && data[1] == 0xFE && data[2] == 0x41)
//			{
//				reVal = Encoding.Unicode;
//			}
//			return reVal;
//		}

//		/// <summary> 
//		/// 判断是否是不带 BOM 的 UTF8 格式 
//		/// </summary> 
//		/// <param name=“data“></param> 
//		/// <returns></returns> 
//		private static bool IsUTF8Bytes(byte[] data)
//		{
//			int charByteCounter = 1; //计算当前正分析的字符应还有的字节数 
//			byte curByte; //当前分析的字节. 
//			for (int i = 0; i < data.Length; i++)
//			{
//				curByte = data[i];
//				if (charByteCounter == 1)
//				{
//					if (curByte >= 0x80)
//					{
//						//判断当前 
//						while (((curByte <<= 1) & 0x80) != 0)
//						{
//							charByteCounter++;
//						}
//						//标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X 
//						if (charByteCounter == 1 || charByteCounter > 6)
//						{
//							return false;
//						}
//					}
//				}
//				else
//				{
//					//若是UTF-8 此时第一位必须为1 
//					if ((curByte & 0xC0) != 0x80)
//					{
//						return false;
//					}
//					charByteCounter--;
//				}
//			}
//			if (charByteCounter > 1)
//			{
//				throw new Exception("非预期的byte格式");
//			}
//			return true;
//		}

//	}


//}