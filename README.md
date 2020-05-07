# DrLight

Dr Light is a small program I wrote for [@GTA-DPS]( https://github.com/GTA-DPS ) It was his idea, and with some elbow grease this turned out great.

Dr Light corrects wrong Direction/Tangent entries in .light files exported by Gims EVO using stock rockstar .light files for reference. This should allow modders to recreate the quality of interior lighting that Rockstar has.

## How to use it

1. Download & unzip somewhere.
2. Run the .exe once, it will create a `reference` folder.
3. Fill the `reference` folder with rockstar .light files. You have to find these yourself, they are not supplied with Dr Light.
4. Drag and drop your .light file or files (it can do multiple at the same time) on to the .exe.

Done. Your .light file(s) now should have the correct direction/tangent values.

## How it works

The program will load all the reference Direction & Tangent pairs into memory. Then it compares the values in your .light file 
with the reference direction & tangent values.

If only the tangent wrong for that direction it will just replace the tangent.

Sometimes the direction is also different. In that case, it will find the direction in the reference files that is closest 
to what's in your .light file.
Then it will replace both your direction & tangent with the reference values.

In short: it will attempt to fix your interior lighting as best as it can

## Thanks to

[@GTA-DPS]( https://github.com/GTA-DPS )


[@sldsmkd]( https://github.com/sldsmkd ) for providing these files

https://github.com/sldsmkd/vector3d


