# PlaywrightFirefoxServerCore2022
Minimal repo to demonstrate issue with firefox on windows core.


With docker for windows installed and running windows containers, run `buildandrun.cmd`.

This creates a windows container based on Windows Server Core 2022 LTSC, and tries to launch firefox using playwright.

The program installs pwsh, runs the playwright install script, then uses playwright to launch firefox in headless mode.

Example log output below.

```batch
You can invoke the tool using the following command: pwsh
Tool 'powershell' (version '7.3.6') was successfully installed.
Downloading Firefox 115.0 (playwright build v1422) from https://playwright.azureedge.net/builds/firefox/1422/firefox-win64.zip
|                                                                                |   0% of 79.5 Mb
|■■■■■■■■                                                                        |  10% of 79.5 Mb
|■■■■■■■■■■■■■■■■                                                                |  20% of 79.5 Mb
|■■■■■■■■■■■■■■■■■■■■■■■■                                                        |  30% of 79.5 Mb
|■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■                                                |  40% of 79.5 Mb
|■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■                                        |  50% of 79.5 Mb
|■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■                                |  60% of 79.5 Mb
|■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■                        |  70% of 79.5 Mb
|■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■                |  80% of 79.5 Mb
|■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■        |  90% of 79.5 Mb
|■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■| 100% of 79.5 Mb
Firefox 115.0 (playwright build v1422) downloaded to C:\Users\ContainerAdministrator\AppData\Local\ms-playwright\firefox-1422
```
This section shows pwsh and firefox getting installed.

```batch
Unhandled exception. Microsoft.Playwright.PlaywrightException: Protocol error (Browser.newPage): Browser closed.
==================== Browser output: ====================
<launching> C:\Users\ContainerAdministrator\AppData\Local\ms-playwright\firefox-1422\firefox\firefox.exe -no-remote -headless -profile C:\Users\ContainerAdministrator\AppData\Local\Temp\playwright_firefoxdev_profile-fadQhb -juggler-pipe -silent
<launched> pid=2352
[pid=2352][err] *** You are running in headless mode.
[pid=2352][out] console.warn: services.settings: Ignoring preference override of remote settings server
[pid=2352][out] console.warn: services.settings: Allow by setting MOZ_REMOTE_SETTINGS_DEVTOOLS=1 in the environment
[pid=2352][out] console.error: ({})
[pid=2352][out]
[pid=2352][out] Juggler listening to the pipe
[pid=2352][out] Crash Annotation GraphicsCriticalError: |[0][GFX1-]: RenderCompositorSWGL failed mapping default framebuffer, no dt (t=6.88553) [GFX1-]: RenderCompositorSWGL failed mapping default framebuffer, no dt
[pid=2352][err] JavaScript error: resource://gre/modules/XULStore.sys.mjs, line 60: Error: Can't find profile directory.
[pid=2352][out] console.error: (new SyntaxError("The URI is malformed.", (void 0), 133))
[pid=2352][err] [Child 2900, MediaDecoderStateMachine #1] WARNING: Decoder=22c7e7e2700 Decode error: NS_ERROR_DOM_MEDIA_FATAL_ERR (0x806e0005) - Error no decoder found for video/avc: file C:/mozilla-build/msys/firefox/dom/media/MediaDecoderStateMachineBase.cpp:164
   at Microsoft.Playwright.Transport.Connection.InnerSendMessageToServerAsync[T](String guid, String method, Dictionary`2 dictionary, Boolean keepNulls) in /_/src/Playwright/Transport/Connection.cs:line 192
   at Microsoft.Playwright.Transport.Connection.WrapApiCallAsync[T](Func`1 action, Boolean isInternal) in /_/src/Playwright/Transport/Connection.cs:line 472
   at Microsoft.Playwright.Core.BrowserContext.NewPageAsync() in /_/src/Playwright/Core/BrowserContext.cs:line 368
   at Microsoft.Playwright.Core.Browser.<>c__DisplayClass31_0.<<NewPageAsync>b__0>d.MoveNext() in /_/src/Playwright/Core/Browser.cs:line 180
--- End of stack trace from previous location ---
   at Microsoft.Playwright.Transport.Connection.WrapApiCallAsync[T](Func`1 action, Boolean isInternal) in /_/src/Playwright/Transport/Connection.cs:line 515
   at Microsoft.Playwright.Core.Browser.NewPageAsync(BrowserNewPageOptions options) in /_/src/Playwright/Core/Browser.cs:line 178
   at PlaywrightFirefoxServerCore2022.Program.Main() in \PlaywrightFirefoxServerCore2022\PlaywrightFirefoxServerCore2022\Program.cs:line 15
   at PlaywrightFirefoxServerCore2022.Program.Main() in \PlaywrightFirefoxServerCore2022\PlaywrightFirefoxServerCore2022\Program.cs:line 15
   at PlaywrightFirefoxServerCore2022.Program.<Main>()
```
Here we see the error(s) occurring with launching the new page in the browser.

There is a seperate issue here (also shown) regarding the remote settings...
This can be fixed (seen in the fix-remote-settings branch) by adding
```cs
        System.Environment.SetEnvironmentVariable("MOZ_REMOTE_SETTINGS_DEVTOOLS", "1");
```
