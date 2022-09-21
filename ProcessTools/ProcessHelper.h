#pragma once
#include <string>

#ifndef UNICODE
#define UNICODE
#endif
#ifndef _UNICODE
#define _UNICODE
#endif

#define IN     const&
#define OUT    &
#define REF    &

#define SETVALUEFROMPOINTER(p, v) (*p=v)
#if defined(UNICODE) || defined(_UNICODE)
#define OutPutStr(f, v) wprintf_s(L##f, v)
#else
#define OutPutStr(f, v) printf_s(f, v)
#endif

namespace ProcessTools {
	struct PROCESSINFO
	{
		ULONG_PTR CurrentProcessID;
		ULONG_PTR ParentProcessID;
		TCHAR lpszBuffer_Parent_Name[BUFSIZ];
		DWORD ErrCodeForBuffer;
	};

	public ref class ProcessHelper
	{
	private:

	public:
		ProcessHelper() { }

		/// <summary>
		/// 获取当前进程、父进程的PID
		/// </summary>
		void GetProcessPID(PROCESSINFO processInfo);

		/// <summary>
		/// 获取当前进程、父进程的PID
		/// </summary>
		/// <param name="CurrentProcessID"></param>
		/// <param name="ParentProcessID"></param>
		void GetProcessPID(PULONG_PTR CurrentProcessID, PULONG_PTR ParentProcessID);

		/// <summary>
		/// 获取当前进程、父进程的PID以及名称
		/// </summary>
		/// <param name="processInfo"></param>	
		void GetProcessPIDAndName(PROCESSINFO& processInfo);

		/// <summary>
		/// 获取当前进程、父进程的PID以及名称
		/// </summary>
		/// <param name="CurrentProcessID"></param>
		/// <param name="ParentProcessID"></param>
		/// <param name="lpszBuffer_Parent_Name"></param>	
		/// <param name="ErrCodeForBuffer"></param>	
		void GetProcessPIDAndName(PULONG_PTR CurrentProcessID, PULONG_PTR ParentProcessID, std::wstring& lpszBuffer_Parent_Name, PDWORD errCodeForBuffer);
		
		void GetProcessPIDAndName(PULONG_PTR CurrentProcessID, PULONG_PTR ParentProcessID, LPTSTR lpszBuffer_Parent_Name, PDWORD ErrCodeForBuffer);
	};
}

