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

### Registry Location Differences
- **0000 branch**: Contains both simple flags and complex bitfield parameters
- **nvlddmkm branch**: Contains only binary Enable/Disable parameters

### Input Format
All values must be entered in hexadecimal format with `0x` prefix (e.g., `0x00000001`).

## Features
- Automated value calculation from bitfield specifications
- Registry integration with direct write capability
- Documentation browser for parameter descriptions
- Reverse engineering — reconstruct bitfield configuration from existing values
- Export functionality — generate `.reg` files for batch deployment
- DWORD analysis — extract all references to specific parameters

## Usage
Type the name of a DWORD parameter in the upper field and press **Search**.  
If the parameter exists in the documentation, one of three outcomes is possible:

1. **All parameters and sub‑parameters found**  
   Choose the ones you need and click **Calculate**.

2. **Parameters found but no sub‑parameters**  
   That means this DWORD only supports values `0` or `1`.  
   You’ll see a section that lets you assign the same value to all parameters and get the resulting DWORD value.

3. **Parameter not found**  
   That could mean:  
   - a bug or missing feature in `nvtweak`, or  
   - missing info in the documentation.  
     
   In that case, you can only view the description and enter a value manually.  
   If the DWORD belongs to **nvlddmkm**, the corresponding window will appear.


## Troubleshooting
If you encounter bugs or unexpected behavior, please report them through the [issue tracker](https://github.com/NZK95/nvtweak/issues).

## Credits
[BEYONDPERFORMANCE](https://x.com/BEYONDPERF_LLG)
