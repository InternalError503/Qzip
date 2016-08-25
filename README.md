QuickZip
========

#### What it does: 
A simply .net based archive utility that allows fast creation or extractions of `*.zip` archives.

#### Why make it:
I wanted something fast simple and down to the point without including a 3rd party library or tool like 7zip.

#### Why release it:
Someone may find it useful to use in one of there project or helping as a reference.

#### Why write it in vb.net and C#:
As shown in [why release it](#Why-release-it)

#### What license is it under:
Source code released under [MPL 2.0](https://www.mozilla.org/MPL/2.0/)

### Commands:

| Command | Result |
|:----------:|-------------|
| --D | `*`Path to the folder you want the archive from.|
| --O | `*`Path to output the generated archive. (*.zip automatically added)|
| --X | Extracts a archive when used with `--D` and `--O`|
| --B | Include base folder directory. (When building archives)|
| --Best | Optimal possible compression level.|
| --Fast | Fastest possible compression level.|
| --Store | No compression.|

| Command | Optional |
|:----------:|-------------|
| --B | True |
| --Best | True |
| --Fast | True |
| --Store | True |

For items marked with __*__ are required template parameters all parameters must be set.

#### Usage example:

Create archive from a directory: `Qzip.exe --d=somefolder --o=somefolder.zip --best --b`

Extract archive to a directory: `Qzip.exe --d=somefolder.zip --o=somefolder --X`


##### To-Do: (Highly Optional Ideas)
- Add type of overwrite settings, i.e overwrite files only if newer or never overwrite.
- Add type of comfirmation on overwrite that can be forced by an argument to skip user input.
- Currently we delete the archive an make a new one everytime, Need an option to just update the existing.
- Maybe have compressing feedback, Improve extracting feedback.
- Maybe add better command line aguments then current.