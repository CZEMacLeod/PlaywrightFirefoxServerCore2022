# Playwright on Windows Server Core 2022 - Chromium

This repo has several branches to demonstate issues with running on Windows Server Core containers.

This branch is a minimal repo to demonstrate the issue with chromium.

With docker for windows installed and running windows containers, run `buildandrun.cmd`.

The first action is to grab a copy of Windows Server 2022 LTSC and extract the WinSxS folder to your local drive.
Note that this uses a lot of disk space! It will be slow the first time while it copies all the files, but subsequent runs are much faster as the files already exist.

Next dotnet publish creates a windows container based on Windows Server Core 2022 LTSC, and launched the program.
The program installs 3 prerequisits before running the playwright code.
1. The program runs a fix to install the base fonts required for chromium using DISM and the WinSxS folder mapped into the container.
1. Installs pwsh
1. Runs the playwright install script
 
Finally it calls playwright to launch chromium in headless mode.

Example log outputs below. Progress bars removed for brevity.

## Install Fonts
```
Starting: cmd.exe /c "installfonts.cmd"

C:\app>dism /online /enable-feature /featurename:ServerCoreFonts-NonCritical-Fonts-MinConsoleFonts /Source:c:\WindowsSource\ /LimitAccess

Deployment Image Servicing and Management tool
Version: 10.0.20348.681

Image Version: 10.0.20348.1906

Enabling feature(s)
[                           0.1%                           ]
[==========================100.0%==========================]
The operation completed successfully.

C:\app>dism /online /enable-feature /featurename:ServerCoreFonts-NonCritical-Fonts-Support /Source:c:\WindowsSource\ /LimitAccess

Deployment Image Servicing and Management tool
Version: 10.0.20348.681

Image Version: 10.0.20348.1906

Enabling feature(s)
[                           0.1%                           ]
[==========================100.0%==========================]
The operation completed successfully.

C:\app>dism /online /enable-feature /featurename:ServerCoreFonts-NonCritical-Fonts-BitmapFonts /Source:c:\WindowsSource\ /LimitAccess

Deployment Image Servicing and Management tool
Version: 10.0.20348.681

Image Version: 10.0.20348.1906

Enabling feature(s)
[                           0.1%                           ]
[==========================100.0%==========================]
The operation completed successfully.

C:\app>dism /online /enable-feature /featurename:ServerCoreFonts-NonCritical-Fonts-TrueType /Source:c:\WindowsSource\ /LimitAccess

Deployment Image Servicing and Management tool
Version: 10.0.20348.681

Image Version: 10.0.20348.1906

Enabling feature(s)
[                           0.1%                           ]
[==========================100.0%==========================]
The operation completed successfully.

C:\app>dism /online /enable-feature /featurename:ServerCoreFonts-NonCritical-Fonts-UAPFonts /Source:c:\WindowsSource\ /LimitAccess

Deployment Image Servicing and Management tool
Version: 10.0.20348.681

Image Version: 10.0.20348.1906

Enabling feature(s)
[                           0.1%                           ]
[==========================100.0%==========================]
The operation completed successfully.
Returned: 0
```

Here we see Dism install the fonts features.

## Install PWSH
```
Starting: dotnet tool install --global PowerShell
You can invoke the tool using the following command: pwsh
Tool 'powershell' (version '7.3.6') was successfully installed.
Returned: 0
```

## Install Playwright Chromium
```
Starting: pwsh.exe playwright.ps1 install chromium
Downloading Chromium 116.0.5845.82 (playwright build v1076) from https://playwright.azureedge.net/builds/chromium/1076/chromium-win64.zip
|                                                                                |   0% of 116 Mb
|■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■| 100% of 116 Mb
Chromium 116.0.5845.82 (playwright build v1076) downloaded to C:\Users\ContainerAdministrator\AppData\Local\ms-playwright\chromium-1076
Downloading FFMPEG playwright build v1009 from https://playwright.azureedge.net/builds/ffmpeg/1009/ffmpeg-win64.zip
|                                                                                |   1% of 1.4 Mb
|■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■| 100% of 1.4 Mb
FFMPEG playwright build v1009 downloaded to C:\Users\ContainerAdministrator\AppData\Local\ms-playwright\ffmpeg-1009
Returned: 0
```
This section shows pwsh and chromium getting installed.

## Expected output
```
Image saved C:\app\screenshot.png 102806
```
Here we see the output working as exprected. Note the filesize may vary depending on the content of [https://playwright.dev/dotnet](https://playwright.dev/dotnet).

## Error output
```
C:\app\.playwright\package\lib\server\chromium\crPage.js:377
      this._firstNonInitialNavigationCommittedReject(new Error('Page closed'));
                                                     ^

Error: Page closed
    at CRSession.<anonymous> (C:\app\.playwright\package\lib\server\chromium\crPage.js:377:54)
    at Object.onceWrapper (node:events:628:28)
    at CRSession.emit (node:events:526:35)
    at C:\app\.playwright\package\lib\server\chromium\crConnection.js:211:39

Node.js v18.17.0
Unhandled exception. Microsoft.Playwright.PlaywrightException: Process exited
   at Microsoft.Playwright.Transport.Connection.InnerSendMessageToServerAsync[T](String guid, String method, Dictionary`2 dictionary, Boolean keepNulls) in /_/src/Playwright/Transport/Connection.cs:line 192
   at Microsoft.Playwright.Transport.Connection.WrapApiCallAsync[T](Func`1 action, Boolean isInternal) in /_/src/Playwright/Transport/Connection.cs:line 515
   at Microsoft.Playwright.Core.Browser.CloseAsync() in /_/src/Playwright/Core/Browser.cs:line 79
   at PlaywrightWindowsServerCore2022.Program.Main() in \PlaywrightWindowsServerCore2022\Program.cs:line 17
   at PlaywrightWindowsServerCore2022.Program.<Main>()
```
Without the fonts installed we get the following error. This can be caused by commenting out the line in `Program.cs`
```cs
    InstallFonts();
```

## Acknoledgements and Improvements
The DISM font install approach is loosely based on the technique described in [Adding optional font packages to Windows containers](https://techcommunity.microsoft.com/t5/itops-talk-blog/adding-optional-font-packages-to-windows-containers/ba-p/3559761).
That requires that the user has admin rights to create a temp share.

Here we map in the folder to the final image and run the script on startup.
Obviously this is not optimal, and we should create an intermediate docker image with the fonts loaded.

Similarly we could pre-install pwsh too.

Unfortunately, the Playwright install script is added by the dotnet publish stage, so cannot be pre-run.
However, we could use a similar technique to the [mcr.microsoft.com/playwright](https://playwright.dev/docs/docker) image to create our own based on Windows Server Core 2022.
