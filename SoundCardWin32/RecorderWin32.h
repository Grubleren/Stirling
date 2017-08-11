#ifndef SOUNDCARDWIN32_H
#define SOUNDCARDWIN32_H

#include "stdafx.h"

#define DLLEXPORT __declspec(dllexport)
#define DLLCALL __cdecl

#ifdef __cplusplus
extern "C" {
#endif

	DLLEXPORT BOOL DLLCALL Start(int bufferLength, int fs);
	DLLEXPORT void DLLCALL Stop();
	DLLEXPORT int DLLCALL Wait(short* pBuffer);

#ifdef __cplusplus
}
#endif
#endif