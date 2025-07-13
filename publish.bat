@echo off
setlocal enabledelayedexpansion

set NUGET_SOURCE=https://api.nuget.org/v3/index.json

for /R %%f in (*.nupkg) do (
    rem Check if the parent folder is named "Release"
    for %%d in ("%%~dpf.") do (
        set "foldername=%%~nxd"
        if /I "!foldername!"=="Release" (
            echo Pushing package: %%f
            nuget push "%%f" -Source %NUGET_SOURCE% -SkipDuplicate
        )
    )
)

pause