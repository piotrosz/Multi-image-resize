## Multi image resize ##

Console application that can transform multiple images in directory. 

It can:

* resize every image (you can specify width, length and quality)
* draw custom text on every image (you can specify corner in which text will appear, font size, font family and font color)
* draw custom image on every image (you can specify corner in which image will appear)

### Examples ###

To resize:

`mono imgres.exe -i=~/Pictures -o=~/Pictures/thumbnails -q=90 -w=200 -h=200` 

To add logo:

`mono imgres.exe -i=~/Pictures -o=~/Pictures/thumbnails -q=90 -l=~/logo.jpg`

To add custom text:

`mono imgres.exe -i=~/Pictures -o=~/Pictures/thumbnails -q=90 -t="Copyright info, 2012" -ff=Arial -fs=12 -fc=Yellow`

