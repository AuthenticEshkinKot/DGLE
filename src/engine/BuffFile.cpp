/**
\author		Kotlyarov Semen aka Kimer
\date		29.10.2012 (c)Andrey Korotkov

This file is a part of DGLE project and is distributed
under the terms of the GNU Lesser General Public License.
See "DGLE.h" for more details.
*/

#include "BuffFile.h"

using namespace std;

CBuffFile::CBuffFile(uint8 *pData, uint32 ui32DataSize, bool bDelData, const std::string &strFileName):
_strFileName(strFileName), _ui32Pos(0), _ui32Size(ui32DataSize),
_pData(pData), _bDelData(bDelData)
{
	if (_strFileName.empty())
		_strFileName = "Virtual." + UIntToStr(*pData);
}

DGLE_RESULT DGLE_API CBuffFile::Read(void *pBuffer, uint uiCount, uint &uiRead)
{
	memcpy(pBuffer, &_pData[_ui32Pos], uiCount);
	
	_ui32Pos += uiCount;
	uiRead = uiCount;

	return S_OK;
}

DGLE_RESULT DGLE_API CBuffFile::Write(const void *pBuffer, uint uiCount, uint &uiWritten)
{
	uiWritten = 0;
	return E_NOTIMPL;
}

DGLE_RESULT DGLE_API CBuffFile::Seek(uint32 ui32Offset, E_FILE_SYSTEM_SEEK_FLAG eWay, uint32 &ui32Position)
{
	switch(eWay)
	{
	case FSSF_BEGIN:
		_ui32Pos = ui32Offset;
		break;
	case FSSF_END:
		_ui32Pos = _ui32Size - ui32Offset;
		break;
	case FSSF_CURRENT:
		_ui32Pos += ui32Offset;
		break;
	}
	
	ui32Position = _ui32Pos;

	return S_OK;
}

DGLE_RESULT DGLE_API CBuffFile::GetSize(uint32 &ui32Size)
{
	ui32Size = _ui32Size;
	return S_OK;
}

DGLE_RESULT DGLE_API CBuffFile::IsOpen(bool &bOpened)
{
	bOpened = true;
	return S_OK;
}

DGLE_RESULT DGLE_API CBuffFile::GetName(char *pcName, uint &uiCharsCount)
{
	string name = GetFileName(_strFileName.c_str());

	if (!pcName)
	{
		uiCharsCount = name.size() + 1;
		return S_OK;
	}

	if (uiCharsCount <= name.size())
	{
		uiCharsCount = name.size() + 1;
		strcpy(pcName, "");
		return E_INVALIDARG;
	}

	strcpy(pcName, name.c_str());

	return S_OK;
}

DGLE_RESULT DGLE_API CBuffFile::GetPath(char *pcPath, uint &uiCharsCount)
{
	string path = GetFilePath(_strFileName.c_str());

	if (!pcPath)
	{
		uiCharsCount = path.size() + 1;
		return S_OK;
	}

	if (uiCharsCount <= path.size())
	{
		uiCharsCount = path.size() + 1;
		strcpy(pcPath, "");
		return E_INVALIDARG;
	}

	strcpy(pcPath, path.c_str());

	return S_OK;
}

DGLE_RESULT DGLE_API CBuffFile::Free()
{
	if (_bDelData)
		delete[] _pData;

	delete this;
	
	return S_OK;
}