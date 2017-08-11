
#include "stdafx.h"

CRITICAL_SECTION CriticalSection;

LPDIRECTSOUNDCAPTURE8 pDSC;
LPDIRECTSOUNDCAPTUREBUFFER  pDSCBuffer;
DSCBUFFERDESC DSCBufferDesc;
WAVEFORMATEX wave;

HRESULT result;

int nBuffer = 10;
int bufferLength;
int gap;
DWORD i0;
DWORD i1;
DWORD delay;

DLLEXPORT BOOL DLLCALL Start(int bufLen, int fs)
{
	result = DirectSoundCaptureCreate8(NULL, &pDSC, NULL);

	if (result != S_OK)
		return FALSE;

	wave.wFormatTag = WAVE_FORMAT_PCM;
	wave.nSamplesPerSec = fs;
	wave.nChannels = 2;
	wave.nBlockAlign = 2 * wave.nChannels;
	wave.nAvgBytesPerSec = fs * wave.nBlockAlign;
	wave.wBitsPerSample = 8 * wave.nBlockAlign / wave.nChannels;
	wave.cbSize = sizeof(WAVEFORMATEX);

	bufferLength = bufLen * wave.nBlockAlign;

	DSCBufferDesc.dwBufferBytes = bufferLength * nBuffer;
	DSCBufferDesc.dwFlags = 0;
	DSCBufferDesc.dwFXCount = 0;
	DSCBufferDesc.dwReserved = 0;
	DSCBufferDesc.dwSize = sizeof(DSCBUFFERDESC);
	DSCBufferDesc.lpDSCFXDesc = NULL;
	DSCBufferDesc.lpwfxFormat = &wave;

	result = pDSC->CreateCaptureBuffer(&DSCBufferDesc, &pDSCBuffer, NULL);

	InitializeCriticalSectionAndSpinCount(&CriticalSection, 0x00000400);

	pDSCBuffer->Start(DSCBSTART_LOOPING);

	i0 = 0;
	i1 = bufferLength - 1;
	gap = 0;
	delay = 1000 * bufferLength / (wave.nSamplesPerSec * wave.nBlockAlign) / 10;

	return true;
}

DLLEXPORT int DLLCALL Wait(short* pBuffer)
{

	LPVOID pAudioPtr1;
	DWORD audioBytes1;
	LPVOID pAudioPtr2;
	DWORD audioBytes2;

	DWORD readPos;
	DWORD capturePos;

	bool running = true;

	do
	{
		Sleep(delay);

		if (pDSCBuffer == NULL)
			return 0;
		result = pDSCBuffer->GetCurrentPosition(&capturePos, &readPos);

		int diff = readPos - i0;
		diff = diff >= 0 ? diff : diff + DSCBufferDesc.dwBufferBytes;
		if (diff > (nBuffer - 2) * bufferLength)
			gap++;

		if (capturePos < i0 && (readPos <= capturePos || readPos > i1) || capturePos > i1 && readPos > i1)
		{
			result = pDSCBuffer->Lock(i0, bufferLength, &pAudioPtr1, &audioBytes1, &pAudioPtr2, &audioBytes2, 0);
			memcpy(pBuffer, pAudioPtr1, audioBytes1);
			memcpy(pBuffer + audioBytes1, pAudioPtr2, audioBytes2);
			result = pDSCBuffer->Unlock(pAudioPtr1, audioBytes1, pAudioPtr2, audioBytes2);

			i0 += bufferLength;
			i1 += bufferLength;
			i0 = i0 >= DSCBufferDesc.dwBufferBytes ? 0 : i0;
			i1 = i0 == 0 ? bufferLength - 1 : i1;
			running = false;
		}

	} while (running);

	return gap;
}

DLLEXPORT void DLLCALL Stop()
{

}
