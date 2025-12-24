# nvtweak
[![Downloads](https://img.shields.io/github/downloads/NZK95/nvtweak/total.svg)](https://github.com/NZK95/nvtweak/releases)

> ### Disclaimer
> The author is not responsible for any possible damage caused to hardware as a result of using this project. <br>
> This software does not guarantee any increase in performance and is intended for enthusiasts only. <br>
> You use this program at your own risk. <br>

## Introduction
NVIDIA GPU drivers contain a huge number of parameters stored in the Windows registry — usually under either `0000` or `nvlddmkm`. <br>
Most of them are **hidden** and cannot be modified without external tools. <br>
Even if you manage to find the names of those parameters (DWORD), there’s another problem — figuring out the correct value to set. It takes time. 
`nvtweak` automates this by parsing the documentation and generating correct DWORD values automatically. <br>
It also provides a set of utilities for browsing NVIDIA docs and handling DWORDs efficiently. <br>

## Registry Location Differences
- **0000 branch**: Contains both simple flags and complex bitfield parameters
- **nvlddmkm branch**: Contains only binary Enable/Disable parameters

## Input Format
All values must be entered in hexadecimal format with `0x` prefix (e.g., `0x00000001`).

## Features
- Automated value calculation from bitfield specifications
- Registry integration with direct write capability
- Documentation browser for parameter descriptions
- Reverse engineering — reconstruct bitfield configuration from existing values
- Export functionality — generate `.reg` files for batch deployment
- DWORD analysis — extract all references to specific parameters

## Usage
1. Enter the `DWORD` parameter name in the search field
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

## Troubleshooting
If you encounter bugs or unexpected behavior, please report them through the [issue tracker](https://github.com/NZK95/nvtweak/issues).

## Credits
[BEYONDPERFORMANCE](https://x.com/BEYONDPERF_LLG)
