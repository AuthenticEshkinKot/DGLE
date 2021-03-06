/**
\author		Korotkov Andrey aka DRON
\date		29.04.2012 (c)Korotkov Andrey

This file is a part of DGLE project and is distributed
under the terms of the GNU Lesser General Public License.
See "DGLE.h" for more details.
*/

#pragma once

#include "Common.h"

class CRender2D;
class CRender3D;

class CRender: public CInstancedObj, public IRender
{
	float _fFovAngle, _fZNear, _fZFar;
	int _iMaxTexResolution, _iMaxAnisotropy, _iMaxLight, _iMaxTexUnits;

	std::string _strFeturesList, _strMetricsList;

	ICoreRenderer *_pCoreRenderer;

	CRender2D *_pRender2D;
	CRender3D *_pRender3D;

	void _SetPerspectiveMatrix(uint width, uint height);

	static void DGLE_API _s_ConListFeatures(void *pParametr, const char *pcParam);

public:

	CRender(uint uiInstIdx);
	~CRender();

	inline CRender2D* pRender2D() { return _pRender2D; }
	inline CRender3D* pRender3D() { return _pRender3D; }

	void BeginRender();
	void EndRender();
	void OnResize(uint uiWidth, uint uiHeight);

	DGLE_RESULT DGLE_API SetClearColor(const TColor4 &stColor);
	DGLE_RESULT DGLE_API Unbind(E_ENG_OBJ_TYPE eType);
	DGLE_RESULT DGLE_API EnableScissor(const TRectF &stArea);	
	DGLE_RESULT DGLE_API DisableScissor();
	DGLE_RESULT DGLE_API SetRenderTarget(ITexture* pTargetTex);
	DGLE_RESULT DGLE_API ScreenshotBMP(const char* pFileName);
	DGLE_RESULT DGLE_API CreatePostProcess(IPostprocess *&pPP);

	DGLE_RESULT DGLE_API GetRender2D(IRender2D *&prRender2D);
	DGLE_RESULT DGLE_API GetRender3D(IRender3D *&prRender3D);

	DGLE_RESULT DGLE_API GetType(E_ENGINE_SUB_SYSTEM &eSubSystemType);

	IDGLE_BASE_IMPLEMENTATION(IRender, INTERFACE_IMPL(IEngineSubSystem, INTERFACE_IMPL_END))
};