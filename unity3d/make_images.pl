#!/usr/local/bin/perl
use Image::Magick;

my $imgs = Image::Magick->new(size=>'128x128');

#$imgs->Read("xc:green");
#$imgs->Draw(fill=>'#000000', stroke=>'#000000', primitive=>'line', points=>'42,0 42,127');
#$imgs->Draw(fill=>'#000000', stroke=>'#000000', primitive=>'line', points=>'85,0 85,127');
#$imgs->Draw(fill=>'#000000', stroke=>'#000000', primitive=>'line', points=>'0,42 127,42');
#$imgs->Draw(fill=>'#000000', stroke=>'#000000', primitive=>'line', points=>'0,85 127,85');

$imgs->Read("xc:transparent");
$imgs->Draw(fill=>'#0000ff', stroke=>'#0000ff', primitive=>'circle', points=>'20.5,20.5 20.5,0.5');
$imgs->Annotate(font=>'Tahoma-Bold', x=>9, y=>34, pointsize=>36, stroke=>'black', strokewidth=>2, fill=>'white', text=>'7');
$imgs->Draw(fill=>'#3f3f3f', stroke=>'#1f1f1f', primitive=>'circle', points=>'63.5,20.5 63.5,0.5');
$imgs->Draw(fill=>'#7f7f7f', stroke=>'#5f5f5f', primitive=>'circle', points=>'106.5,20.5 106.5,0.5');
$imgs->Draw(fill=>'#ff0000', stroke=>'#ff0000', primitive=>'circle', points=>'20.5,63.5 20.5,43.5');
$imgs->Annotate(font=>'Tahoma-Bold', x=>8, y=>77, pointsize=>36, stroke=>'black', strokewidth=>2, fill=>'white', text=>'4');
$imgs->Draw(fill=>'#ff00ff', stroke=>'#ff00ff', primitive=>'circle', points=>'63.5,63.5 63.5,43.5');
$imgs->Annotate(font=>'Tahoma-Bold', x=>53, y=>77, pointsize=>36, stroke=>'black', strokewidth=>2, fill=>'white', text=>'5');
$imgs->Draw(fill=>'#00ffff', stroke=>'#00ffff', primitive=>'circle', points=>'106.5,63.5 106.5,43.5');
$imgs->Annotate(font=>'Tahoma-Bold', x=>94, y=>77, pointsize=>36, stroke=>'black', strokewidth=>2, fill=>'white', text=>'6');
$imgs->Draw(fill=>'#00ff00', stroke=>'#00ff00', primitive=>'circle', points=>'20.5,106.5 20.5,86.5');
$imgs->Annotate(font=>'Tahoma-Bold', x=>9, y=>119, pointsize=>36, stroke=>'black', strokewidth=>2, fill=>'white', text=>'1');
$imgs->Draw(fill=>'#ffff00', stroke=>'#ffff00', primitive=>'circle', points=>'63.5,106.5 63.5,86.5');
$imgs->Annotate(font=>'Tahoma-Bold', x=>52, y=>119, pointsize=>36, stroke=>'black', strokewidth=>2, fill=>'white', text=>'2');
$imgs->Draw(fill=>'#ff7f00', stroke=>'#ff7f00', primitive=>'circle', points=>'106.5,106.5 106.5,86.5');
$imgs->Annotate(font=>'Tahoma-Bold', x=>95, y=>119, pointsize=>36, stroke=>'black', strokewidth=>2, fill=>'white', text=>'3');
$imgs->Write("Assets/disc_sheet.png");
