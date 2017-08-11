// SoundCardWin32.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "SoundCardWin32.h"

PWAVEHDR pWaveHdrIn1;
PWAVEHDR pWaveHdrIn2;

PBYTE pBufferIn1;
PBYTE pBufferIn2;

HWAVEIN hwi = NULL;

MMRESULT result;

int bufferLength = 2400;
short* inputBuffer;

HANDLE event;

DLLEXPORT void DLLCALL Start()
{
	WAVEFORMATEX waveform;

	waveform.wFormatTag = WAVE_FORMAT_PCM;
	waveform.nChannels = 1;
	waveform.nSamplesPerSec = 48000;
	waveform.nAvgBytesPerSec = 96000;
	waveform.nBlockAlign = 2;
	waveform.wBitsPerSample = 16;
	waveform.cbSize = 0;

	result = waveInOpen(&hwi, WAVE_MAPPER, &waveform, (DWORD_PTR)waveInProc, 0, CALLBACK_FUNCTION);

	pBufferIn1 = (PBYTE)malloc(bufferLength * 2);
	pBufferIn2 = (PBYTE)malloc(bufferLength * 2);

	pWaveHdrIn1 = (PWAVEHDR)malloc(sizeof (WAVEHDR));
	pWaveHdrIn2 = (PWAVEHDR)malloc(sizeof (WAVEHDR));


	pWaveHdrIn1->lpData = (LPSTR)pBufferIn1;
	pWaveHdrIn1->dwBufferLength = bufferLength * 2;
	pWaveHdrIn1->dwBytesRecorded = 0;
	pWaveHdrIn1->dwUser = 0;
	pWaveHdrIn1->dwFlags = 0;
	pWaveHdrIn1->dwLoops = 1;
	pWaveHdrIn1->lpNext = NULL;
	pWaveHdrIn1->reserved = 0;


	pWaveHdrIn2->lpData = (LPSTR)pBufferIn2;
	pWaveHdrIn2->dwBufferLength = bufferLength * 2;
	pWaveHdrIn2->dwBytesRecorded = 0;
	pWaveHdrIn2->dwUser = 0;
	pWaveHdrIn2->dwFlags = 0;
	pWaveHdrIn2->dwLoops = 1;
	pWaveHdrIn2->lpNext = NULL;
	pWaveHdrIn2->reserved = 0;

	result = waveInPrepareHeader(hwi, pWaveHdrIn1, sizeof (WAVEHDR));
	result = waveInPrepareHeader(hwi, pWaveHdrIn2, sizeof (WAVEHDR));

	result = waveInAddBuffer(hwi, pWaveHdrIn1, sizeof (WAVEHDR));
	result = waveInAddBuffer(hwi, pWaveHdrIn2, sizeof (WAVEHDR));

	result = waveInStart(hwi);

	event = CreateEvent(
		NULL,               // default security attributes
		FALSE,               // auto-reset event
		FALSE,              // initial state is nonsignaled
		TEXT("Event")  // object name
		);
}

DLLEXPORT short* DLLCALL Wait()
{
	WaitForSingleObject(event, 10000);

	return inputBuffer;
}

void CALLBACK waveInProc(HWAVEIN hwi, UINT uMsg, DWORD_PTR dwInstance, DWORD_PTR dwParam1, DWORD_PTR dwParam2)
{
	switch (uMsg)
	{
	case MM_WIM_OPEN:		// Wave input opened
		break;
	case MM_WIM_DATA:		// Wave input data available

		inputBuffer = (short*)((PWAVEHDR)dwParam1)->lpData;

		result = waveInAddBuffer(hwi, (PWAVEHDR)dwParam1, sizeof (WAVEHDR));

		SetEvent(event);

		break;

	case MM_WIM_CLOSE:

		free(pBufferIn1);
		free(pBufferIn2);

		free(pWaveHdrIn1);
		free(pWaveHdrIn2);

		hwi = NULL;

		break;
	}
}

DLLEXPORT void DLLCALL Stop()
{
	if (hwi != NULL)
	{
		waveInStop(hwi);
		waveInClose(hwi);
	}
}
