REM codesign setup
pushd CertificateFile_pfx
signtool.exe sign /a /v  /f HanGil_CodeSign.pfx /p aa123123 ..\Setup_XlsImageExtractor.exe
popd



