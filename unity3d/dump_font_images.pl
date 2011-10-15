#!/usr/local/bin/perl
use Image::Magick;

my $x = "";
my @fonts = $imgs->QueryFont();
for($i = 0; $i < scalar @fonts; $i++)
{
    #my $font_image = Image::Magick->new(debug=>'Annotate');
    my $font_image = Image::Magick->new;
    $font_image->ReadImage('xc:green');
    $x = $font_image->Resize(width=>640, height=>100);
    warn "$x" if "$x";
    my $text = $fonts[$i];
    $x = $font_image->Annotate(font=>$text, x=>0, y=>80, pointsize=>80, stroke=>'black', strokewidth=>2, fill=>'white', text=>$text);
    warn "$x" if "$x";
    my $file_name = "Fonts/".$text.".png";
    $x = $font_image->Write(filename=>$file_name);
    warn "$x" if "$x";
}
