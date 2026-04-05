@echo off
SET "winrar=C:\Program Files\WinRAR\WinRAR.exe"
SET "keyboard_file=LastEpoch_Hud(Keyboard).rar"
SET "gamepad_file=LastEpoch_Hud(WinGamepad).rar"
SET "unwanted_file=LastEpoch_Hud.deps.json"
SET "unwanted_file2=osx.os"
SET "unwanted_file3=osx_arm.os"
SET "unwanted_file4=x11.os"
cd %~dp0
cd ..\Build\Keyboard\net6.0\
SET "keyboard_dir=%cd%
cd %~dp0
cd ..\Build\WinGamepad\net6.0\
SET "gamepad_dir=%cd%
IF EXIST %keyboard_dir%\%unwanted_file% (
		del "%keyboard_dir%\%unwanted_file%"
)
IF EXIST %keyboard_dir%\%unwanted_file2% (
		del "%keyboard_dir%\%unwanted_file2%"
)
IF EXIST %keyboard_dir%\%unwanted_file3% (
		del "%keyboard_dir%\%unwanted_file3%"
)
IF EXIST %keyboard_dir%\%unwanted_file4% (
		del "%keyboard_dir%\%unwanted_file4%"
)
IF EXIST %gamepad_dir%\%unwanted_file% (
		del "%gamepad_dir%\%unwanted_file%"
)
IF EXIST %gamepad_dir%\%unwanted_file2% (
		del "%gamepad_dir%\%unwanted_file2%"
)
IF EXIST %gamepad_dir%\%unwanted_file3% (
		del "%gamepad_dir%\%unwanted_file3%"
)
IF EXIST %gamepad_dir%\%unwanted_file4% (
		del "%gamepad_dir%\%unwanted_file4%"
)
cd %~dp0
cd ..\Latest\
SET "latest_dir=%cd%
mkdir %latest_dir%
IF EXIST %latest_dir%\%keyboard_file% (
	del "%latest_dir%\%keyboard_file%"
)
IF EXIST %latest_dir%\%gamepad_file% (
	del "%latest_dir%\%gamepad_file%"
)
IF EXIST %keyboard_dir%\* (
	cd %keyboard_dir%
	"%winrar%" a -r "%latest_dir%\%keyboard_file%"
)
IF EXIST %gamepad_dir%\* (
	cd %gamepad_dir%
	"%winrar%" a -r "%latest_dir%\%gamepad_file%"
)