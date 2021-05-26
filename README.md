<div align='center' >
<h1>Anf</h1>
</div>

<div align='center' >
	<h5>A cross platforms comic reader</h5>
</div>

<div align='center'>

[![codecov](https://codecov.io/gh/Cricle/Anf/branch/dev/graph/badge.svg?token=XMIT1MFLDZ)](https://codecov.io/gh/Cricle/Anf)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/a2248df890a242b081e6719bb795f6c6)](https://www.codacy.com?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=Cricle/Anf&amp;utm_campaign=Badge_Grade)

</div>

# Build Status

|Provider|Status|
|:-:|:-|
|Github|[![.NET](https://github.com/Cricle/Anf/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Cricle/Anf/actions/workflows/dotnet.yml)|
|Azure Pipelines|[![Build Status](https://hcricle.visualstudio.com/Kw.Comic/_apis/build/status/Cricle.Anf?branchName=dev)](https://hcricle.visualstudio.com/Kw.Comic/_build/latest?definitionId=7&branchName=dev)|

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

# Render System

|OS|Render Engine|
|:-:|:-:|
|Windows x86|DirectX(SharpDX)|
|Windows x64|SkiaSharp(See Avalonia.Desktop)|
|Linux/MacOS|SkiaSharp/X11(See Avalonia.Desktop)|
|Android/iOS|MAUI|
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

# What's Next

- [ ] Make all logic unification
- [ ] Add test to conver code
- [ ] Accelerate start-up speed
