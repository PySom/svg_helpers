# svg_helpers
- The application serves as an SVG utility
- There are two functions in this solution
- RemoveWhiteSpace and ReplaceSvgEmbeddedStyleWithInlineStyle
- The reason for the project is that most times, I work with SVGs from a designer who designs in Adobe XD. 
- Most times the svgs are saved with spaces. When developing mobile applications with flutter, spaces in image names are not permitted. 
- This trivial issue becomes complex as more svg assets are downloaded.
- The function RemoveWhiteSpace helps to solve this issue. It takes in two arguments 1. The directory and 2. The separator (default is '_').
- It goes through all svgs in that directory, replacing any space in the file name with the separator.

- The second function ReplaceSvgEmbeddedStyleWithInlineStyle does as its name suggest.
- It removes embedded style tag and replaces its content with inline style.
- The motivation for this project is because the SVGPicture package in flutter does not understand style tags in svgs
- Most times the designer using Adobe XD creates svgs that has style tags in them for which the appearance of the svg is heavily dependent.
- This function does the replacement off all svgs found in the directory supplied.

- Both functions tend to listen for input from the command line when running the application.
- For RemoveWhiteSpace, run the application with the command 'dotnet run "remove|<directory_path> [<separator>]"'
- For ReplaceSvgEmbeddedStyleWithInlineStyle, run the application with the command 'dotnet run "replace|<directory_path>"'


- Please do report bugs for fixes.
