@echo off
SETLOCAL EnableDelayedExpansion

SET "SCRIPT_DIR=%~dp0"
SET "BUILD_DIR=%SCRIPT_DIR%..\Build"
SET "LATEST_DIR=%SCRIPT_DIR%..\Latest"

REM Find WinRAR (only tool that can create .rar archives)
SET "RAR="

REM Try registry (64-bit and 32-bit keys)
FOR %%K IN (
    "HKLM\SOFTWARE\WinRAR"
    "HKLM\SOFTWARE\WOW6432Node\WinRAR"
) DO (
    IF "!RAR!"=="" (
        FOR /F "tokens=2*" %%A IN ('reg query %%K /v "exe64" 2^>nul') DO SET "RAR=%%B"
        IF "!RAR!"=="" (
            FOR /F "tokens=2*" %%A IN ('reg query %%K /v "exe32" 2^>nul') DO SET "RAR=%%B"
        )
    )
)

REM Try common install paths
IF "!RAR!"=="" IF EXIST "C:\Program Files\WinRAR\WinRAR.exe" SET "RAR=C:\Program Files\WinRAR\WinRAR.exe"
IF "!RAR!"=="" IF EXIST "C:\Program Files (x86)\WinRAR\WinRAR.exe" SET "RAR=C:\Program Files (x86)\WinRAR\WinRAR.exe"

REM Try PATH
IF "!RAR!"=="" (
    WHERE WinRAR.exe >nul 2>&1 && FOR /F "delims=" %%P IN ('WHERE WinRAR.exe') DO SET "RAR=%%P"
)

IF "!RAR!"=="" (
    echo [MakeLatest] WinRAR not found. Install WinRAR to create .rar archives.
    exit /b 1
)

echo Using: !RAR!

REM Prepare Latest directory
IF NOT EXIST "%LATEST_DIR%" mkdir "%LATEST_DIR%"

REM Package each config that has a build
FOR %%C IN (Keyboard WinGamepad) DO (
    SET "SRC=%BUILD_DIR%\%%C\net6.0"
    IF EXIST "!SRC!\LastEpoch_Hud.dll" (
        REM Remove unwanted build artifacts
        FOR %%F IN (LastEpoch_Hud.deps.json osx.os osx_arm.os x11.os) DO (
            IF EXIST "!SRC!\%%F" del "!SRC!\%%F"
        )

        SET "OUT=%LATEST_DIR%\LastEpoch_Hud(%%C).rar"
        IF EXIST "!OUT!" del "!OUT!"

        cd /d "!SRC!"
        "!RAR!" a -r -ibck "!OUT!"
        echo Created: !OUT!
    ) ELSE (
        echo [MakeLatest] Skipped %%C - no build found
    )
)

ENDLOCAL
