# DrLight

Dr Light is a small program I wrote for DPS. It was all his idea, I just executed.

Dr Light corrects wrong Direction/Tangent entries in .light files exported by Gims EVO using reference rockstar .light files.

## Usage

1. Download & unzip somewhere.
2. Run the .exe once, prompting it to create a `reference` folder.
3. Fill the `reference` folder with reference rockstar .light files. You have to find these yourself.
4. Drag and drop your .light file (or files, it can do multiple at the same time) on to the .exe.

Done. Your .light files now should have the correct direction/tangent values.

## How it works

The program will load all the reference Direction & Tangent pairs into memory. Then it compares the values in your .light file 
with the reference direction & tangent values.

If only the tangent wrong for that direction it will just replace the tangent.

Sometimes the direction is also different. In that case, it will find the direction in the reference files that is closest 
to what's in your .light file.
Then it will replace both your direction & tangent with the reference values.
