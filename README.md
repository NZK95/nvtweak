<div align="center">

<h1>
  <img src="https://cdn-icons-png.flaticon.com/512/888/888866.png" width="30" align="center"/>
  nvtweak
</h1>

*A tool for tweaking hidden NVIDIA driver settings.*

![GitHub Last Commit](https://img.shields.io/github/last-commit/NZK95/nvtweak?style=flat-square)
[![Downloads](https://img.shields.io/github/downloads/NZK95/nvtweak/total?style=flat-square&color=brightgreen)](https://github.com/NZK95/nvtweak/releases)
![GitHub Stars](https://img.shields.io/github/stars/NZK95/nvtweak?style=flat-square)
![GitHub Issues](https://img.shields.io/github/issues/NZK95/nvtweak?style=flat-square)
![GitHub License](https://img.shields.io/github/license/NZK95/nvtweak?style=flat-square)
![Platform](https://img.shields.io/badge/platform-Windows-blue?style=flat-square&logo=windows)
![NVIDIA](https://img.shields.io/badge/NVIDIA-GPU-76B900?style=flat-square&logo=nvidia&logoColor=white)

<img src="https://github.com/NZK95/nvtweak/blob/master/assets/images/nvtweak%20%231.png" width="800"/>

</div>

<br>

## Disclaimer
> [!WARNING]
> The author is not responsible for any possible damage caused to hardware as a result of using this project.
>
> This software does not guarantee any increase in performance and is intended for enthusiasts only.
>
> The NVIDIA documentation is often incomplete and many parameters are not described properly.
>
> You use this program at your own risk.

## Table of Contents
- [Requirements](#requirements)
- [Introduction](#introduction)
- [Features](#features)
- [Usage](#usage)
- [Useful Links](#useful-links)
- [Credits](#credits)
- [Troubleshooting](#troubleshooting)
- [License](#license)

## Requirements
- Windows x64
- Administrator privileges
- Last version of **nvtweak** from [`releases`](https://github.com/NZK95/nvtweak/releases)

## Introduction
NVIDIA GPU drivers contain a huge number of parameters stored in the Windows registry — usually under either `0000` or `nvlddmkm` path.
Most of them are **hidden** and cannot be seen or read without external tools (<code>WPA/MXA/ProcMon</code>).
Even if you find the names of those parameters, there’s another problem — figuring out the correct value to set, and without the documentation they are useless.
You can use the **Leaked NVIDIA Documentation 2022** to calculate value manually, but it takes time and is annoying.
`nvtweak` automates this by parsing the documentation and generating correct values for parameters automatically. 
It also provides a set of utilities for browsing NVIDIA docs and handling registry DWORDs efficiently. 

### Registry Location Differences
- **0000 branch**: Contains simple flags and complex bitfield parameters
- **nvlddmkm branch**: Contains only binary <code>Enable/Disable</code> parameters

### Input Format
All values must be entered in hexadecimal format with `0x` prefix (e.g., `0x00000001`).

## Features
| Feature                                           | Description                                                        |
|---------------------------------------------------|--------------------------------------------------------------------|
| **Automated Value Calculation**                   | Automatically calculate values based on bitfield specifications.   |
| **Registry Integration with Direct Write**        | Directly write to registry with integration capabilities.         |
| **Documentation Browser for Parameter Descriptions** | Allows browsing of documentation related to parameter descriptions.|
| **Reverse**                           | Reconstruct bitfield configuration from existing values.          |
| **Export Functionality**                          | Generate .reg files for batch deployment.                         |
| **DWORD Analysis**                                | Extract all references to specific parameters in DWORD format.    |


## Usage
1. Enter the DWORD parameter name (If hidden extract them with WPA/MXA/PROCMON etc..) in the search field
2. Click **Search** to query the documentation database
3. Interpret results based on parameter type:

#### Case 1: Complex Bitfield Parameter
- Select desired bitfield options from the presented list
- Click **Calculate** to generate the hexadecimal value

#### Case 2: Binary Flag Parameter
- Only values `0x00000000` and `0x00000001` are valid
- Use the provided interface to set all flags uniformly

#### Case 3: Undocumented Parameter
- Parameter exists in registry but lacks documentation
- Manual value entry required
- Use **Show Description** to view context (if available)

## Useful Links
**NVIDIA 2022 LEAK** <br>

```bat
magnet:?xt=urn:btih:DC718539145BDE27DDDB5E94C67949E6D1C8513C&dn=integdev_gpu_drv.rar&tr=udp%3a%2f%2ftracker.openbittorrent.com%3a80%2fannounce&tr=udp%3a%2f%2ftracker.opentrackr.org%3a1337%2fannounce
```

## Credits
[BEYONDPERFORMANCE](https://x.com/BEYONDPERF_LLG)

## Troubleshooting
If you encounter bugs or unexpected behavior, please report them through the [issue tracker](https://github.com/NZK95/nvtweak/issues).

> [!NOTE]
> Some parameters are very experimental and can break the video driver, cause BSOD or other problems, so I always recommend storing the registry backup and logging all changed settings, if you still encounter this problem, just boot into Windows using Safe Mode and delete the setting that causes problems
## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE.txt) file for details.
