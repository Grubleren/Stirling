#ifndef SOUNDCARDWIN32_H
#define SOUNDCARDWIN32_H

#include "mmsystem.h"
#include "stdlib.h"

#define DLLEXPORT __declspec(dllexport)
#define DLLCALL __cdecl

void CALLBACK waveInProc(HWAVEIN hwi, UINT uMsg, DWORD_PTR dwInstance, DWORD_PTR dwParam1, DWORD_PTR dwParam2);
void CALLBACK waveOutProc(HWAVEOUT hwo, UINT uMsg, DWORD_PTR dwInstance, DWORD_PTR dwParam1, DWORD_PTR dwParam2);

#ifdef __cplusplus
extern "C" {
#endif

	DLLEXPORT void DLLCALL Start();
	DLLEXPORT void DLLCALL Stop();
	DLLEXPORT short* DLLCALL Wait();

#ifdef __cplusplus
}
#endif
#endif