# DrLight

Dr Light is a small program I wrote for [@GTA-DPS]( https://github.com/GTA-DPS ) It was his idea, and with some elbow grease this turned out great.

Dr Light corrects wrong Direction/Tangent entries in .light files exported by Gims EVO using stock rockstar .light files for reference. This should allow modders to recreate the quality of interior lighting that Rockstar has.

### [Download](https://github.com/dustinslane/DrLight/releases/latest)

## How to use it

1. Download & unzip somewhere.
2. Run the .exe once, it will create a `reference` folder.
3. Fill the `reference` folder with rockstar .light files. You have to find these yourself, they are not supplied with Dr Light.
4. Drag and drop your .light file or files (it can do multiple at the same time) on to the .exe.

Done. Your .light file(s) now should have the correct direction/tangent values.

## Requirements

.NET framework 4.7.2 ( basically you need to have windows 7 SP1 or higher )

## How it works

The program will load all the reference Direction & Tangent pairs into memory. Then it compares the values in your .light file 
with the reference direction & tangent values.

If only the tangent wrong for that direction it will just replace the tangent.

Sometimes the direction is also different. In that case, it will find the direction in the reference files that is closest 
to what's in your .light file.
Then it will replace both your direction & tangent with the reference values.

In short: it will attempt to fix your interior lighting as best as it can

![](https://cdn.discordapp.com/attachments/669992366182105088/708034324691157044/unknown.png)

Notice in this image how the light is bouncing off of the wall in the back. This is because there's an angled light in the ceiling, I've been told this is unfortunately not possible with how GIMS EVO exports .light files. 

Credit to DPS for that interior and the image.

## Thanks to

[@GTA-DPS]( https://github.com/GTA-DPS )


[@sldsmkd]( https://github.com/sldsmkd ) for providing these files

https://github.com/sldsmkd/vector3d


