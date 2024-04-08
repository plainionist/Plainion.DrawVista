
Browser for interactive diagrams based on Draw.IO

## Idea

Just as wiki pages are automatically linked to each other when one page mentions another by name,
Plainion.DrawVista automatically creates links between diagrams when the name of one diagram
is mentioned within another.

This way related diagrams are easily connected and accessible, allowing convenient navigation
between different components and layers of your designs and architecture.

## Installation

- clone this repository
- run 'build\build-release.cmd'
- copy 'src\Plainion.DrawVista\bin\Release\net8.0\*' to a location of your choice or run the application from this location
  
## Usage

- run "Plainion.DrawVista.exe" from command prompt
- open a browser at the port printed on the command prompt
- create one or multiple Draw.IO files or PNGs with the Draw.IO model embedded
  - make sure that the filename without the file extension matches the names of your diagram elements
- upload the files using the "upload" page
- the upload will take a few seconds/minutes
- navigate to the "home" page and start browsing your diagrams
