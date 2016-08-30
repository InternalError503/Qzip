QuickZip
========

#### What it does: 
A simple .net based archive utility that allows fast creation or extractions of `*.zip` archives.

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
| -D= | `*`Path of the folder you want the archive from, `*`Path of archive you want to extract from.|
| -O= | `*`Path to output generated archive (*.zip automatically added), `*`Path to output archive contents.|
| -X | Extracts a archive when used with `-D` and `-O`|
| -M(N) | (`0` = Never overwrite, `1` = Overwrite only if newer, `2` = Always overwrite _[Default]_)|
| -F | Force overwrite mode 2 (Always Overrite)|
| -B | Include base folder directory. (When building archives)|
| -C(N) | (`0` = Optimal possible compression, `1` = Fastest possible compression, `2` = No compression _[Default]_)|

##### Optional Arguments:

| Command | Optional |
|:----------:|-------------|
| -B | True |
| -C(N) | True |
| -M(N) | True |
| -F | True |

##### Argument `-C(N)`:

| Command | Use |
|:----------:|-------------|
| -C(N) | Number 0-2 |
| -C0 | Optimal possible compression |
| -C1 | Fastest possible compression |
| -C2 | No compression |

##### Argument `-M(N)`:

| Command | Use |
|:----------:|-------------|
| -M(N) | Number 0-2 |
| -M0 | Never overwrite |
| -M1 | Overwrite only if newer |
| -M2 | Always overwrite |

##### Argument Extract Overrite:

By default extracting prompts for user input (Use `-F` to skip):

| Command | Use |
|:----------:|-------------|
| Input | Yes (Y) \ No (N) \ All (A) |
| Y | Confirms overwrite |
| N | Skips current file |
| A | Always overwrite all files |
| Exit | Will terminate program |


For items marked with __*__ are required template parameters all parameters must be set.

#### Usage example:

Create archive from a directory: `Qzip.exe -d=somefolder -o=somefolder.zip -c1 -b`

Extract archive to a directory: `Qzip.exe -d=somefolder.zip --o=somefolder -x -m1`


##### To-Do: (Highly Optional Ideas)
- ~~Add type of overwrite settings, i.e overwrite files only if newer or never overwrite.~~
- ~~Add type of comfirmation on overwrite that can be forced by an argument to skip user input.~~
- Currently we delete the archive and make a new one every time, Need an option to just update the existing.
- Maybe have compressing feedback, Improve extracting feedback.
- ~~Maybe add better command line aguments then current.~~