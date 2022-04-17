# fshwrite

A FSH image writer that is used by BAT4Max version 4.5 and later.
It compresses the DXT1 and DXT3 formats using [libsquish](https://github.com/svn2github/libsquish), which produces a higher quality image than the DXT compression algorithm used by fshtool.

## Installation

Place `fshwrite.exe`, `squish_Win32.dll` and `squish_x64.dll` into `<3dsMax_Root>\gamepacks\BAT`, overwriting the existing files.

## Updating BAT4Max to use the HD options 

I recommend that you make a backup copy of the original script and the edited script. That way you can swap out the script in use depending on whether you are exporting a building or props/automata. Buildings should generally not need to use uncompressed textures. 

To use the `/hd` and `/zoom5hd` commands you will need to edit `BuildingMill.ms`. You should use search and replace to update every fshwrite call in the `BuildingMill.ms` script. 

The fshwrite commands start with `fshwrite /b:`.

Changing that command to `fshwrite /zoom5hd /b:` will make BAT4Max write the zoom 5 files as uncompressed, alternatively replacing `/zoom5hd` with `/hd` will make BAT4Max write all images as uncompressed. 

## License

This project is licensed under the terms of the MIT License.   
See [LICENSE.txt](LICENSE.txt) for more information.