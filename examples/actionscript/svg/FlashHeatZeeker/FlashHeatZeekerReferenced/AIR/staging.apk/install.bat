echo off
rem http://help.adobe.com/en_US/flex/mobileapps/WS19f279b149e7481c-24dc70c812b9cbf7285-8000.html
rem http://forum.starling-framework.org/topic/wrong-wmode-with-adobeair-32-desktop
call createapk.bat

"C:\util\android-sdk-windows\platform-tools\adb.exe"  install -r foo.apk
