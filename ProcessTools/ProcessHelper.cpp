#include <Windows.h>
#include <winternl.h>
#include <processthreadsapi.h>
#include <stdio.h>
#include <Psapi.h>
#include <iostream>
#include <cstring>
#include "ProcessHelper.h"
#include <string>
#include <cstring>
#include <comdef.h>
#include <atlconv.h>
using namespace System;
using namespace ProcessTools;

/*
/// GetProcessImageFileName 函数
//windows7: kernel32.dll|.lib
// windows r8+: psapi.dll|.lib
#include <psapi.h>
*/

#pragma comment(lib, "shlwapi.lib")
#define DLL_EXPORT extern "C" __declspec(dllexport) 


typedef NTSTATUS(__stdcall* NTQUERYINFORMATIONPROCESS)(
	HANDLE ProcessHandle,
	PROCESSINFOCLASS ProcessInformationClass,
	PVOID ProcessInformation,
	ULONG ProcessInformationLength,
	PULONG ReturnLength
	);

void ProcessTools::ProcessHelper::GetProcessPID(PROCESSINFO processInfo) {
	GetProcessPID(&processInfo.CurrentProcessID, &processInfo.ParentProcessID);
}

void ProcessTools::ProcessHelper::GetProcessPID(PULONG_PTR currentProcessID, PULONG_PTR parentProcessID)
{
	/// 下面是获取函数 NtQueryInformationProcess 的函数指针
	HMODULE hMod = GetModuleHandle(L"NTDLL.DLL");
	if (hMod == NULL)
	{
		return;
	}

	NTQUERYINFORMATIONPROCESS ptrNtQueryInformationProcess = (NTQUERYINFORMATIONPROCESS)GetProcAddress(hMod, "NtQueryInformationProcess");
	if (ptrNtQueryInformationProcess == NULL)
	{
		return;
	}

	PROCESS_BASIC_INFORMATION processBasicInformation;
	ULONG retLength = 0;
	NTSTATUS status = ptrNtQueryInformationProcess(GetCurrentProcess(), ProcessBasicInformation, (LPVOID)&processBasicInformation, sizeof(processBasicInformation), &retLength);

	if (NT_SUCCESS(status)) {
		///// 结构体 PROCESS_BASIC_INFORMATION 的 "UniqueProcessId"字段 是当前进程的PID
		//*CurrentProcessID = processBasicInformation.UniqueProcessId;
		///// 结构体 PROCESS_BASIC_INFORMATION 的 "Reserved3"字段 是父进程的PID
		//*ParentProcessID = (ULONG_PTR)processBasicInformation.Reserved3;

		/// 结构体 PROCESS_BASIC_INFORMATION 的 "UniqueProcessId"字段 是当前进程的PID
		SETVALUEFROMPOINTER(currentProcessID, processBasicInformation.UniqueProcessId);
		/// 结构体 PROCESS_BASIC_INFORMATION 的 "Reserved3"字段 是父进程的PID
		SETVALUEFROMPOINTER(parentProcessID, (ULONG_PTR)processBasicInformation.Reserved3);
	}
	else {
		DWORD err_code = GetLastError();
		fprintf_s(stderr, "[NtQueryInformationProcess]err code: %lu\n", err_code);
	}
}

void ProcessTools::ProcessHelper::GetProcessPIDAndName(PROCESSINFO& processInfo) {
	GetProcessPIDAndName(&processInfo.CurrentProcessID, &processInfo.ParentProcessID, processInfo.lpszBuffer_Parent_Name, &processInfo.ErrCodeForBuffer);
}

void ProcessTools::ProcessHelper::GetProcessPIDAndName(PULONG_PTR currentProcessID, PULONG_PTR parentProcessID, std::wstring& lpszBuffer_Parent_Name, PDWORD errCodeForBuffer)
{
	TCHAR szFullPath[BUFSIZ];
	memset(szFullPath, 0, sizeof(szFullPath));
	GetProcessPIDAndName(currentProcessID, parentProcessID, szFullPath, errCodeForBuffer);
	lpszBuffer_Parent_Name = std::wstring(szFullPath);

	//_bstr_t bstr(lpszBuffer_Parent_Name.c_str());
	//LPTSTR lr = (LPTSTR)bstr;
	//GetProcessPIDAndName(currentProcessID, parentProcessID, lr, errCodeForBuffer);

	///// LPTSTR => std:string
	//USES_CONVERSION;
	//lpszBuffer_Parent_Name = W2A(szFullPath);
}

void ProcessTools::ProcessHelper::GetProcessPIDAndName(PULONG_PTR currentProcessID, PULONG_PTR parentProcessID, LPTSTR lpszBuffer_Parent_Name, PDWORD ErrCodeForBuffer)
{
	/// 下面是获取函数 NtQueryInformationProcess 的函数指针
	HMODULE hMod = GetModuleHandle(L"NTDLL.DLL");
	if (hMod == NULL)
	{
		return;
	}

	NTQUERYINFORMATIONPROCESS ptrNtQueryInformationProcess = (NTQUERYINFORMATIONPROCESS)GetProcAddress(hMod, "NtQueryInformationProcess");
	if (ptrNtQueryInformationProcess == NULL)
	{
		return;
	}

	PROCESS_BASIC_INFORMATION processBasicInformation;
	ULONG retLength = 0;
	NTSTATUS status = ptrNtQueryInformationProcess(GetCurrentProcess(), ProcessBasicInformation, (LPVOID)&processBasicInformation, sizeof(processBasicInformation), &retLength);

	if (NT_SUCCESS(status)) {
		/// 结构体 PROCESS_BASIC_INFORMATION 的 "UniqueProcessId"字段 是当前进程的PID
		*currentProcessID = processBasicInformation.UniqueProcessId;
		//*currentProcessID = GetCurrentProcessId();
		/// 结构体 PROCESS_BASIC_INFORMATION 的 "Reserved3"字段 是父进程的PID
		*parentProcessID = (ULONG_PTR)processBasicInformation.Reserved3;

		DWORD dwParentID = 0;
		if (lpszBuffer_Parent_Name != NULL) {
			dwParentID = (LONG_PTR)processBasicInformation.Reserved3;
			HANDLE hParentProcess = OpenProcess(PROCESS_QUERY_INFORMATION, FALSE, dwParentID);
			if (hParentProcess) {
				//TCHAR szFullPath[BUFSIZ];
				//memset(szFullPath, 0, sizeof(szFullPath));
				/// 用来接收进程文件名和路径的长度(必须！)
				DWORD bufs = BUFSIZ;
				/// 获取进程路径
				BOOL ret = QueryFullProcessImageName(hParentProcess, 0, lpszBuffer_Parent_Name, &bufs);
				//BOOL ret = QueryFullProcessImageName(hParentProcess, 0, szFullPath, &bufs);
				if (TRUE == ret)
					SETVALUEFROMPOINTER(ErrCodeForBuffer, 0);
				else
					SETVALUEFROMPOINTER(ErrCodeForBuffer, GetLastError());

				/// 结果是DOS路径+文件名
				TCHAR buf[BUFSIZ];
				GetProcessImageFileName(hParentProcess, buf, BUFSIZ);
				OutPutStr("GetProcessImageFileName: %s\n", buf);

				CloseHandle(hParentProcess);
			}
			else {
				SETVALUEFROMPOINTER(ErrCodeForBuffer, GetLastError());
			}
		}
	}
	else {
		DWORD err_code = GetLastError();
		fprintf_s(stderr, "[NtQueryInformationProcess]err code: %lu\n", err_code);
	}
}
