
Browser for interactive diagrams based on Draw.IO

## Idea

Just as wiki pages are automatically linked to each other when one page mentions another by name,
Plainion.DrawVista automatically creates links between diagrams when the name of one diagram
is mentioned within another.

This way related diagrams are easily connected and accessible, allowing convenient navigation
between different components and layers of your designs and architecture.

![](<docs/Index Page.png>)
![](<docs/Programming Languages Page.png>)
![](<docs/Structured Programming Page.png>)

## Installation

- pre-requisite: install DrawIO (the desktop app)
- pre-requisites for building: .NET 10 SDK, Node.js 22.13+ in a supported even-numbered release, and pnpm 11+
- clone this repository
- run 'build\build-release.cmd'
- both Debug and Release .NET builds rebuild the WebUI and copy it to the output directory
- copy 'src\Plainion.DrawVista\bin\Release\net10.0\*' to a location of your choice or run the application from this location

## Usage

- run `Plainion.DrawVista.exe -name my-diagrams` from command prompt
- open a browser at the port printed on the command prompt
- create one or multiple Draw.IO files or PNGs with the Draw.IO model embedded
  - for ".drawio" files: make sure the names of the "tabs" match the names of your diagram elements
  - for ".drawio.png" files: make sure that the filename without the file extension matches the names of your diagram elements
- upload the files using the "upload" page
- the upload will take a few seconds/minutes
- the home/index diagram is automatically regenerated when diagrams are uploaded or deleted; no restart is needed
- navigate to the "home" page and start browsing your diagrams
- use **Layers**, immediately right of Search, to show or hide individual layers
  - layers are listed topmost first, as in draw.io; initial visibility comes from draw.io, including initially hidden layers
  - choices are remembered separately for each page in the current browser tab, including refresh, and reset in a new tab/session
  - toggling keeps the diagram's alignment, zoom, and pan; search still includes hidden-layer captions
  - re-upload older diagrams to add layer support; pages without layer metadata or with only the default layer keep a disabled Layers control
