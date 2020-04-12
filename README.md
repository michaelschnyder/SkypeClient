# SkypeNotifier
Skype Notifier consist of a comple of libraries that eventually team up to send notifications about incoming Skype calls to other devices like an Arduino/ESP32/ESP8266.

**Current Status**

:heavy_check_mark: Login and recieve notifications on a console application

:heavy_check_mark: Login and recieve notifications on a WPF application

:heavy_check_mark: Code split for reusability

**Pending**
* Forward notifications to listener device
* Wrapper to hide heavy dependencies to CefSharp

## Demo
In the current state the application provides console output for incoming messages and calls, as seen below
![Communicating with Skype running in console](skype-console-client-demo.gif)

The shown demo can be found in `Skype.Client.CefSharp.OffScreen` project

## Acknowledgements
This project is based on 
Newtonsoft Json.NET (MIT)
CEFSharp (BSD)
