rem 불필요한 파일 삭제
pushd ..\bin\Release
del *.pdb
del *.xml
popd

pushd ..\Install
call CodeSign_Program_All.bat
popd

pushd ..\Patch
"c:\Program Files (x86)\wyBuild\wybuild.cmd.exe" "XlsImageExtractor.wyp" /bu /bwu /upload
popd

pushd ..\Install
"c:\Program Files (x86)\Inno Setup 6\Compil32.exe" /cc Setup_XlsImageExtractor.iss
call CodeSign_Setup_All.bat
popd


rem ftp -s:upload.txt
