mkdir %cd%\WindowsSource
REM docker run --rm -v %cd%\WindowsSource\:c:\WindowsSource\ mcr.microsoft.com/windows/server:ltsc2022 Robocopy C:\Windows\WinSXS\ C:\WindowsSource\ /MIR
dotnet publish --os win --arch x64 -c Release -t:PublishContainer
docker run --rm -v %cd%\WindowsSource\:c:\WindowsSource\ playwrightwindowsservercore2022:chromium
