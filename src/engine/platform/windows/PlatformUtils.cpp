/**
\author		Korotkov Andrey aka DRON
\date		17.09.2012 (c)Korotkov Andrey

This file is a part of DGLE project and is distributed
under the terms of the GNU Lesser General Public License.
See "DGLE.h" for more details.
*/

#include "..\..\Common.h"

#ifdef DXDIAG_VIDEO_INFO
#include <dxdiag.h>
#pragma message("Linking with \"dxguid.lib\".")
#pragma comment(linker, "/defaultlib:dxguid.lib")
#endif

#define _WIN32_DCOM 1
#include <wbemidl.h>
#include <comutil.h>
#pragma message("Linking with \"comsuppw.lib\" and \"wbemuuid.lib\".")
#pragma comment(linker, "/defaultlib:comsuppw.lib")
#pragma comment(linker, "/defaultlib:wbemuuid.lib")

using namespace std;

extern HMODULE hModule;

namespace DGLE
{

LARGE_INTEGER	perfFreq;

TWinMessage WinAPIMsgToEngMsg(UINT Msg, WPARAM wParam, LPARAM lParam)
{
	switch (Msg)
	{
	case WM_PAINT:
		return TWinMessage(WMT_REDRAW);

	case WM_SHOWWINDOW:
		if (wParam == TRUE && lParam == 0)
			return TWinMessage(WMT_PRESENT);
		else
			return TWinMessage();
		
	case WM_CLOSE:
		return TWinMessage(WMT_CLOSE);

	case WM_CREATE:
		return TWinMessage(WMT_CREATE);

	case WM_DESTROY:
		return TWinMessage(WMT_DESTROY);

	case WM_SETFOCUS:
		return TWinMessage(WMT_ACTIVATED, wParam);

	case WM_KILLFOCUS:
		return TWinMessage(WMT_DEACTIVATED, wParam);

	case WM_MOVING:
		return TWinMessage(WMT_MOVE, ((RECT*)lParam)->left, ((RECT*)lParam)->top, (RECT*)lParam);

	case WM_SIZING:
		return TWinMessage(WMT_SIZE, ((RECT*)lParam)->right - ((RECT*)lParam)->left, ((RECT*)lParam)->bottom - ((RECT*)lParam)->top, (RECT*)lParam);

	case WM_SIZE:
		RECT r;
		r.left = r.top = 0;
		r.right = LOWORD(lParam);
		r.bottom = HIWORD(lParam);

		if (wParam == SIZE_MINIMIZED)
			return TWinMessage(WMT_MINIMIZED, r.right, r.bottom, &r);					
		else
			if (wParam == SIZE_RESTORED)
				return TWinMessage(WMT_RESTORED, r.right, r.bottom, &r);			
			else
				return TWinMessage(WMT_SIZE, r.right, r.bottom, &r);

	case WM_KEYUP:
		if (lParam & 0x00100000)
		{
			if (wParam == 16)
				return TWinMessage(WMT_KEY_UP, KEY_RSHIFT);
			else
				if (wParam == 17)
					return TWinMessage(WMT_KEY_UP, KEY_RCONTROL);
				else
					return TWinMessage(WMT_KEY_UP, ASCIIKeyToEngKey((uchar)wParam));
		}
		else
			return TWinMessage(WMT_KEY_UP, ASCIIKeyToEngKey((uchar)wParam));

	case WM_KEYDOWN:
		if (lParam & 0x00100000)
		{
			if (wParam == 16)
				return TWinMessage(WMT_KEY_DOWN, KEY_RSHIFT);
			else
				if (wParam == 17)
					return TWinMessage(WMT_KEY_DOWN, KEY_RCONTROL);
				else
					return TWinMessage(WMT_KEY_DOWN, ASCIIKeyToEngKey((uchar)wParam));
		}
		else
			return TWinMessage(WMT_KEY_DOWN, ASCIIKeyToEngKey((uchar)wParam));

	case WM_SYSKEYUP:
		if (wParam == VK_MENU)
			return TWinMessage(WMT_KEY_UP, lParam & 0x00100000 ? KEY_RALT : KEY_LALT);
		else
			return TWinMessage(WMT_KEY_UP, ASCIIKeyToEngKey((uchar)wParam));
	
	case WM_SYSKEYDOWN:
		if (wParam == VK_MENU)
			return TWinMessage(WMT_KEY_DOWN, lParam & 0x00100000 ? KEY_RALT : KEY_LALT);
		else
			return TWinMessage(WMT_KEY_DOWN, ASCIIKeyToEngKey((uchar)wParam));

	case WM_CHAR:
		return TWinMessage(WMT_ENTER_CHAR, (uint32)wParam);

	case WM_MOUSEMOVE:
		return TWinMessage(WMT_MOUSE_MOVE, LOWORD(lParam), HIWORD(lParam));

	case WM_LBUTTONDOWN:
		return TWinMessage(WMT_MOUSE_DOWN, 0);

	case WM_MBUTTONDOWN:
		return TWinMessage(WMT_MOUSE_DOWN, 3);

	case WM_RBUTTONDOWN:
		return TWinMessage(WMT_MOUSE_DOWN, 2);

	case WM_LBUTTONUP:
		return TWinMessage(WMT_MOUSE_UP, 0);

	case WM_MBUTTONUP:
		return TWinMessage(WMT_MOUSE_UP, 3);

	case WM_RBUTTONUP:
		return TWinMessage(WMT_MOUSE_UP, 2);

	case WM_MOUSEWHEEL: 
		int delta;
		delta = GET_WHEEL_DELTA_WPARAM(wParam);
		return TWinMessage(WMT_MOUSE_WHEEL, 0, 0, &delta);

	default: 
		MSG msg;
		ZeroMemory(&msg, sizeof(MSG));
		msg.lParam = lParam;
		msg.wParam = wParam;
		msg.message = Msg;
		return TWinMessage(WMT_UNKNOWN, 0, 0, &msg);
	}
}

void EngMsgToWinAPIMsg(const TWinMessage &msg, UINT &Msg, WPARAM &wParam, LPARAM &lParam)
{
	switch (msg.uiMsgType)
	{
	case WMT_REDRAW:
		Msg = WM_PAINT;
		wParam = 0;
		lParam = 0;
		break;

	case WMT_PRESENT:
		Msg = WM_SHOWWINDOW;
		wParam = TRUE;
		lParam = 0;
		break;

	case WMT_CLOSE:
		Msg = WM_PAINT;
		wParam = 0;
		lParam = 0;
		break;

	case WMT_CREATE:
		Msg = WM_CREATE;
		wParam = 0;
		lParam = 0;
		break;

	case WMT_DESTROY:
		Msg = WM_DESTROY;
		wParam = 0;
		lParam = 0;
		break;

	case WMT_ACTIVATED:
		Msg = WM_SETFOCUS;
		wParam = 0;
		lParam = 0;
		break;

	case WMT_DEACTIVATED:
		Msg = WM_KILLFOCUS;
		wParam = 0;
		lParam = 0;
		break;

	case WMT_MINIMIZED:
		Msg = WM_SIZE;
		wParam = SIZE_MINIMIZED;
		lParam = MAKELPARAM((WORD)msg.ui32Param1, (WORD)msg.ui32Param2);
		break;

	case WMT_MOVE:
		Msg = WM_MOVING;
		wParam = 0;
		if (msg.pParam3)
			lParam = LPARAM(msg.pParam3);
		else
			lParam = 0;
		break;

	case WMT_RESTORED:
	case WMT_SIZE:
		Msg = WM_SIZE;
		wParam = SIZE_RESTORED;
		lParam = MAKELPARAM((WORD)msg.ui32Param1, (WORD)msg.ui32Param2);
		break;

	case WMT_KEY_UP:
		Msg = WM_KEYUP;
		wParam = EngKeyToASCIIKey((uint8)msg.ui32Param1);
		lParam = 0;
		break;

	case WMT_KEY_DOWN:
		Msg = WM_KEYDOWN;
		wParam = EngKeyToASCIIKey((uint8)msg.ui32Param1);
		lParam = 0;
		break;

	case WMT_ENTER_CHAR:
		Msg = WM_CHAR;
		wParam = msg.ui32Param1;
		lParam = 0;
		break;

	case WMT_MOUSE_MOVE:
		Msg = WM_MOUSEMOVE;
		wParam = 0;
		lParam = MAKELPARAM((WORD)msg.ui32Param1, (WORD)msg.ui32Param2);
		break;

	case WMT_MOUSE_DOWN:
		if (msg.ui32Param1 == 0)
			Msg = WM_LBUTTONDOWN;
		else
			if (msg.ui32Param1 == 2)
					Msg = WM_RBUTTONDOWN;
				else
					Msg = WM_MBUTTONDOWN;
		wParam = 0;
		lParam = 0;
		break;

	case WMT_MOUSE_UP:
		if (msg.ui32Param1 == 0)
			Msg = WM_LBUTTONUP;
		else
			if (msg.ui32Param1 == 2)
					Msg = WM_RBUTTONUP;
				else
					Msg = WM_MBUTTONUP;
		wParam = 0;
		lParam = 0;
		break;

	case WMT_MOUSE_WHEEL:
		Msg = WM_MOUSEWHEEL;
		if (msg.pParam3)
			wParam = MAKEWPARAM(0,*(int*)msg.pParam3);
		else
			wParam = 0;
		lParam = 0;
		break;
	}
}

void GetDisplaySize(uint &width, uint &height)
{
	HDC desktop_dc	= GetDC(GetDesktopWindow());
	width	= GetDeviceCaps(desktop_dc, HORZRES);
	height	= GetDeviceCaps(desktop_dc, VERTRES);
	ReleaseDC(GetDesktopWindow(), desktop_dc);
}

void GetLocalTimaAndDate(TSysTimeAndDate &time)
{
	SYSTEMTIME t;
	GetLocalTime(&t);
	time.ui16Year = t.wYear;
	time.ui16Month = t.wMonth;
	time.ui16Day = t.wDay;
	time.ui16Hour = t.wHour;
	time.ui16Minute = t.wMinute;
	time.ui16Second = t.wSecond;
	time.ui16Milliseconds = t.wMilliseconds;
}

bool PlatformInit()
{
	setlocale(LC_ALL, "");
	LoadKeyboardLayout("00000409", KLF_ACTIVATE);

	if(QueryPerformanceFrequency(&perfFreq) == FALSE)
		return false;

	return true;
}

uint64 GetPerfTimer()
{
	LARGE_INTEGER perf_counter;
	QueryPerformanceCounter(&perf_counter);
	return (uint64)(1000000*perf_counter.QuadPart/perfFreq.QuadPart);
}

void OutputDebugTxt(const char *pcTxt)
{
	OutputDebugString((string(pcTxt) + "\n").c_str());
}

struct TTimer
{
	uint uiId;
	TProcDelegate *pDelegate;
};

vector<TTimer> timers;

void DGLE_API TimerProc(HWND hwnd, UINT uMsg, UINT_PTR idEvent, DWORD dwTime)
{
	for (size_t i = 0; i < timers.size(); ++i)
		if (timers[i].uiId == idEvent)
		{
			timers[i].pDelegate->Invoke();
			break;
		}
}

uint CreateTimer(uint uiInterval, TProcDelegate *pDelOnTimer)
{
	TTimer t;
	t.pDelegate = pDelOnTimer;

	UINT id = 0;

	if (t.uiId = (uint)SetTimer(NULL, id, uiInterval, TimerProc), t.uiId == 0)
		return -1;
	else
	{
		timers.push_back(t);
		return t.uiId;
	}
}

bool ReleaseTimer(uint id)
{
	for (size_t i = 0; i < timers.size(); ++i)
		if (timers[i].uiId == id)
		{
			timers.erase(timers.begin() + i);
			break;
		}

	return KillTimer(NULL, id) != FALSE;
}

void Suspend(uint uiMilliseconds)
{
	Sleep(uiMilliseconds);
}

TDynLibHandle LoadDynamicLib(const char *pcFileName)
{
	return LoadLibraryA(pcFileName);
}

void* GetFuncAddress(TDynLibHandle tHandle, const char *pcFuncName)
{
	return GetProcAddress(tHandle, pcFuncName);
}

bool ReleaseDynamicLib(TDynLibHandle tHandle)
{
	return FreeLibrary(tHandle) != FALSE;
}

void ShowModalUserAlert(const char *pcTxt, const char *pcCaption)
{
	MessageBoxA(NULL, pcTxt, pcCaption, MB_OK | MB_ICONSTOP | MB_SETFOREGROUND | MB_SYSTEMMODAL);
}

void GetEngineFilePath(std::string &strPath)
{
	char eng_file_name[MAX_PATH];
		
	if (GetModuleFileNameA(hModule, eng_file_name, MAX_PATH) == 0)
		strcpy(eng_file_name, "");

	strPath = GetFilePath(eng_file_name) + "\\";
}

bool FindFilesInDir(const char* pcMask, std::vector<std::string> &fileNames)
{
	char* part;
	char tmp[MAX_PATH];
	HANDLE hSearch = NULL;
	WIN32_FIND_DATAA wfd;

	ZeroMemory(&wfd, sizeof(WIN32_FIND_DATAA));

	if (GetFullPathNameA(pcMask, MAX_PATH, tmp, &part) == 0) return false;

	wfd.dwFileAttributes = FILE_ATTRIBUTE_NORMAL;

	if (!((hSearch = FindFirstFileA(tmp, &wfd)) == INVALID_HANDLE_VALUE))
		do
		{
			if (!strncmp(wfd.cFileName, ".", 1) || !strncmp(wfd.cFileName, "..", 2))
				continue;

			string fullname(tmp);

			int pos = (int)fullname.find_last_of("\\");

			fullname = fullname.substr(0, pos + 1);
			fullname += string(wfd.cFileName);
			fileNames.push_back(fullname);
		}
		while (FindNextFileA(hSearch, &wfd));

		FindClose (hSearch);

		return true;
}

#ifdef DXDIAG_VIDEO_INFO
HRESULT GetStringValue(IDxDiagContainer* pObject, WCHAR* wstrName, TCHAR* strValue, int nStrLen)
{
	HRESULT hr;
	VARIANT var;
	VariantInit(&var);

	if (FAILED(hr = pObject->GetProp(wstrName, &var)))
		return hr;

	if (var.vt != VT_BSTR)
		return E_INVALIDARG;
	
#ifdef _UNICODE
	wcsncpy( strValue, var.bstrVal, nStrLen-1 );
#else
	wcstombs( strValue, var.bstrVal, nStrLen );   
#endif

	strValue[nStrLen-1] = TEXT('\0');
	VariantClear( &var );

	return S_OK;
}
#endif

void GetSystemInformation(string &strInfo, TSystemInfo &stSysInfo)
{
	string result = "System Information\n\t";

	OSVERSIONINFOEX osvi;
	bool is_ok = true;

	string str = "Operating System: ";

	ZeroMemory(&osvi, sizeof(OSVERSIONINFOEX));
	osvi.dwOSVersionInfoSize = sizeof(OSVERSIONINFOEX);

	is_ok = GetVersionEx((OSVERSIONINFO *) &osvi) != 0;

	if (!is_ok) 
		str += "Couldn't get OS version!"; 
	else
	{
		if (osvi.dwPlatformId == VER_PLATFORM_WIN32_NT && osvi.dwMajorVersion > 4)
		{	
				str += IntToStr(osvi.dwMajorVersion) + "." + IntToStr(osvi.dwMinorVersion) + " ";
				
				str += "\"Microsoft Windows ";

				if (osvi.dwMajorVersion == 6)
				{
					if (osvi.dwMinorVersion == 0)
					{
						if (osvi.wProductType == VER_NT_WORKSTATION)
							str += "Vista ";
						else
							str += "Server 2008 ";
					}
					else
						if (osvi.dwMinorVersion == 1)
						{
							if (osvi.wProductType == VER_NT_WORKSTATION)
								str += "7 ";
							else
								str += "Server 2008 R2 ";
						}

					DWORD os_type;
					typedef BOOL (WINAPI *PGPI)(DWORD, DWORD, DWORD, DWORD, PDWORD);
					PGPI p_gpi;
					p_gpi = (PGPI)GetProcAddress(GetModuleHandleA("kernel32.dll"), "GetProductInfo");
					
					if (p_gpi(osvi.dwMajorVersion, osvi.dwMinorVersion, 0, 0, &os_type) != 0)
						switch ( os_type )
						{
							case PRODUCT_UNLICENSED:
								str += "Unlicensed copy";
								break;
							case PRODUCT_ULTIMATE:
								str += "Ultimate Edition";
								break;
							case 0x00000030/*PRODUCT_PROFESSIONAL*/:
								str += "Professional";
								break;
							case PRODUCT_HOME_PREMIUM:
								str += "Home Premium Edition";
								break;
							case PRODUCT_HOME_BASIC:
								str += "Home Basic Edition";
								break;
							case PRODUCT_ENTERPRISE:
								str += "Enterprise Edition";
								break;
							case PRODUCT_BUSINESS:
								str += "Business Edition";
								break;
							case PRODUCT_STARTER:
								str += "Starter Edition";
								break;
							case PRODUCT_CLUSTER_SERVER:
								str += "Cluster Server Edition";
								break;
							case PRODUCT_DATACENTER_SERVER:
								str += "Datacenter Edition";
								break;
							case PRODUCT_DATACENTER_SERVER_CORE:
								str += "Datacenter Edition (core installation)";
								break;
							case PRODUCT_ENTERPRISE_SERVER:
								str += "Enterprise Edition";
								break;
							case PRODUCT_ENTERPRISE_SERVER_CORE:
								str += "Enterprise Edition (core installation)";
								break;
							case PRODUCT_ENTERPRISE_SERVER_IA64:
								str += "Enterprise Edition for Itanium-based Systems";
								break;
							case PRODUCT_SMALLBUSINESS_SERVER:
								str += "Small Business Server";
								break;
							case PRODUCT_SMALLBUSINESS_SERVER_PREMIUM:
								str += "Small Business Server Premium Edition";
								break;
							case PRODUCT_STANDARD_SERVER:
								str += "Standard Edition";
								break;
							case PRODUCT_STANDARD_SERVER_CORE:
								str += "Standard Edition (core installation)";
								break;
							case PRODUCT_WEB_SERVER:
								str += "Web Server Edition";
								break;
						}
				}
				else
					if (osvi.dwMajorVersion == 5)
					{
						if (osvi.dwMinorVersion == 0)
						{
							str += "2000 ";

							if (osvi.wProductType == VER_NT_WORKSTATION)
								str += "Professional";
							else 
							{
								if (osvi.wSuiteMask & VER_SUITE_DATACENTER )
									str += "Datacenter Server";
								else
									if (osvi.wSuiteMask & VER_SUITE_ENTERPRISE)
										str += "Advanced Server";
									else
										str += "Server";
							}
						}
						else
							if (osvi.dwMinorVersion == 1)
							{
								str += "XP ";

								if (osvi.wSuiteMask & VER_SUITE_PERSONAL)
									str += "Home Edition";
								else
									str += "Professional";

							}
							else
								if (osvi.dwMinorVersion == 2)
								{
									if (GetSystemMetrics(SM_SERVERR2))
										str += "Server 2003 R2, ";
									else
										if (osvi.wSuiteMask & VER_SUITE_STORAGE_SERVER)
											str += "Storage Server 2003";
										else
											if (osvi.wSuiteMask & 0x00008000/*VER_SUITE_WH_SERVER*/)
												str += "Home Server";
											else
												if (osvi.wProductType == VER_NT_WORKSTATION)
													str += "XP Professional x64 Edition";
												else
													str += "Server 2003";
						
									if (osvi.wProductType != VER_NT_WORKSTATION)
									{
										if (osvi.wSuiteMask & VER_SUITE_COMPUTE_SERVER)
											str += "Compute Cluster Edition";
										else 
											if (osvi.wSuiteMask & VER_SUITE_DATACENTER)
												str += "Datacenter Edition";
											else
												if (osvi.wSuiteMask & VER_SUITE_ENTERPRISE)
													str += "Enterprise Edition";
												else
													if (osvi.wSuiteMask & VER_SUITE_BLADE)
														str += "Web Edition";
													else
														str += "Standard Edition";
									}

								}
					}

				str+= "\" " + (string)osvi.szCSDVersion + " (Build " + IntToStr(osvi.dwBuildNumber) + ")";

				if (osvi.dwMajorVersion >= 6)
				{
#ifdef _WIN64
					str+= ", 64-bit";
#else
					typedef BOOL (WINAPI *PISWOW64)(HANDLE, PBOOL);
					PISWOW64 p_is_wow;
					BOOL b_is_wow = FALSE;
					p_is_wow = (PISWOW64)GetProcAddress(GetModuleHandleA("kernel32"),"IsWow64Process");
					if (NULL != p_is_wow) p_is_wow(GetCurrentProcess(), &b_is_wow);

					if (b_is_wow)
						str+= ", 64-bit";
					else
						str+= ", 32-bit";
#endif
				}

		} else 
			str += "Unsupported or unknown Microsoft Windows version.";

		result += str + "\n\t";

		if (str.size() < 128)
			strcpy(stSysInfo.cOSName, str.c_str());
		else
		{
			memcpy(stSysInfo.cOSName, str.c_str(), 127);
			stSysInfo.cOSName[127] = '\0';
		}
	} 

	HKEY h_key;
	uint32 dw_type;
	uint32 dw_size;
	uint32 mhz;
	
	int32 ret = RegOpenKeyEx(HKEY_LOCAL_MACHINE, "HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0", 0, KEY_READ, &h_key);
	
	dw_size = 0;
	ret = RegQueryValueEx(h_key, "ProcessorNameString", NULL, &dw_type, NULL, &dw_size);
	_ASSERT(dw_size > 0);
	
	char *pc_processor_name = new char[dw_size];
	ret = RegQueryValueEx(h_key, "ProcessorNameString", NULL, &dw_type, (LPBYTE)pc_processor_name, &dw_size);
	
	if (ret != ERROR_SUCCESS)
		pc_processor_name = "Couldn't get CPU name!";

	dw_size = sizeof(DWORD);
	ret = RegQueryValueEx(h_key, "~MHz", NULL, &dw_type, (LPBYTE)&mhz, &dw_size);
	if (ret != ERROR_SUCCESS)
		pc_processor_name = "Couldn't get CPU speed!";

	RegCloseKey(h_key);

	SYSTEM_INFO st_sys_info; 
	ZeroMemory(&st_sys_info, sizeof(SYSTEM_INFO));
	::GetSystemInfo((SYSTEM_INFO*) &st_sys_info);

	str = "CPU: ";
	str += string(pc_processor_name) + " (~" + IntToStr(mhz) + " Mhz";
	if (st_sys_info.dwNumberOfProcessors>1) str+=" X "+IntToStr(st_sys_info.dwNumberOfProcessors);
	str +=")";

	result += str + "\n\t";
	stSysInfo.uiCPUFreq = mhz;
	stSysInfo.uiCPUCount = st_sys_info.dwNumberOfProcessors;
	
	if (strlen(pc_processor_name) <= 128)
		strcpy(stSysInfo.cCPUName, pc_processor_name);
	else
	{
		memcpy(stSysInfo.cCPUName, pc_processor_name, 127);
		stSysInfo.cCPUName[127] = '\0';
	}
	
	delete[] pc_processor_name;

	MEMORYSTATUSEX stat;
	stat.dwLength = sizeof (stat);
	GlobalMemoryStatusEx(&stat);

	uint64	ram_free	= stat.ullAvailPhys/(1024*1024),
			ram_total	= stat.ullTotalPhys/(1024*1024);
	
	str = "RAM Total: " + UInt64ToStr(ram_total) + " MB.";
	result += str + "\n\t";
	str = "RAM Available: " + UInt64ToStr(ram_free) + " MB.";
	result += str + "\n\t";

	stSysInfo.uiRAMAvailable	= (uint)ram_free;
	stSysInfo.uiRAMTotal		= (uint)ram_total;

	str = "Video device: ";

	HRESULT hr, chr = CoInitialize(NULL);

	string vcard_advanced_str = "";

#ifdef DXDIAG_VIDEO_INFO
	IDxDiagProvider*  p_dx_diag_provider;

	hr = E_FAIL;

	if (SUCCEEDED(chr))
		hr = CoCreateInstance( CLSID_DxDiagProvider, NULL, CLSCTX_INPROC_SERVER, IID_IDxDiagProvider, (LPVOID*) &p_dx_diag_provider);
	
	if (SUCCEEDED(hr))
	{
		DXDIAG_INIT_PARAMS dx_diag_init_param;
		ZeroMemory(&dx_diag_init_param, sizeof(DXDIAG_INIT_PARAMS));

		dx_diag_init_param.dwSize                  = sizeof(DXDIAG_INIT_PARAMS);
		dx_diag_init_param.dwDxDiagHeaderVersion   = DXDIAG_DX9_SDK_VERSION;
		dx_diag_init_param.bAllowWHQLChecks        = BOOL(false);
		dx_diag_init_param.pReserved               = NULL;

		hr = p_dx_diag_provider->Initialize(&dx_diag_init_param);

		IDxDiagContainer* p_dx_diag_root;

		hr = p_dx_diag_provider->GetRootContainer(&p_dx_diag_root);

		IDxDiagContainer* p_container = NULL;

		hr = p_dx_diag_root->GetChildContainer(L"DxDiag_DisplayDevices", &p_container);
		
		DWORD nInstanceCount = 0;
		hr = p_container->GetNumberOfChildContainers(&nInstanceCount);

		if(nInstanceCount > 1)
			str+="(Count: " + IntToStr(nInstanceCount)+") ";

		stSysInfo.uiVideocardCount = nInstanceCount;

		IDxDiagContainer* p_object = NULL;

		wchar wc_container[256];

		hr = p_container->EnumChildContainerNames(0, wc_container, 256);
		hr = p_container->GetChildContainer(wc_container, &p_object);

		#define EXPAND(x) x, sizeof(x)/sizeof(TCHAR)
		
		char txt[200];

		GetStringValue(p_object, L"szDescription", EXPAND(txt));
		str+=string(txt);

		if(strlen(txt) <= 128)
			strcpy(stSysInfo.cVideocardName, txt);
		else
		{
			memcpy(stSysInfo.cVideocardName, txt, 127);
			stSysInfo.cOSName[127] = '\0';
		}

		GetStringValue( p_object, L"szDisplayMemoryLocalized", EXPAND(txt));
		
		string str_ts = txt, str_ts2 = "", str_res = "";
		
		for (uint i = 0; i < str_ts.length(); ++i)
			if (str_ts[i] == '.')
				str_res = str_ts2;
			else
				str_ts2 += str_ts[i];
		
		stSysInfo.uiVideocardRAM = StrToInt(str_res);

		GetStringValue( p_object, L"szDriverName", EXPAND(txt));
		vcard_advanced_str = "(driver \"" + string(txt) + "\",";

		GetStringValue( p_object, L"szDriverVersion", EXPAND(txt));
		vcard_advanced_str += " ver. " + string(txt) + ",";

		GetStringValue( p_object, L"szDriverDateLocalized", EXPAND(txt));
		vcard_advanced_str += " date " + string(txt) + ")";

		p_object->Release();
		p_container->Release();
		p_dx_diag_root->Release();
		p_dx_diag_provider->Release();
	}
	else 
	{
#endif

	DISPLAY_DEVICE dd;

	ZeroMemory(&dd, sizeof(DISPLAY_DEVICE));
	dd.cb = sizeof(DISPLAY_DEVICE);

	if (EnumDisplayDevices(NULL, 0, (DISPLAY_DEVICE*)&dd, 0) == 0)
		str = "Error getting video device!";
	else
		str += dd.DeviceString;

	stSysInfo.uiVideocardRAM    = 0;
	stSysInfo.uiVideocardCount  = 0;

	strcpy(stSysInfo.cVideocardName, dd.DeviceString); 

#ifdef DXDIAG_VIDEO_INFO
	}
#endif

	IWbemLocator* locator = NULL;
	uint vram = 0;
	hr = E_FAIL;

	if (SUCCEEDED(chr))
		hr = CoCreateInstance(CLSID_WbemLocator, NULL, CLSCTX_INPROC_SERVER, IID_IWbemLocator, (void**)&locator);

	if (SUCCEEDED(hr))
	{
		IWbemServices* services = NULL;
		hr = locator->ConnectServer(_bstr_t("root\\cimv2"), NULL, NULL, NULL, 0, NULL, NULL, &services);
		locator->Release();

		if (SUCCEEDED(hr))
		{
			hr = CoSetProxyBlanket(services, RPC_C_AUTHN_WINNT, RPC_C_AUTHZ_NONE, NULL, RPC_C_AUTHN_LEVEL_CALL, RPC_C_IMP_LEVEL_IMPERSONATE, NULL, EOAC_NONE);
			if (FAILED(hr))
				services->Release();
			else
			{
				IEnumWbemClassObject* instance_enum = NULL;
				hr = services->CreateInstanceEnum(_bstr_t("Win32_VideoController"), WBEM_FLAG_FORWARD_ONLY | WBEM_FLAG_RETURN_IMMEDIATELY, NULL, &instance_enum);
				services->Release();
				
				if (SUCCEEDED(hr))
				{
					IWbemClassObject* instance;
					ULONG objectsReturned = 0;

					while (true)
					{
						hr = instance_enum->Next(WBEM_INFINITE, 1, &instance, &objectsReturned);
						if(FAILED(hr))
							break;

						if(objectsReturned != 1)
							break;

						VARIANT v;
						VariantInit(&v);
						hr = instance->Get(_bstr_t("AdapterRAM"), 0, &v, NULL, NULL);
						
						if(SUCCEEDED(hr))
							vram = V_UI4(&v)/(1024*1024);

						VariantClear(&v);
						instance->Release();
						instance = NULL;
					}

					instance_enum->Release();
				}
			}
		}
	}

	if(SUCCEEDED(chr))
		CoUninitialize();

	if(vram != 0)
		stSysInfo.uiVideocardRAM = vram;

	result += str + " " + IntToStr(stSysInfo.uiVideocardRAM) + " MB. " + vcard_advanced_str;

	strInfo = result;
}

void Terminate()
{
	TerminateProcess(GetCurrentProcess(), 2);
}

}