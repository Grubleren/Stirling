@echo off
rem ------------------------------------------------------------------------------
rem 
rem  (C) Brüel & Kjær Sound And Vibration A/S
rem 
rem  Description  : Publishes 
rem  
rem ------------------------------------------------------------------------------

echo Press a key to publish the SystemInfo component, or Ctrl-C to cancel.
pause > NUL:

rem Set default directory to location of this batch file
cd %~d0%~p0

set DirFile="..\Publish"

if not exist %DirFile% mkdir %DirFile%

call :xcopym /y /r /f "Machine\bin\release\Machine.exe" %DirFile%
call :xcopym /y /r /f "Machine\bin\release\Machine.pdb" %DirFile%

call :xcopym /y /r /f "FftAdapter\bin\release\FftAdapter.dll" %DirFile%
call :xcopym /y /r /f "FftAdapter\bin\release\FftAdapter.pdb" %DirFile%

call :xcopym /y /r /f "Upsampling\bin\release\Upsampling.dll" %DirFile%
call :xcopym /y /r /f "Upsampling\bin\release\Upsampling.pdb" %DirFile%

call :xcopym /y /r /f "FftAnalysis\bin\release\FftAnalysis.dll" %DirFile%
call :xcopym /y /r /f "FftAnalysis\bin\release\FftAnalysis.pdb" %DirFile%

call :xcopym /y /r /f "CpbAnalysis\bin\release\CpbAnalysis.dll" %DirFile%
call :xcopym /y /r /f "CpbAnalysis\bin\release\CpbAnalysis.pdb" %DirFile%

call :xcopym /y /r /f "Detector\bin\release\Detector.dll" %DirFile%
call :xcopym /y /r /f "Detector\bin\release\Detector.pdb" %DirFile%

call :xcopym /y /r /f "DetectorBank\bin\release\DetectorBank.dll" %DirFile%
call :xcopym /y /r /f "DetectorBank\bin\release\DetectorBank.pdb" %DirFile%

call :xcopym /y /r /f "BBDetector\bin\release\BBDetector.dll" %DirFile%
call :xcopym /y /r /f "BBDetector\bin\release\BBDetector.pdb" %DirFile%

call :xcopym /y /r /f  "Afilter\bin\release\Afilter.dll" %DirFile%
call :xcopym /y /r /f  "Afilter\bin\release\Afilter.pdb" %DirFile%

call :xcopym /y /r /f  "display\bin\release\display.dll" %DirFile%
call :xcopym /y /r /f  "display\bin\release\display.pdb" %DirFile%

call :xcopym /y /r /f "Measure\bin\release\Measure.dll" %DirFile%
call :xcopym /y /r /f "Measure\bin\release\Measure.pdb" %DirFile%

call :xcopym /y /r /f "SoundCard\bin\release\SoundCard.dll" %DirFile%
call :xcopym /y /r /f "SoundCard\bin\release\SoundCard.pdb" %DirFile%

call :xcopym /y /r /f "PlayBack\bin\release\PlayBack.dll" %DirFile%
call :xcopym /y /r /f "PlayBack\bin\release\PlayBack.pdb" %DirFile%

call :xcopym /y /r /f "Generator\bin\release\Generator.dll" %DirFile%
call :xcopym /y /r /f "Generator\bin\release\Generator.pdb" %DirFile%

call :xcopym /y /r /f "Butterworth\bin\release\Butterworth.dll" %DirFile%
call :xcopym /y /r /f "Butterworth\bin\release\Butterworth.pdb" %DirFile%

call :xcopym /y /r /f "Generators\bin\release\Generators.dll" %DirFile%
call :xcopym /y /r /f "Generators\bin\release\Generators.pdb" %DirFile%

call :xcopym /y /r /f "DisplayComponent\bin\release\DisplayComponent.dll" %DirFile%
call :xcopym /y /r /f "DisplayComponent\bin\release\DisplayComponent.pdb" %DirFile%

call :xcopym /y /r /f "IppWrapper\bin\release\IppWrapper.dll" %DirFile%
call :xcopym /y /r /f "IppWrapper\bin\release\IppWrapper.pdb" %DirFile%

call :xcopym /y /r /f "FilterBank\bin\release\FilterBank.dll" %DirFile%
call :xcopym /y /r /f "FilterBank\bin\release\FilterBank.pdb" %DirFile%

call :xcopym /y /r /f "WaveIO\bin\release\WaveIO.dll" %DirFile%
call :xcopym /y /r /f "WaveIO\bin\release\WaveIO.pdb" %DirFile%




goto :eof

:xcopym 
  xcopy %1 %2 %3 %4 %5
  IF ErrorLevel 1 GOTO :Err
  goto :eof   
        
:Err     
   pause