// ProcessTools.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。
//

#ifndef UNICODE
#define UNICODE
#endif
#ifndef _UNICODE
#define _UNICODE
#endif

#include <iostream>
#include <Shlwapi.h>
#include <tchar.h>
#include <comdef.h>
#include "ProcessHelper.h"
using namespace ProcessTools;

#if defined(UNICODE) || defined(_UNICODE)
#define OutPutStr(f, v) wprintf_s(L##f, v)
#else
#define OutPutStr(f, v) printf_s(f, v)
#endif

int main(int argc, TCHAR* argv[])
{
#if USE_STRING
	ULONG_PTR currentProcessID;
	ULONG_PTR parentProcessID;

	std::wstring lpszBuffer_Parent_Name;
	DWORD errCodeForBuffer;
	ProcessHelper^ helper = gcnew ProcessHelper();
	helper->GetProcessPIDAndName(&currentProcessID, &parentProcessID, lpszBuffer_Parent_Name, &errCodeForBuffer);
	if (errCodeForBuffer) {
		fprintf(stderr, "GetProcessName--> err code: %lu\n", errCodeForBuffer);
	}

	OutPutStr("CurrentProcessPID: %llu\n", currentProcessID);
	OutPutStr("ParentProcessPID: %llu\n", parentProcessID);
	OutPutStr("ParentProcessFullPath: %s\n", lpszBuffer_Parent_Name.c_str());
	PathStripPath(const_cast<LPWSTR>(lpszBuffer_Parent_Name.c_str()));
	OutPutStr("ParentProcessName: %s\n", lpszBuffer_Parent_Name.c_str());
#else
	PROCESSINFO processInfo;
	memset(&processInfo, 0, sizeof(processInfo));
	ProcessHelper^ helper = gcnew ProcessHelper();

	helper->GetProcessPID(&processInfo.CurrentProcessID, &processInfo.ParentProcessID);
	OutPutStr("-------------------------------%s-----------------------------------\n", L"USE_OBJECT_GetProcessPID");
	OutPutStr("CurrentProcessPID: %llu\n", processInfo.CurrentProcessID);
	OutPutStr("ParentProcessPID: %llu\n", processInfo.ParentProcessID);
	OutPutStr("-------------------------------%s-----------------------------------\n\n", L"USE_OBJECT_GetProcessPID");

	OutPutStr("------------------------------%s------------------------------------\n", L"USE_OBJECT_GetProcessPIDAndName");
	helper->GetProcessPIDAndName(processInfo);
	if (processInfo.ErrCodeForBuffer) {
		fprintf(stderr, "GetProcessName--> err code: %lu\n", processInfo.ErrCodeForBuffer);
	}
	OutPutStr("CurrentProcessPID: %llu\n", processInfo.CurrentProcessID);
	OutPutStr("ParentProcessPID: %llu\n", processInfo.ParentProcessID);
	OutPutStr("ParentProcessFullPath: %s\n", processInfo.lpszBuffer_Parent_Name);
	PathStripPath(processInfo.lpszBuffer_Parent_Name);
	OutPutStr("ParentProcessName: %s\n", processInfo.lpszBuffer_Parent_Name);
	OutPutStr("------------------------------%s------------------------------------\n\n", L"USE_OBJECT_GetProcessPIDAndName");

	ULONG_PTR currentProcessID;
	ULONG_PTR parentProcessID;

	OutPutStr("------------------------------%s------------------------------------\n", L"USE_FIELD_GetProcessPIDAndName");
	helper->GetProcessPID(&currentProcessID, &parentProcessID);
	OutPutStr("CurrentProcessPID: %llu\n", currentProcessID);
	OutPutStr("ParentProcessPID: %llu\n", parentProcessID);
	OutPutStr("------------------------------%s------------------------------------\n\n", L"USE_FIELD_GetProcessPIDAndName");

	TCHAR lpszBuffer_Parent_Name[BUFSIZ];
	memset(lpszBuffer_Parent_Name, 0, sizeof(lpszBuffer_Parent_Name));
	DWORD errCodeForBuffer;

	OutPutStr("------------------------------%s------------------------------------\n", L"USE_FIELD_GetProcessPIDAndName");
	helper->GetProcessPIDAndName(&currentProcessID, &parentProcessID, lpszBuffer_Parent_Name, &errCodeForBuffer);
	if (errCodeForBuffer) {
		fprintf(stderr, "GetProcessName--> err code: %lu\n", errCodeForBuffer);
	}
	OutPutStr("CurrentProcessPID: %llu\n", currentProcessID);
	OutPutStr("ParentProcessPID: %llu\n", parentProcessID);
	OutPutStr("ParentProcessFullPath: %s\n", lpszBuffer_Parent_Name);
	PathStripPath(lpszBuffer_Parent_Name);
	OutPutStr("ParentProcessName: %s\n", lpszBuffer_Parent_Name);
	OutPutStr("------------------------------%s------------------------------------\n\n", L"USE_FIELD_GetProcessPIDAndName");
#endif 

	system("pause");
}


// 运行程序: Ctrl + F5 或调试 >“开始执行(不调试)”菜单
// 调试程序: F5 或调试 >“开始调试”菜单

// 入门使用技巧: 
//   1. 使用解决方案资源管理器窗口添加/管理文件
//   2. 使用团队资源管理器窗口连接到源代码管理
//   3. 使用输出窗口查看生成输出和其他消息
//   4. 使用错误列表窗口查看错误
//   5. 转到“项目”>“添加新项”以创建新的代码文件，或转到“项目”>“添加现有项”以将现有代码文件添加到项目
//   6. 将来，若要再次打开此项目，请转到“文件”>“打开”>“项目”并选择 .sln 文件

