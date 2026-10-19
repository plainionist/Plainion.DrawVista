
Browser for interactive diagrams based on Draw.IO

## Idea

Just as wiki pages are automatically linked to each other when one page mentions another by name,
Plainion.DrawVista automatically creates links between diagrams when the name of one diagram
is mentioned within another.

This way related diagrams are easily connected and accessible, allowing convenient navigation
between different components and layers of your designs and architecture.

![](</doc/Index Page.png>)
![](</doc/Programming Languages Page.png>)
![](</doc/Structured Programming Page.png>)

## Installation

- pre-requisite: install DrawIO (the desktop app)
- pre-requisites for building: .NET 10 SDK, Node.js 22.13+ in a supported even-numbered release, and pnpm 11+
- clone this repository
- run 'build\build-release.cmd'
- copy 'src\Plainion.DrawVista\bin\Release\net10.0\*' to a location of your choice or run the application from this location
  
## Usage

- run `Plainion.DrawVista.exe -name my-diagrams` from command prompt
- open a browser at the port printed on the command prompt
- create one or multiple Draw.IO files or PNGs with the Draw.IO model embedded
  - for ".drawio" files: make sure the names of the "tabs" match the names of your diagram elements
  - for ".drawio.png" files: make sure that the filename without the file extension matches the names of your diagram elements
- upload the files using the "upload" page
- the upload will take a few seconds/minutes
- navigate to the "home" page and start browsing your diagrams

## Multiple instances and storage

`-name` is mandatory and must be a valid single Windows folder name. Quote names containing spaces.
To run independent instances on the same machine, use different names and ports:

```cmd
Plainion.DrawVista.exe -name team-a -p 5236
Plainion.DrawVista.exe -name team-b -p 5237
```

Each instance stores its data under `%ALLUSERSPROFILE%\Plainion.DrawVista\<name>`
(normally `C:\ProgramData\Plainion.DrawVista\<name>`):

- `store.db`: the diagram database
- `input`: generated SVGs and temporary Draw.io models
- `input\StartPage`: generated start-page files
- `store`: legacy filesystem storage, imported into this instance's database on startup

Existing shared data is not moved automatically. To retain it, stop the application, create the
named subfolder, and move the existing `store.db`, `input`, and (if present) `store` directory
from `%ALLUSERSPROFILE%\Plainion.DrawVista` into that subfolder before starting the named instance.
Do not overwrite data belonging to an existing named instance.
