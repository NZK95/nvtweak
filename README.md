# What is **nvtweak** for?

NVIDIA GPU drivers contain a huge number of parameters stored in the Windows registry — usually under either `0000` or `nvlddmkm`.  
Most of them are **hidden** and cannot be modified without external tools.

Even if you manage to find the names of those parameters (DWORD), there’s another problem — figuring out the correct value to set.

Some parameters include the words `Enable` or `Disable` in their names, and those are simple:  
`0x00000000` means *disabled*, and `0x00000001` means *enabled*.

But others use **bitfields** and **bitmasks**, information about which comes from leaked NVIDIA documentation.

- **Bitfields** — a range of bits inside a DWORD. For example, `1:0` means bits 0 and 1 (two bits total).  
- **Bitmask** — the resulting binary value of a DWORD, e.g.  
  `0000 0000 0000 0000 0000 0000 0000 1111`.  

Calculating a bitmask manually takes time. `nvtweak` automates this by parsing the documentation and generating correct DWORD values automatically.  
It also provides a set of utilities for browsing NVIDIA docs and handling DWORDs efficiently.


# Important
- In **nvlddmkm**, unlike **0000**, there are no bitfields — only simple DWORD parameters using the Enable/Disable format mentioned above.  
  Still, some **0000** parameters can behave the same way.  
- All user input must be in **hexadecimal** format, e.g. `0x00000001`.  
- For DWORDs found under **nvlddmkm**, value computation is not performed for the reasons explained above.  

# How to use it
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


# Other features
- **Apply to registry** — writes the calculated value directly to the Windows registry.  
- **Show Description** — shows the parameter’s description in the lower text area.  
  Due to how NVIDIA docs are structured, the description might appear either above or below the parameter. Both are shown; one of them will make sense.  
- **Save to .reg file** — saves the DWORD and its value into a `.reg` file in the nvtweak folder. You can apply it later manually.  
- **Options from value** — displays which bitfields and flags were used to produce a specific value.  
- **Export dwords from documentation** — saves all references of a selected DWORD into a text file for analysis.


## Troubleshooting
If you encounter bugs or unexpected behavior, please report them through the [issue tracker](https://github.com/NZK95/nvtweak/issues).
