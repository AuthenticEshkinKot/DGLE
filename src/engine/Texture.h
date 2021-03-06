/**
\author		Korotkov Andrey aka DRON
\date		01.05.2012 (c)Korotkov Andrey

This file is a part of DGLE project and is distributed
under the terms of the GNU Lesser General Public License.
See "DGLE.h" for more details.
*/

#pragma once

#include "Common.h"
#include "ResourceManager.h"

class CRender2D;

class CTexture: public CInstancedObj, public ITexture
{
	ICoreRenderer *_pCoreRenderer;
	CRender2D *_pRender2D;
	ICoreTexture *_pCoreTexture;
	uint _uiWidth, _uiHeight, _uiFrameWidth, _uiFrameHeight;
	float _quad[16];

public:

	CTexture(uint uiInstIdx, ICoreTexture *pCoreTex, uint uiWidth, uint uiHeight);
	~CTexture();

	DGLE_RESULT DGLE_API GetDimensions(uint &uiWidth, uint &uiHeight);
	DGLE_RESULT DGLE_API SetFrameSize(uint uiFrameWidth, uint uiFrameHeight);
	DGLE_RESULT DGLE_API GetFrameSize(uint &uiFrameWidth, uint &uiFrameHeight);
	
	DGLE_RESULT DGLE_API GetCoreTexture(ICoreTexture *&prCoreTex);
	
	DGLE_RESULT DGLE_API Draw2DSimple(int iX, int iY, uint uiFrameIndex);
	DGLE_RESULT DGLE_API Draw2D(int iX, int iY, uint uiWidth, uint uiHeight, float fAngle, uint uiFrameIndex);
	DGLE_RESULT DGLE_API Draw3D(uint uiFrameIndex);
	
	DGLE_RESULT DGLE_API Bind(uint uiMTextureLayer);

	IENGBASEOBJ_IMPLEMENTATION(EOT_TEXTURE)
	IDGLE_BASE_IMPLEMENTATION(ITexture, INTERFACE_IMPL(IEngBaseObj, INTERFACE_IMPL_END))
};