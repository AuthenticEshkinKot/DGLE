﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;
//using System.Runtime.CompilerServices;

namespace DGLE
{
    	//variant//

	public enum E_DGLE_VARIANT_TYPE
	{
		DVT_UNKNOWN = 0,
		DVT_INT,
		DVT_FLOAT,
		DVT_BOOL,
		DVT_POINTER,
		DVT_DATA
	};

	public struct TVariant
	{
	

		private E_DGLE_VARIANT_TYPE _type;
		private int _i;
		private float _f;
        private IntPtr? _p; // it is nullable for all operations to be ok, is nullable ok ? // phomm

		public void Clear()
		{
			_type = E_DGLE_VARIANT_TYPE.DVT_UNKNOWN;
			_i = 0;
			_f = 0.0f;
			_p = null; 
		}

		public void SetInt(int iVal)
		{
			Clear();
			_type = E_DGLE_VARIANT_TYPE.DVT_INT;
			_i = iVal;
		}

		public void SetFloat(float fVal)
		{
			Clear();
			_type = E_DGLE_VARIANT_TYPE.DVT_FLOAT;
			_f = fVal;
		}

		public void SetBool(bool bVal)
		{
			Clear();
			_type = E_DGLE_VARIANT_TYPE.DVT_BOOL;
			_i = bVal ? 1 : 0;
		}

		public void SetPointer(IntPtr pointer)
		{
			Clear();
			_type = E_DGLE_VARIANT_TYPE.DVT_POINTER;
			_p = pointer;
		}

		public void SetData(/*[MarshalAs(UnmanagedType.LPArray)] byte[]*/ IntPtr pData, uint uiDataSize)
		{
			Clear();
			_type = E_DGLE_VARIANT_TYPE.DVT_DATA;
			_p = (IntPtr)pData;
			_i = (int)uiDataSize;
		}

		public int AsInt() 
		{
			if (_type != E_DGLE_VARIANT_TYPE.DVT_INT)
				return 0;
			else
				return _i;
		}

		public float AsFloat() 
		{
			if (_type != E_DGLE_VARIANT_TYPE.DVT_FLOAT)
				return 0.0f;
			else
				return _f;
		}

		public bool AsBool() 
		{
			if (_type != E_DGLE_VARIANT_TYPE.DVT_BOOL)
				return false;
			else
				return _i == 1;
		}

		public IntPtr? AsPointer() 
		{
			if (_type != E_DGLE_VARIANT_TYPE.DVT_POINTER)
				return null;
			else
				return _p;
		}

		public void GetData(/*[MarshalAs(UnmanagedType.LPArray)] byte[]*/ IntPtr? pData, out uint uiDataSize) 
		{
			if (_type != E_DGLE_VARIANT_TYPE.DVT_DATA)
			{
				pData = null;
				uiDataSize = 0;
			}
			else
			{
				pData = _p;
				uiDataSize = (uint)_i;
			}
		}

		public E_DGLE_VARIANT_TYPE GetType() 
        { 
            return _type;
        }

        public static implicit operator int(TVariant v) { return v._i; }

        public static implicit operator float(TVariant v) { return v._f; }

        public static implicit operator bool(TVariant v) { return v._i == 1; }

        public static implicit operator IntPtr?(TVariant v) { return v._p; }

	} 



	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TWinMessage
	{
		public UInt32 message;
		public UInt32 wParam;
		public UInt32 lParam;

		public static TWinMessage Default
		{
			get { return new TWinMessage(0, 0, 0); }
		}
		
		public TWinMessage(UInt32 msg, UInt32 wpm, UInt32 lpm)
		{
			message = msg;
			wParam	= wpm;
			lParam	= lpm;
		}
	};

	public enum E_ENG_WINDOW_FLAGS
	{
		EWF_DEFAULT = 0x00000000,/**< This flag is suitable in most cases. */
		EWF_ALLOW_SIZEING = 0x00000001,/**< User can resize engine window arbitrarily */
		EWF_TOPMOST = 0x00000002,/**< Engine window will be always on top. */
		EWF_DONT_HOOK_MLOOP = 0x00000004,/**< If flag set and engine doesn't owns window, host applications main loop will not be hooked. User must call window repaint manually. */
		EWF_DONT_HOOK_ROOT_WIN = 0x00000008,/**< If flag set and engine doesn't owns window, main host application window will not be hooked. User must redirect windows messages manually.*/
		EWF_RESTRICT_ALT_ENTER = 0x00000010,/**< Switching between fullscreen and windowed modes by pressing "Alt-Enter" will be restricted. */
		EWF_RESTRICT_CONSOLE = 0x00000020 /**< Restricts calling engine console window by pressing "~" key. */
	};

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TEngWindow
	{	
		public UInt32 Width;
		public UInt32 Height;
		[MarshalAs(UnmanagedType.U1)] public bool FullScreen;
		[MarshalAs(UnmanagedType.U1)] public bool VSync;
		public UInt32 SamplesCount;
		public E_ENG_WINDOW_FLAGS Flags;

		public static TEngWindow Default
		{
			get { return new TEngWindow(800, 600, false, false, 1, E_ENG_WINDOW_FLAGS.EWF_DEFAULT); }
		}

		public TEngWindow(UInt32 width, UInt32 height, [MarshalAs(UnmanagedType.U1)] bool fscreen, [MarshalAs(UnmanagedType.U1)] bool vsync, UInt32 scount, E_ENG_WINDOW_FLAGS flags)
		{
			Width		    = width;
			Height		    = height;
			FullScreen	    = fscreen;
			VSync		    = vsync;
			SamplesCount    = scount;
			Flags           = (E_ENG_WINDOW_FLAGS)flags;
		}
	};

	[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet=CharSet.Ansi)]
	public struct TSystemInfo
	{	
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string os_name;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string cpu_name;
		public UInt32 cpu_count;
		public UInt32 cpu_freq;
		public UInt32 ram_total;
		public UInt32 ram_free;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string videocard_name;
		public UInt32 videocard_count;
		public UInt32 videocard_ram;
	};

	[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet=CharSet.Ansi)]
	public struct TPluginInfo
	{
		public byte btPluginSDKVersion;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string cName;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		public string cVersion;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string cVendor;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		public string cDescription;
	};

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct NamingBitmap
	{
		public byte[] _bytes;
		public int _iHeight;
		public int _iWidth;
		public string _strName;
		public NamingBitmap(byte[] bytes, int iHeight, int iWidth, string strName)
		{
			_bytes = bytes;
			_iHeight = iHeight;
			_iWidth = iWidth;
			_strName = strName;
		}
	};

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TColor4
	{
		public float r, g, b, a;
	 
		public static TColor4 White
		{
			get { return new TColor4(255, 255, 255, 255); }
		}

		public TColor4(int color, byte alpha)
		{
			r = (color & 0xff)          /255.0f;
			g = ((color >> 8) & 0xff)   /255.0f;
			b = (color >> 16)           /255.0f;
			a = (float)(alpha/255.0f);
		}

		public TColor4(UInt32 ui32RGBA)
		{
			r = (float)((ui32RGBA & 0xff) & 0xff)/0xff;
			g = (float)((ui32RGBA >> 8) & 0xff)/0xff;
			b = (float)((ui32RGBA >> 16) & 0xff)/0xff;
			a = (float)(ui32RGBA >> 24)/0xff;
		}

		public TColor4(byte ubR, byte ubG, byte ubB, byte ubA)
		{			
			r = ubR / 255.0f; g = ubG / 255.0f; b = ubB / 255.0f; a = ubA / 255.0f;
		}

		public void SetColorF(float fR, float fG, float fB, float fA)
		{
			r = fR; g = fG; b = fB; a = fA;
		}

		public void SetColorB(byte ubR, byte ubG, byte ubB, byte ubA)
		{
			r = ubR / 255.0f; g = ubG / 255.0f; b = ubB / 255.0f; a = ubA / 255.0f;
		}

		public UInt32 ColorRGB()
		{
			return (UInt32)(255 * r + (255 * g) * 0x100 + (255 * b) * 0x10000);
		}

		public UInt32 ColorRGBA()
		{
			return (UInt32)(255 * r + (255 * g) * 0x100 + (255 * b) * 0x10000 + (255 * a) * 0x1000000);
		}


		public static bool operator ==(TColor4 left, TColor4 right)
		{
			if (left.r != right.r || left.g != right.g || left.b != right.b || left.a != right.a)
				return false;
			return true;
		}
		public static bool operator !=(TColor4 left, TColor4 right)
		{
			if (left.r != right.r || left.g != right.g || left.b != right.b || left.a != right.a)
				return true;
			return false;
		}
		public static TColor4 Parse(string strColor)
		{
			TColor4 resColor = new TColor4();
			int index = strColor.IndexOf('=', 0) + 1;
			int endIndex = strColor.IndexOf(' ', index);
			resColor.r = float.Parse(strColor.Substring(index, endIndex - index));

			index = strColor.IndexOf('=', endIndex) + 1;
			endIndex = strColor.IndexOf(' ', index);
			resColor.g = float.Parse(strColor.Substring(index, endIndex - index));

			index = strColor.IndexOf('=', endIndex) + 1;
			endIndex = strColor.IndexOf(' ', index);
			resColor.b = float.Parse(strColor.Substring(index, endIndex - index));

			index = strColor.IndexOf('=', endIndex) + 1;
			endIndex = strColor.Length;
			resColor.a = float.Parse(strColor.Substring(index, endIndex - index));

			return resColor;
		}
		public string ToString()
		{
			return "r=" + r.ToString() + " g=" + g.ToString() + " b=" + b.ToString() + " a=" + a.ToString();
		}
	};

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TPoint2
	{
		public float x, y;

		public static TPoint2 Default
		{
			get { return new TPoint2(0.0f, 0.0f); }
		}

		public TPoint2(float[] _arr) { x = _arr[0]; y = _arr[1]; }
		public TPoint2(float _x, float _y) { x = _x; y = _y; }

		public static TPoint2 operator +(TPoint2 lPoint, TPoint2 rPoint)
		{
			return new TPoint2(lPoint.x + rPoint.x, lPoint.y + rPoint.y);
		}
		public static TPoint2 operator -(TPoint2 lPoint, TPoint2 rPoint)
		{
			return new TPoint2(lPoint.x - rPoint.x, lPoint.y - rPoint.y);
		}

		public static TPoint2 Parse(string strColor)
		{
			TPoint2 resPoint = new TPoint2();
			int index = strColor.IndexOf('=', 0) + 1;
			int endIndex = strColor.IndexOf(' ', index);
			resPoint.x = float.Parse(strColor.Substring(index, endIndex - index));

			index = strColor.IndexOf('=', endIndex) + 1;
			endIndex = strColor.Length;
			resPoint.y = float.Parse(strColor.Substring(index, endIndex - index));

			return resPoint;
		}
		public string ToString()
		{
			return "x=" + x.ToString() + " y=" + y.ToString();
		}
	};

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TPoint3
	{
		public float x, y, z;

		public static TPoint3 Default
		{
			get { return new TPoint3(0.0f, 0.0f, 0.0f); }
		}

		public TPoint3(float _x, float _y, float _z) { x = _x; y = _y; z = _z; }
		public string ToString()
		{
			return "x=" + x.ToString() + " y=" + y.ToString() + " z=" + z.ToString();
		}
	};

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TVertex2
	{
		public float x, y, u, w, r, g, b, a;

		public static TVertex2 Default
		{
			get { return new TVertex2(0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f); }
		}

		public TVertex2(float fX, float fY, float fU, float fW, float fR, float fG, float fB, float fA)
		{
			x = fX; y = fY; u = fU; w = fW;
			r = fR; g = fG; b = fB; a = fA;
		}
	};

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TVertex3
	{
		public float x, y, z, u, w, r, g, b, a;

		public static TVertex3 Default
		{
			get { return new TVertex3(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f); }
		}
		
		public TVertex3(float fX, float fY, float fZ, float fU, float fW, float fR, float fG, float fB, float fA)
		{
			x = fX; y = fY; z = fZ;
			u = fU; w = fW;
			r = fR; g = fG; b = fB; a = fA;
		}
	};

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TRectF
	{
		public float x, y, width, height;

		public static TRectF Default
		{
			get { return new TRectF(0.0f, 0.0f, 0.0f, 0.0f); }
		}
		
		public TRectF(float fX, float fY, float fWidth, float fHeight)
		{
			x = fX;
			y = fY;
			width = fWidth;
			height = fHeight;
		}

		public TRectF(ref TPoint2 stLeftTop, ref TPoint2 stRightBottom)
		{
			x = (stLeftTop.x);
			y = (stLeftTop.y);
			width = (stRightBottom.x - stLeftTop.x);
			height = (stRightBottom.y - stLeftTop.y);
		}

		public bool IntersectRect(TRectF stRect)
		{
			return (x < stRect.x + stRect.width && x + width > stRect.x && y < stRect.y + stRect.height && y + height > stRect.y) ||
					(stRect.x + stRect.width < x && stRect.x > x + width && stRect.y + stRect.height < y && stRect.y > y + height);
		}
		public bool PointInRect(TPoint2 stPoint)
		{
			return stPoint.x > x && stPoint.x < x + width && stPoint.y > y && stPoint.y < y + height;
		}
		public bool InRect(TRectF stRect)
		{
			return	stRect.x < x && stRect.y < y && stRect.x + stRect.width > x + width && stRect.y + stRect.height > y + height;
		}
		public TRectF Intersect(TRectF stRect)
		{
			if (IntersectRect(stRect))
			{
				TRectF result = stRect;

				if (x > stRect.x) result.x = x;
				if (y > stRect.y) result.y = y;

				float rectr = x + width, strectr = stRect.x + stRect.width;
				result.width = (rectr > strectr ? strectr : rectr) - result.x;

				float rectb = y + height, strectb = stRect.y + stRect.height;
				result.height = (rectb > strectb ? strectb : rectb) - result.y;

				return result;            }
			else
				return new TRectF();
		}

		public void Round()
		{
			float fTmp = x + width;
			if(fTmp > (float)(int)fTmp)
				width = (float)(int)(width + 1.0f);
			else
				width = (float)(int)width;
			fTmp = y + height;
			if(fTmp > (float)(int)fTmp)
				height = (float)(int)(height + 1.0f);
			else
				height = (float)(int)height;
			x = (float)(int)x;
			y = (float)(int)y; 
		}
		public string ToString()
		{
			return "x=" + x.ToString() + " y=" + y.ToString() + " width=" + width.ToString() + " height=" + height.ToString();
		}
	};

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TMatrix
	{
		public float _00; public float _01; public float _02; public float _03;
		public float _10; public float _11; public float _12; public float _13;
		public float _20; public float _21; public float _22; public float _23;
		public float _30; public float _31; public float _32; public float _33;
		
		public float this[int n]
		{
			get
			{
				switch (n)
				{
					case 0:  return _00; case 1:  return _01; case 2:  return _02; case 3:  return _03;
					case 4:  return _10; case 5:  return _11; case 6:  return _12; case 7:  return _13;
					case 8:  return _20; case 9:  return _21; case 10: return _22; case 11: return _23;
					case 12: return _30; case 13: return _31; case 14: return _32; case 15: return _33;
					default: throw new ArgumentException();
				}
			}
			set
			{
				switch (n)
				{
					case 0:  _00 = value; break; case 1:  _01 = value; break; case 2:  _02 = value; break; case 3:  _03 = value; break;
					case 4:  _10 = value; break; case 5:  _11 = value; break; case 6:  _12 = value; break; case 7:  _13 = value; break;
					case 8:  _20 = value; break; case 9:  _21 = value; break; case 10: _22 = value; break; case 11: _23 = value; break;
					case 12: _30 = value; break; case 13: _31 = value; break; case 14: _32 = value; break; case 15: _33 = value; break;
					default: throw new ArgumentException();
				}
			}
		}

		public TMatrix(
			float __00, float __01, float __02, float __03,
			float __10, float __11, float __12, float __13,
			float __20, float __21, float __22, float __23,
			float __30, float __31, float __32, float __33)
		{
			_00 = __00; _01 = __01; _02 = __02; _03 = __03;
			_10 = __10; _11 = __11; _12 = __12; _13 = __13;
			_20 = __20; _21 = __21; _22 = __22; _23 = __23;
			_30 = __30; _31 = __31; _32 = __32; _33 = __33;
		}

		public TMatrix(float[] m)
		{
			if (m.Length < 16) throw new ArgumentException();
			_00 = m[0];  _01 = m[1];  _02 = m[2];  _03 = m[3];
			_10 = m[4];  _11 = m[5];  _12 = m[6];  _13 = m[7];
			_20 = m[8];  _21 = m[9];  _22 = m[10]; _23 = m[11];
			_30 = m[12]; _31 = m[13]; _32 = m[14]; _33 = m[15];
		}

		public float[,] To2DArray()
		{
			float[,] res = new float[4, 4];
			for (int i = 0; i < 4; i++)
				for (int j = 0; j < 4; j++)
					res[i, j] = this[i + 4 * j];
			return res;
		}

#if PHYSX
#endif
		public static TMatrix operator -(TMatrix m)
		{
			TMatrix res = new TMatrix();
			for(int i = 0; i < 16; i++)
				res[i] = - m[i];
			return res;
		}

		public static TMatrix operator *(TMatrix left, float right)
		{
			return new TMatrix(
				left._00 * right, left._01 * right, left._02 * right, left._03 * right,
				left._10 * right, left._11 * right, left._12 * right, left._13 * right,
				left._20 * right, left._21 * right, left._22 * right, left._23 * right,
				left._30 * right, left._31 * right, left._32 * right, left._33 * right
				);
		}

		public static TMatrix operator *(float left, TMatrix right)
		{
			return new TMatrix(
				right._00 * left, right._01 * left, right._02 * left, right._03 * left,
				right._10 * left, right._11 * left, right._12 * left, right._13 * left,
				right._20 * left, right._21 * left, right._22 * left, right._23 * left,
				right._30 * left, right._31 * left, right._32 * left, right._33 * left
				);
		}

		public static TMatrix operator /(TMatrix left, float right)
		{
			return new TMatrix(
				left._00 / right, left._01 / right, left._02 / right, left._03 / right,
				left._10 / right, left._11 / right, left._12 / right, left._13 / right,
				left._20 / right, left._21 / right, left._22 / right, left._23 / right,
				left._30 / right, left._31 / right, left._32 / right, left._33 / right
				);
		}

		public static TMatrix operator +(TMatrix left, TMatrix right)
		{
			return new TMatrix(
				left._00 + right._00, left._01 + right._01, left._02 + right._02, left._03 + right._03,
				left._10 + right._10, left._11 + right._11, left._12 + right._12, left._13 + right._13,
				left._20 + right._20, left._21 + right._21, left._22 + right._22, left._23 + right._23,
				left._30 + right._30, left._31 + right._31, left._32 + right._32, left._33 + right._33
				);
		}

		public static TMatrix operator -(TMatrix left, TMatrix right)
		{
			return new TMatrix(
				left._00 - right._00, left._01 - right._01, left._02 - right._02, left._03 - right._03,
				left._10 - right._10, left._11 - right._11, left._12 - right._12, left._13 - right._13,
				left._20 - right._20, left._21 - right._21, left._22 - right._22, left._23 - right._23,
				left._30 - right._30, left._31 - right._31, left._32 - right._32, left._33 - right._33
				);
		}

		public static TMatrix operator +(TMatrix m, float right)
		{
			return new TMatrix(
				m._00 + right, m._01, m._02, m._03,
				m._10, m._11 + right, m._12, m._13,
				m._20, m._21, m._22 + right, m._23,
				m._30, m._31, m._32, m._33 + right
				);
		}

		public static TMatrix operator +(float left, TMatrix m)
		{
			return new TMatrix(
				m._00 + left, m._01, m._02, m._03,
				m._10, m._11 + left, m._12, m._13,
				m._20, m._21, m._22 + left, m._23,
				m._30, m._31, m._32, m._33 + left
				);
		}

		public static TMatrix operator -(TMatrix m, float right)
		{
			return new TMatrix(
				m._00 - right, m._01, m._02, m._03,
				m._10, m._11 - right, m._12, m._13,
				m._20, m._21, m._22 - right, m._23,
				m._30, m._31, m._32, m._33 - right
				);
		}

		public static TMatrix operator -(float left, TMatrix m)
		{
			return new TMatrix(
				-m._00 + left, -m._01, -m._02, -m._03,
				-m._10, -m._11 + left, -m._12, -m._13,
				-m._20, -m._21, -m._22 + left, -m._23,
				-m._30, -m._31, -m._32, -m._33 + left
				);
		}

		public static TMatrix operator *(TMatrix left, TMatrix right)
		{
			TMatrix product = new TMatrix();
			product._00 = left._00 * right._00 + left._01 * right._10 + left._02 * right._20 + left._03 * right._30;
			product._10 = left._10 * right._00 + left._11 * right._10 + left._12 * right._20 + left._13 * right._30;
			product._20 = left._20 * right._00 + left._21 * right._10 + left._22 * right._20 + left._23 * right._30;
			product._30 = left._30 * right._00 + left._31 * right._10 + left._32 * right._20 + left._33 * right._30;
			product._01 = left._00 * right._01 + left._01 * right._11 + left._02 * right._21 + left._03 * right._31;
			product._11 = left._10 * right._01 + left._11 * right._11 + left._12 * right._21 + left._13 * right._31;
			product._21 = left._20 * right._01 + left._21 * right._11 + left._22 * right._21 + left._23 * right._31;
			product._31 = left._30 * right._01 + left._31 * right._11 + left._32 * right._21 + left._33 * right._31;
			product._02 = left._00 * right._02 + left._01 * right._12 + left._02 * right._22 + left._03 * right._32;
			product._12 = left._10 * right._02 + left._11 * right._12 + left._12 * right._22 + left._13 * right._32;
			product._22 = left._20 * right._02 + left._21 * right._12 + left._22 * right._22 + left._23 * right._32;
			product._32 = left._30 * right._02 + left._31 * right._12 + left._32 * right._22 + left._33 * right._32;
			product._03 = left._00 * right._03 + left._01 * right._13 + left._02 * right._23 + left._03 * right._33;
			product._13 = left._10 * right._03 + left._11 * right._13 + left._12 * right._23 + left._13 * right._33;
			product._23 = left._20 * right._03 + left._21 * right._13 + left._22 * right._23 + left._23 * right._33;
			product._33 = left._30 * right._03 + left._31 * right._13 + left._32 * right._23 + left._33 * right._33;

			return product;
		}

		public static TMatrix MatrixIdentity
		{
			get
			{
				return new TMatrix(
					1.0f, 0.0f, 0.0f, 0.0f,
					0.0f, 1.0f, 0.0f, 0.0f,
					0.0f, 0.0f, 1.0f, 0.0f,
					0.0f, 0.0f, 0.0f, 1.0f);
			}
		}
		
		public static TMatrix MatrixInverse(TMatrix stMatrix)
		{
			return stMatrix.MatrixInverse();
		}

		public TMatrix MatrixInverse()
		{
			float[][] mat = new float[4][];
			for (int i = 0; i < 4; i++)
			{
				mat[i] = new float[8]
				{
					this[4 * i],     this[4 * i + 1],
					this[4 * i + 2], this[4 * i + 3],
					(i == 0) ? 1.0f : 0.0f, (i == 1) ? 1.0f : 0.0f,
					(i == 2) ? 1.0f : 0.0f, (i == 3) ? 1.0f : 0.0f
				};
			}
			int r;
			for (int i = 0; i < 4; ++i)
			{
				int row_num = i;
				float major = (float)Math.Abs(mat[i][i]);
				for (r = i + 1; r < 4; ++r)
				{
					float cur_ABS = (float)Math.Abs(mat[r][i]);
					if (cur_ABS > major)
					{
						major = cur_ABS;
						row_num = r;
					}
				}
				if (row_num != i)
				{
					float[] row = mat[i];
					mat[i] = mat[row_num];
					mat[row_num] = row;
				}
				for (r = i + 1; r < 4; ++r)
				{
					float factor = mat[r][i] / mat[i][i];
					if (factor == 0.0f) break;
					for (int c = i; c < 8; ++c)
						mat[r][c] -= factor * mat[i][c];
				}
			}
			for (int i = 3; i > 0; --i)
				for (r = 0; r < i; ++r)
				{
					float factor = mat[r][i] / mat[i][i];
					for (int c = 4; c < 8; ++c)
						mat[r][c] -= factor * mat[i][c];
				}
			return new TMatrix(
				mat[0][4] / mat[0][0], mat[0][5] / mat[0][0], mat[0][6] / mat[0][0], mat[0][7] / mat[0][0],
				mat[1][4] / mat[1][1], mat[1][5] / mat[1][1], mat[1][6] / mat[1][1], mat[1][7] / mat[1][1],
				mat[2][4] / mat[2][2], mat[2][5] / mat[2][2], mat[2][6] / mat[2][2], mat[2][7] / mat[2][2],
				mat[3][4] / mat[3][3], mat[3][5] / mat[3][3], mat[3][6] / mat[3][3], mat[3][7] / mat[3][3]);
		}

		public TMatrix MatrixTranspose()
		{
			return new TMatrix(
				_00, _10, _20, _30,
				_01, _11, _21, _31,
				_02, _12, _22, _32,
				_03, _13, _23, _33
				);
		}
		
		public static TMatrix MatrixTranspose(TMatrix stMatrix)
		{
			return new TMatrix(
				stMatrix._00, stMatrix._10, stMatrix._20, stMatrix._30,
				stMatrix._01, stMatrix._11, stMatrix._21, stMatrix._31,
				stMatrix._02, stMatrix._12, stMatrix._22, stMatrix._32,
				stMatrix._03, stMatrix._13, stMatrix._23, stMatrix._33
				);
		}
		
		public static TMatrix MatrixScale(TPoint3 stVec)
		{
			return new TMatrix(
				stVec.x, 0.0f, 0.0f, 0.0f,
				0.0f, stVec.y, 0.0f, 0.0f,
				0.0f, 0.0f, stVec.z, 0.0f,
				0.0f, 0.0f, 0.0f, 1.0f);
		}
		
		public static TMatrix MatrixTranslate(TPoint3 stVec)
		{
			return new TMatrix(
				1.0f, 0.0f, 0.0f, 0.0f,
				0.0f, 1.0f, 0.0f, 0.0f,
				0.0f, 0.0f, 1.0f, 0.0f,
				stVec.x, stVec.y, stVec.z, 1.0f);
		}

		public static TMatrix MatrixRotate(float fAngle, TPoint3 stAxes)
		{
			float
				axis_norm = (float)Math.Sqrt(stAxes.x * stAxes.x + stAxes.y * stAxes.y + stAxes.z * stAxes.z),
				x = stAxes.x / axis_norm,
				y = stAxes.y / axis_norm,
				z = stAxes.z / axis_norm,
				sin_angle = (float)Math.Sin(fAngle * 3.1415926535897932 / 180.0f),
				cos_angle = (float)Math.Cos(fAngle * 3.1415926535897932 / 180.0f);
			return new TMatrix(
				(1.0f - x * x) * cos_angle + x * x,			z * sin_angle + x * y * (1.0f - cos_angle),	x * z * (1.0f - cos_angle) - y * sin_angle,	0.0f,
				x * y * (1.0f - cos_angle) - z * sin_angle,	(1.0f - y * y) * cos_angle + y * y,			y * z * (1.0f - cos_angle) + x * sin_angle,	0.0f,
				x * z * (1.0f - cos_angle) + y * sin_angle,	y * z * (1.0f - cos_angle) - x * sin_angle,	(1.0f - z * z) * cos_angle + z * z,			0.0f,
				0.0f,										0.0f,										0.0f,										1.0f);
		}

		public static TMatrix MatrixBillboard(TMatrix stMatrix)
		{
			return new TMatrix(
				1.0f,			0.0f,			0.0f,			stMatrix._03,
				0.0f,			1.0f,			0.0f,			stMatrix._13,
				0.0f,			0.0f,			1.0f,			stMatrix._23,
				stMatrix._30,   stMatrix._31,	stMatrix._32,	stMatrix._33);
		}
	};

	public class TTransformStack : Stack<TMatrix>
	{
		public TTransformStack(ref TMatrix base_transform)
		{
			Push(base_transform);
		}
		public void Push()
		{
			base.Push(Peek());
		}
		new public void Pop()
		{
			base.Pop();
		}
		public TMatrix Top()
		{
			return Peek();
		}

		public void Mult1(ref TMatrix transform)
		{
			TMatrix matrix = Top();
			matrix *= transform;
		}
		public void Mult2(ref TMatrix transform)
		{
			TMatrix matrix = Top();
			matrix = transform * matrix;
		}
	};

	
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TMouseStates
	{
		public Int32 iX, iY;
		public Int32 iDeltaX, iDeltaY, iDeltaWheel;
		[MarshalAs(UnmanagedType.U1)] public bool bLeftButton;
		[MarshalAs(UnmanagedType.U1)] public bool bRightButton;
		[MarshalAs(UnmanagedType.U1)] public bool bMiddleButton;
	};

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TKeyBoardStates 
	{
		[MarshalAs(UnmanagedType.U1)] public bool bCapsLock;
		[MarshalAs(UnmanagedType.U1)] public bool bShiftL;
		[MarshalAs(UnmanagedType.U1)] public bool bShiftR;
		[MarshalAs(UnmanagedType.U1)] public bool bCtrlL;
		[MarshalAs(UnmanagedType.U1)] public bool bCtrlR;
		[MarshalAs(UnmanagedType.U1)] public bool bAltL;
		[MarshalAs(UnmanagedType.U1)] public bool bAltR;
	};

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TJoystickStates
	{
		public uint uiBtnsCount;	/**< Count of available joystick buttons. */
		
// not allowed to use bool[32] // phomm  
	  
		public bool[] bButtons;	/**< Array of joystick buttons states (pressed or not). */
		public int iXAxes;		    /**< X-axis position. Value varies -100 to 100. */
		public int iYAxes;		    /**< Y-axis position. Value varies -100 to 100. */
		public int iZAxes;		    /**< Z-axis position. Value varies -100 to 100. */
		public int iRAxes;			/**< Current position of the rudder or fourth joystick axis. Value varies -100 to 100. */
		public int iUAxes;			/**< Current fifth axis position. Value varies -100 to 100. */
		public int iVAxes;			/**< Current sixth axis position. Value varies -100 to 100. */
		public int iPOV;			/**< Point-Of-View direction. */

	};

	public enum E_KEYBOARD_KEY_CODES
	{
		KEY_ESCAPE          = 0x01,		// Escape
		KEY_TAB             = 0x0F,		// Tab
		KEY_GRAVE           = 0x29,     // accent grave "~"
		KEY_CAPSLOCK        = 0x3A,     // Caps Lock
		KEY_BACKSPACE       = 0x0E,     // Backspace
		KEY_RETURN          = 0x1C,     // Enter
		KEY_SPACE           = 0x39,		// Space
		KEY_SLASH           = 0x35,     // "/" 
		KEY_BACKSLASH       = 0x2B,		// "\"

		KEY_SYSRQ           = 0xB7,     // PtrScr (SysRq)
		KEY_SCROLL          = 0x46,     // Scroll Lock
		KEY_PAUSE           = 0xC5,     // Pause

		KEY_INSERT          = 0xD2,     // Insert
		KEY_DELETE          = 0xD3,     // Delete
		KEY_HOME            = 0xC7,     // Home
		KEY_END             = 0xCF,     // End
		KEY_PGUP            = 0xC9,     // PgUp
		KEY_PGDN            = 0xD1,     // PgDn

		KEY_LSHIFT          = 0x2A,		// Left Shift
		KEY_RSHIFT          = 0x36,		// Right Shift
		KEY_LALT			= 0x38,     // Left Alt
		KEY_RALT            = 0xB8,     // Right Alt
		KEY_LWIN            = 0xDB,     // Left Windows key 
		KEY_RWIN            = 0xDC,     // Right Windows key 
		KEY_LCONTROL        = 0x1D,		// Left Control
		KEY_RCONTROL        = 0x9D,		// Right Control

		KEY_UP              = 0xC8,     // UpArrow
		KEY_RIGHT           = 0xCD,     // RightArrow
		KEY_LEFT            = 0xCB,     // LeftArrow
		KEY_DOWN            = 0xD0,     // DownArrow

		KEY_1               = 0x02,
		KEY_2               = 0x03,
		KEY_3               = 0x04,
		KEY_4               = 0x05,
		KEY_5               = 0x06,
		KEY_6               = 0x07,
		KEY_7               = 0x08,
		KEY_8               = 0x09,
		KEY_9               = 0x0A,
		KEY_0               = 0x0B,

		KEY_F1              = 0x3B,
		KEY_F2              = 0x3C,
		KEY_F3              = 0x3D,
		KEY_F4              = 0x3E,
		KEY_F5              = 0x3F,
		KEY_F6              = 0x40,
		KEY_F7              = 0x41,
		KEY_F8              = 0x42,
		KEY_F9              = 0x43,
		KEY_F10             = 0x44,
		KEY_F11             = 0x57,
		KEY_F12             = 0x58,

		KEY_Q               = 0x10,
		KEY_W               = 0x11,
		KEY_E               = 0x12,
		KEY_R               = 0x13,
		KEY_T               = 0x14,
		KEY_Y               = 0x15,
		KEY_U               = 0x16,
		KEY_I               = 0x17,
		KEY_O               = 0x18,
		KEY_P               = 0x19,
		KEY_A               = 0x1E,
		KEY_S               = 0x1F,
		KEY_D               = 0x20,
		KEY_F               = 0x21,
		KEY_G               = 0x22,
		KEY_H               = 0x23,
		KEY_J               = 0x24,
		KEY_K               = 0x25,
		KEY_L               = 0x26,
		KEY_Z               = 0x2C,
		KEY_X               = 0x2D,
		KEY_C               = 0x2E,
		KEY_V               = 0x2F,
		KEY_B               = 0x30,
		KEY_N               = 0x31,
		KEY_M               = 0x32,

		KEY_MINUS           = 0x0C,		// "-"
		KEY_PLUS            = 0x0D,		// "+"
		KEY_LBRACKET        = 0x1A,		// "["
		KEY_RBRACKET        = 0x1B,     // "]" 

		KEY_SEMICOLON       = 0x27,     // ";"
		KEY_APOSTROPHE      = 0x28,     // '"' 

		KEY_COMMA           = 0x33,		// ","
		KEY_PERIOD          = 0x34,		// "."

		KEY_NUMPAD0         = 0x52,
		KEY_NUMPAD1         = 0x4F,
		KEY_NUMPAD2         = 0x50,
		KEY_NUMPAD3         = 0x51,
		KEY_NUMPAD4         = 0x4B,
		KEY_NUMPAD5         = 0x4C,
		KEY_NUMPAD6         = 0x4D,
		KEY_NUMPAD7         = 0x47,
		KEY_NUMPAD8         = 0x48,
		KEY_NUMPAD9         = 0x49,
		KEY_NUMPADPERIOD    = 0x53,		// "." on numpad
		KEY_NUMPADENTER     = 0x9C,		// Enter on numpad
		KEY_NUMPADSTAR      = 0x37,		// "*" on numpad
		KEY_NUMPADPLUS      = 0x4E,		// "+" on numpad
		KEY_NUMPADMINUS     = 0x4A,		// "-" on numpad
		KEY_NUMPADSLASH     = 0xB5,		// "/" on numpad
		KEY_NUMLOCK         = 0x45,		// Num Lock on numpad
	};

	public delegate TResult Func<in T, out TResult>(T arg);
	public static class Clonner
	{
		static Func<object, object> MemberwiseClone =
		Delegate.CreateDelegate
		(
		typeof(Func<object, object>),
		typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic)
		)
		as Func<object, object>;

		public static object CloneStyleProvider(object obj)
		{
			if (object.ReferenceEquals(obj, null))
				return null;
			else
			{
				Type tyobj = obj.GetType(); //узнаем тип объекта
				if (tyobj.IsPrimitive) return obj; //если тип является примитивом (int, double и т.п.) то нет смысла копаться в его внутренностях
				else
				{
					object clone = MemberwiseClone(obj);
					//глубококлонируем все поля объекта
					foreach (var field in tyobj.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
					{
						if (field.Name == "_strName")
							field.SetValue(clone, DeepClone(field.GetValue(clone)));
						else
							field.SetValue(clone, null);
					}
					return clone;
				}
			}
			
		}

		public static object DeepClone(object obj)
		{
			if (object.ReferenceEquals(obj, null)) //Если объект ~ null, то и клон его - null
				return null;
			else
			{
				if (obj is string)
					return string.Copy(obj as string);
				else if (obj is Array) //Если объект - массив, необходимо поочередно глубококлонировать его элементы
				{
					Array array = (obj as Array).Clone() as Array;
					
					int lenght = (obj as Array).Length;
					if (lenght != 0)
					{
						foreach (var indx in new Indexer(array))
						{
							Type arrayType = array.GetValue(indx).GetType();
							if (arrayType.IsPrimitive)
							{
								int rank = array.Rank;
								int[] sizes = new int[rank];
								for(int i = 0; i < rank; ++i)
									sizes[i] = array.GetLength(i);
								Array resArray = Array.CreateInstance(arrayType, sizes);
								Array.Copy(obj as Array, resArray, lenght);
								return resArray;
							}
							break;
						}
					}
					
					switch (array.Rank)
					{
						case 1: // T[]
							{
								int bi = array.GetLowerBound(0), ei = array.GetUpperBound(0) + 1;
								for (int i = bi; i < ei; ++i)
									array.SetValue(DeepClone(array.GetValue(i)), i);
								/*
								if (obj.GetType().FullName == "System.Byte[]")
								{
									
									int lenght = (obj as Array).Length;
									IntPtr ptr = Marshal.AllocHGlobal(lenght);

									Marshal.Copy(obj as byte[], 0, ptr, lenght);
									byte[] bytes = new byte[lenght];

									Marshal.Copy(ptr, bytes, 0, lenght);
									Marshal.FreeHGlobal(ptr);
									return bytes;
									
								}
								else
								{
									int bi = array.GetLowerBound(0), ei = array.GetUpperBound(0) + 1;
									for (int i = bi; i < ei; i++)
										array.SetValue(DeepClone(array.GetValue(i)), i);
								}
								*/
								break;
							}
						case 2: // T[,]
							{
								int bi = array.GetLowerBound(0), ei = array.GetUpperBound(0) + 1;
								int bj = array.GetLowerBound(1), ej = array.GetUpperBound(1) + 1;
								for (int i = bi; i < ei; ++i)
									for (int j = bj; j < ej; ++j)
										array.SetValue(DeepClone(array.GetValue(i, j)), i, j);
								break;
							}
						case 3: // T[,,]
							{
								int bi = array.GetLowerBound(0), ei = array.GetUpperBound(0) + 1;
								int bj = array.GetLowerBound(1), ej = array.GetUpperBound(1) + 1;
								int bk = array.GetLowerBound(2), ek = array.GetUpperBound(2) + 1;
								for (int i = bi; i < ei; ++i)
									for (int j = bj; j < ej; ++j)
										for (int k = bk; k < ek; ++k)
											array.SetValue(DeepClone(array.GetValue(i, j, k)), i, j, k);
								break;
							}
						default: // T[,,...,] - более трех размерностей
							{
								//для прохода по всем индексам используем специальный индекс-итератор
								foreach (var indx in new Indexer(array)) array.SetValue(DeepClone(array.GetValue(indx)), indx);
								break;
							}
					}
					
					return array;
				}
				else // объект не null, не строка и не массив
				{
					Type tyobj = obj.GetType();
					if (tyobj.FullName == "System.__ComObject")
						return obj;
					if (tyobj.FullName == "GuiEditor.CGUIEngine")
						return obj;
					else if (tyobj.IsPrimitive)  //если тип является примитивом (int, double и т.п.) то нет смысла копаться в его внутренностях
						return obj;
					else
					{
						object clone = MemberwiseClone(obj);
						//глубококлонируем все поля объекта
						foreach (var field in tyobj.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
						{
							if (field.Name == "_pStyle")
							{
								field.SetValue(clone, CloneStyleProvider(field.GetValue(clone)));
							}
							else if (field.Name == "_pParent" || field.Name == "pPicture")
								field.SetValue(clone, null);
							else if ((tyobj.FullName == "GuiEditor.CDCForm" || tyobj.FullName == "GuiEditor.CDCPanel") && field.Name == "pChildren")
								field.SetValue(clone, null);
							else
							{
								//field.SetValue(clone, DeepClone(field.GetValue(clone)));
								object value = DeepClone(field.GetValue(clone));
								field.SetValue(clone, value);
							}
						}
						return clone;
					}
				}
			}
		}
	}

	//[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct Indexer : IEnumerable<int[]>
	{
		public int rank;
		public int[] LoverBownds;
		public int[] UpperBownds;

		public Indexer(Array array)
		{
			this.rank = array.Rank;
			this.LoverBownds = new int[rank];
			this.UpperBownds = new int[rank];
			for (int i = 0; i < this.rank; i++)
			{
				this.LoverBownds[i] = array.GetLowerBound(i);
				this.UpperBownds[i] = array.GetUpperBound(i);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator)GetEnumerator();
		}
		
		public IEnumerator<int[]> GetEnumerator()
		{
			int[] CurrentIndexs = this.LoverBownds.Clone() as int[];
			for (int i = this.rank - 2; ; )
			{
				for (; ++i < this.rank; )
					CurrentIndexs[i] = this.LoverBownds[i];
				for (--i; CurrentIndexs[i] <= this.UpperBownds[i]; CurrentIndexs[i]++)
					yield return CurrentIndexs;
				for (; --i != -1 && CurrentIndexs[i] == this.UpperBownds[i]; ) ;

				if (i < 0) break;
				else CurrentIndexs[i]++;
			}
		}
		//public override string ToString()
		//{
		// StringBuilder sb = new StringBuilder(16);
		// sb.Append("[ { ");
		// foreach (var low in this.LoverBownds) { sb.Append(low); sb.Append(' '); }
		// sb.Append("} <= { ");
		// foreach (var up in this.UpperBownds) { sb.Append(up); sb.Append(' '); }
		// sb.Append("} ]");
		// return sb.ToString();
		//}
	}

}