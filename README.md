# TinkerParse

This is a fork of Demoder's AODB, pulled from [https://github.com/anarchydevs/aodb](https://github.com/anarchydevs/aodb) and upgraded to .net 8.0.

I've added the parsing capability the original repo lacked so it can read various "tables" from the AO resource database and output the data in useful ways. Future versions may export additional resources present in the database as I learn more about those objects. 


## Usage

### Command line options

`Tinkerparse.exe <path to AO folder> [options]`

or

`./Tinkerparse <path to AO folder> [options]`

All options are optional unless otherwise marked.

`-o, --output-dir` - The path where outputs of this application are written. Some options may create subfolders. If not provided, outputs are written to the same folder the application is running in.   

`--items` - Parse all items, and output them according to the relevant output option.

`--nanos` - Parse all nanoprograms, and output them according to the relevant output option.

`--output-json` - Write parsed nano and/or item data as JSON.

`--icons` - Export the game icons.

`--textures` - Export general game textures.

`--dungeon-textures` - Export dungeon textures.

`--ground-textures` - Export ground textures.

`--body-textures` - Export character body textures.


## Build

Clone this repo and build with Visual Studio, VSCode, dotnet cli, or your favorite .NET 8.0 build mechanism. No further steps should be required. 

This application is not reliant on Windows and should build on all platforms supported by .NET 8.0. 

## Thanks

Major thanks goes out to Demoder, Delmus, Auno, and the CellAO team for their lovely code repos, without which this would have been extremely difficult. I stand here on the shoulders of giants.

Also big thanks to 3F1, Portaler, Drake, Unknown, and the whole Project Rubi-Ka team for assistance along the way!
