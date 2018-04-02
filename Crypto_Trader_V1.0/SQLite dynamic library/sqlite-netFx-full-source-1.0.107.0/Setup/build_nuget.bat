@ECHO OFF

::
:: build_nuget.bat --
::
:: Wrapper Tool for NuGet
::
:: Written by Joe Mistachkin.
:: Released to the public domain, use at your own risk!
::

SETLOCAL

REM SET __ECHO=ECHO
REM SET __ECHO2=ECHO
REM SET __ECHO3=ECHO
IF NOT DEFINED _AECHO (SET _AECHO=REM)
IF NOT DEFINED _CECHO (SET _CECHO=REM)
IF NOT DEFINED _VECHO (SET _VECHO=REM)

%_AECHO% Running %0 %*

SET DUMMY2=%1

IF DEFINED DUMMY2 (
  GOTO usage
)

SET ROOT=%~dp0\..
SET ROOT=%ROOT:\\=\%

%_VECHO% Root = '%ROOT%'

SET TOOLS=%~dp0
SET TOOLS=%TOOLS:~0,-1%

%_VECHO% Tools = '%TOOLS%'

IF NOT DEFINED NUGET (
  SET NUGET=NuGet2.exe
)

%_VECHO% NuGet = '%NUGET%'

CALL :fn_ResetErrorLevel

IF NOT EXIST "%ROOT%\Setup\Output" (
  %__ECHO% MKDIR "%ROOT%\Setup\Output"

  IF ERRORLEVEL 1 (
    ECHO Could not create directory "%ROOT%\Setup\Output".
    GOTO errors
  )
)

%__ECHO% "%NUGET%" pack "%ROOT%\NuGet\SQLite.nuspec"

IF ERRORLEVEL 1 (
  ECHO The "%ROOT%\NuGet\SQLite.nuspec" package could not be built.
  GOTO usage
)

%__ECHO% "%NUGET%" pack "%ROOT%\NuGet\SQLite.Core.nuspec"

IF ERRORLEVEL 1 (
  ECHO The "%ROOT%\NuGet\SQLite.Core.nuspec" package could not be built.
  GOTO usage
)

%__ECHO% "%NUGET%" pack "%ROOT%\NuGet\SQLite.Core.MSIL.nuspec"

IF ERRORLEVEL 1 (
  ECHO The "%ROOT%\NuGet\SQLite.Core.MSIL.nuspec" package could not be built.
  GOTO usage
)

%__ECHO% "%NUGET%" pack "%ROOT%\NuGet\SQLite.EF6.nuspec"

IF ERRORLEVEL 1 (
  ECHO The "%ROOT%\NuGet\SQLite.EF6.nuspec" package could not be built.
  GOTO usage
)

%__ECHO% "%NUGET%" pack "%ROOT%\NuGet\SQLite.Linq.nuspec"

IF ERRORLEVEL 1 (
  ECHO The "%ROOT%\NuGet\SQLite.Linq.nuspec" package could not be built.
  GOTO usage
)

%__ECHO% "%NUGET%" pack "%ROOT%\NuGet\SQLite.MSIL.nuspec"

IF ERRORLEVEL 1 (
  ECHO The "%ROOT%\NuGet\SQLite.MSIL.nuspec" package could not be built.
  GOTO usage
)

%__ECHO% "%NUGET%" pack "%ROOT%\NuGet\SQLite.x86.nuspec"

IF ERRORLEVEL 1 (
  ECHO The "%ROOT%\NuGet\SQLite.x86.nuspec" package could not be built.
  GOTO usage
)

%__ECHO% "%NUGET%" pack "%ROOT%\NuGet\SQLite.x64.nuspec"

IF ERRORLEVEL 1 (
  ECHO The "%ROOT%\NuGet\SQLite.x64.nuspec" package could not be built.
  GOTO usage
)

%__ECHO% MOVE *.nupkg "%ROOT%\Setup\Output"

IF ERRORLEVEL 1 (
  ECHO Could not move "*.nupkg" to "%ROOT%\Setup\Output".
  GOTO usage
)

GOTO no_errors

:fn_ResetErrorLevel
  VERIFY > NUL
  GOTO :EOF

:fn_SetErrorLevel
  VERIFY MAYBE 2> NUL
  GOTO :EOF

:usage
  ECHO.
  ECHO Usage: %~nx0
  ECHO.
  GOTO errors

:errors
  CALL :fn_SetErrorLevel
  ENDLOCAL
  ECHO.
  ECHO Build failure, errors were encountered.
  GOTO end_of_file

:no_errors
  CALL :fn_ResetErrorLevel
  ENDLOCAL
  ECHO.
  ECHO Build success, no errors were encountered.
  GOTO end_of_file

:end_of_file
%__ECHO% EXIT /B %ERRORLEVEL%
