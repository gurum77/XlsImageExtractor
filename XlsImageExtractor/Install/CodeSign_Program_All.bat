REM codesign program
pushd CertificateFile_pfx

rem %1 : path for exe file and dll file

rem code sign start *.dll
signtool.exe sign /a /v  /f HanGil_CodeSign.pfx /p aa123123 ..\..\bin\Release\NGettext.dll
signtool.exe sign /a /v  /f HanGil_CodeSign.pfx /p aa123123 ..\..\bin\Release\ImageListView.dll
signtool.exe sign /a /v  /f HanGil_CodeSign.pfx /p aa123123 ..\..\bin\Release\DotNetZip.dll

rem code sign start *.exe
signtool.exe sign /a /v  /f HanGil_CodeSign.pfx /p aa123123 ..\..\bin\Release\XlsImageExtractor.exe

rem call Codesign_all.bat ..\..\bin\Release
popd
