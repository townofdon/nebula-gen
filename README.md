# nebula-gen

Pixel Art Nebula Generator - Standalone Tool

## Installation & Running

This tool should run on MacOS. The build is NOT signed, so you may
need to jump through some hoops to get this to work.

TODO: add links for opening unsigned Mac applications


Windows build coming soon!

## Running in Unity Editor

Because third-party packages were used when making this tool,
those binaries are needed in order for Unity to run. Currently,
it will open in safe-mode and complain that some scripts are missing.

For more info, see [Assets/ThirdParty/README.md](./Assets/ThirdParty/README.md)

## Usage

Usage notes coming soon! For now, open the app and play around with the various
settings. The `Noise` tab mainly drives the generated output (with an optional
second layer), and the `Canvas` tab maps the generated noise to pixel art
output.

### Tabs

#### Canvas Tab

- Change canvas size
- Choose color

#### Noise Tab

- Tweak noise generation parameters
- Enable optional second noise layer

#### Mask Tab

- Enable & tweak optional mask that applies a cut to the noise

#### Border Tab

- Enable border falloff / tiling options
- Useful for creating pixel art nebula clouds with a natural-ish edge
- Tiling is useful for infinitely-repeating backgrounds
