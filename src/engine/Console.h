/**
\author		Korotkov Andrey aka DRON
\date		07.10.2012 (c)Korotkov Andrey

This file is a part of DGLE project and is distributed
under the terms of the GNU Lesser General Public License.
See "DGLE.h" for more details.
*/

#pragma once

#include "Common.h"

struct TConEntry
{
	char	*pcName;
	char	*pcHelp;
	void	*pParametr;
	int		*piValue;
	int		 iMinValue,
			 iMaxValue;

	void(DGLE_API *pProc)(void *pParametr, const char *pcParam);

	bool operator < (const TConEntry &entry) const
	{
		std::string s1(pcName), s2(entry.pcName);
		return std::lexicographical_compare(s1.begin(), s1.end(), s2.begin(), s2.end());
	}

};

class CConsole : public CInstancedObj
{
	IConsoleWindow *_pConsoleWindow;
	std::vector<TConEntry>	_commands;
	std::vector<std::string> _prevCommands;
	int _iPrevMarker;

	bool _ProcessConCmd(const std::string &command);
	void _OnCmdComplete(const char *pcParam);
	void _OnCmdPrev();
	void _OnCmdNext();
	void _SetPos(const char *pcParam);
	void _SetSize(const char *pcParam);
	void _Save(const std::string &strFileName);
	void _Cmdlist();
	void _Help(const char* pcParam);
	
	static void DGLE_API _s_Hide(void *pParametr, const char *pcParam);
	static void DGLE_API _s_Show(void *pParametr, const char *pcParam);
	static void DGLE_API _s_Save(void *pParametr, const char *pcParam);
	static void DGLE_API _s_Clear(void *pParametr, const char *pcParam);
	static void DGLE_API _s_Terminate(void *pParametr, const char *pcParam);
	static void DGLE_API _s_SetPos(void *pParametr, const char *pcParam);
	static void DGLE_API _s_SetSize(void *pParametr, const char *pcParam);
	static void DGLE_API _s_ResetPos(void *pParametr, const char *pcParam);
	static void DGLE_API _s_Cmdlist(void *pParametr, const char *pcParam);
	static void DGLE_API _s_Help(void *pParametr, const char *pcParam);

	static void DGLE_API _s_OnConWindowEvent(CConsole *pConsole, E_CONSOLE_WINDOW_EVENT eEventType, const char *pcCommand);

public:

	CConsole(uint uiInsIdx);	 
	~CConsole();

	void  Visible(bool bVisible);
	TWinHandle GetWindowHandle();
	uint32 GetThreadId();
	void  Write(const std::string &strTxt, bool bToPrevLine = false);
	void  Exec(const char *pcCommand);
	void  RegComProc(const char *pcName, const char *pcHelp, void (DGLE_API *pProc)(void *pParametr, const char *pcParam), void *pParametr); 
	void  RegComValue(const char *pcName, const char *pcHelp, int *piValue, int iMin, int iMax, void (DGLE_API *pProc)(void *pParametr, const char *pcParam) = NULL, void *pParametr = NULL);
	bool  UnRegCom(const char *pcName);
	void  EnterThreadSafeSection();
	void  LeaveThreadSafeSection();
	void  ResetWinPos();
};