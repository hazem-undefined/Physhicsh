# <sub>![icon](https://raw.githubusercontent.com/hazem-undefined/physhicsh-docs/master/ico.bmp)</sub> Physhicsh | ڤيچيكش

// Simple Physics sandbox.

> [!WARNING]  
> Under development. Expect bugs.

[![Latest release](https://img.shields.io/github/v/release/hazem-undefined/Physhicsh?include_prereleases)](https://github.com/hazem-undefined/Physhicsh/releases)

<details>
<summary>Screenshots</summary>

![sc1](https://raw.githubusercontent.com/hazem-undefined/physhicsh-docs/master/screenshot4.jpg)
![sc2](https://raw.githubusercontent.com/hazem-undefined/physhicsh-docs/master/screenshot6.jpg)
![sc3](https://raw.githubusercontent.com/hazem-undefined/physhicsh-docs/master/screenshot5.jpg)
![sc4](https://raw.githubusercontent.com/hazem-undefined/physhicsh-docs/master/screenshot1.jpg)

</details>

## Prerequisites

- [.NET SDK **10.0** or later](https://dotnet.microsoft.com/en-us/download/dotnet/10.0). verify with:
  ```bash
  dotnet --version
  ```

## Build

### Get the source

I recommend using the [latest release](https://github.com/hazem-undefined/Physhicsh/releases) source code as it's the most stable.

For the latest, unstable source code :

```bash
git clone https://github.com/hazem-undefined/Physhicsh.git
cd Physhicsh
```

### Debug build

From the repository root:

```bash
cd Physhicsh
dotnet build Physhicsh.csproj
```

Lands at: `...\Physhicsh\Physhicsh\bin\Debug\net10.0`

### Publish (release) build

From the repository root:

```bash
cd Physhicsh
dotnet publish /p:PublishProfile=FolderProfile
```

Lands at: `...\Physhicsh\Physhicsh\bin\Release\net10.0\publish\win-x64`

# <sub>![icon](https://raw.githubusercontent.com/hazem-undefined/physhicsh-docs/master/ico4.bmp)</sub> Controls

### Main:

**Tab** : Open quick spawn menu

### Spawning:

**R** : Spawn a rectangle with a random color

**C** : Spawn a circle with a random color

### Object:

<ins>Requires holding an object first</ins>

**V** : Freeze/Unfreeze selected object

**X** : Delete selected object

**E** : Rotate right

**Q** : Rotate Left

**H** : Increase the selected circle's radius (requires holding a circle)

**J** : Decrease the selected circle's radius (requires holding a circle)

### Other:

**F3** : Toggle debug window

## [LICENSE](LICENSE)

GPL-3.0 license
