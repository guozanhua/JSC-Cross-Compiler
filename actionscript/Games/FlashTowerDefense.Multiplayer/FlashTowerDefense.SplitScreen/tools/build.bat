:mxmlc
@echo off



:: Dll name
@call :jsc %1

if '%ERRORLEVEL%' == '-1' (
    echo jsc failed.
    goto :eof
)
:: Namespace name, type name
@call :mxmlc FlashTowerDefense/ActionScript/SplitScreen FlashTowerDefenseSplitScreen
@call :mxmlc FlashTowerDefense/ActionScript/SplitScreen/Monetized MochiPreloader

::@call :mxmlc %1/ActionScript %1

goto :eof

:jsc
pushd ..\bin\debug

call c:\util\jsc\bin\jsc.exe %1.dll  -as


popd
goto :eof

:mxmlc
@echo off
pushd ..\bin\debug\web



call :build %1 %2


popd
goto :eof

:build
echo - %2
:: http://www.adobe.com/products/flex/sdk/
:: -compiler.verbose-stacktraces 
:: call C:\util\flex2\bin\mxmlc.exe -keep-as3-metadata -incremental=true -output=%2.swf -strict -sp=. %1/%2.as
call C:\util\flex_sdk_4.6\bin\mxmlc.exe -static-link-runtime-shared-libraries=true  --target-player=10.1.0 -debug -compiler.verbose-stacktraces   -keep-as3-metadata -warnings=false   -incremental=true -output=%2.swf -sp=. %1/%2.as
goto :eof