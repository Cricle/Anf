<div align='center' >
<h1>Anf</h1>
</div>

<div align='center' >
	<h5>A cross platforms comic reader</h5>
</div>

<div align='center'>

[![codecov](https://codecov.io/gh/Cricle/Anf/branch/dev/graph/badge.svg?token=XMIT1MFLDZ)](https://codecov.io/gh/Cricle/Anf)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/095c3968b8b243e9b908ec01b7302ca3)](https://www.codacy.com/gh/Cricle/Anf/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=Cricle/Anf&amp;utm_campaign=Badge_Grade)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Cricle_Anf&metric=alert_status)](https://sonarcloud.io/dashboard?id=Cricle_Anf)

</div>

# Build Status

|Build Info|Status|
|:-:|:-|
|a|[![.NET Build](https://github.com/Cricle/Anf/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Cricle/Anf/actions/workflows/dotnet.yml)|
|Azure Pipelines|[![Build Status](https://hcricle.visualstudio.com/Kw.Comic/_apis/build/status/Cricle.Anf?branchName=dev)](https://hcricle.visualstudio.com/Kw.Comic/_build/latest?definitionId=7&branchName=dev)|

# Test Status

|Provider|Status|
|:-:|:-|
|Github|[![.NET Test & Upload](https://github.com/Cricle/Anf/actions/workflows/dotnet.test.yml/badge.svg)](https://github.com/Cricle/Anf/actions/workflows/dotnet.test.yml)|

# Publish Status

[![Build and deploy ASP.Net Core app to Azure Web App - anfwebc](https://github.com/Cricle/Anf/actions/workflows/dev_anfwebc.yml/badge.svg)](https://github.com/Cricle/Anf/actions/workflows/dev_anfwebc.yml)

# What is this

This is a cross platforms comic reader, it support any platforms client, and a web service.

To easyly watch comic at desktop, phone or web, and it can run at standalone, or shared.

# Support platforms

|OS|Version|
|:-:|:-:|
|Windows|win7/8.1/10 x86/x64|
|Microsoft Store|win10(Using Application Bridge)|
|Linux|To see .NET 5.0 support platforms|
|MacOS|To see .NET 5.0 support platforms|
|Android|To see MAUI support platforms|
|iOS|To see MAUI support platforms|
|Angular(PWA)|To see Angular support platforms|

OR

|OS|Version|
|:-:|:-:|
|Windows|win7/8.1/10 x86/x64|
|Microsoft Store|win10(Using Uno platform)|
|Linux|Using Uno platform|
|MacOS|Using Uno platform|
|Android|Using Uno platform|
|iOS|Using Uno platform|
|Angular(PWA)|To see Angular support platforms|


# Render System

|OS|Render Engine|
|:-:|:-:|
|Windows x86|DirectX(SharpDX)|
|Windows x64|SkiaSharp(See Avalonia.Desktop)|
|Linux/MacOS|SkiaSharp/X11(See Avalonia.Desktop)|
|Android/iOS|MAUI|
|Web/PWA|Angular|

OR

|OS|Render Engine|
|:-:|:-:|
|Windows x86|See Uno platform|
|Windows x64|See Uno platform|
|Linux/MacOS|See Uno platform|
|Android/iOS|See Uno platform|
|Web/PWA|Angular|

# How to accelerated comic analysis

- Using fast network
- Using internal CDN shared parse
- Using Web analysis

# How to build it

## Build desktop

```powershell
dotnet build Platforms\Anf.Desktop\Anf.Desktop.csproj -f net472
```

OR

Uno platform 

# What's Next

- [x] Make all logic unification
- [ ] Add test to conver code
- [ ] Accelerate start-up speed
- [ ] Support uno platform