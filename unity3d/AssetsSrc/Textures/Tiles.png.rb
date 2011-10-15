require 'RMagick'
include Magick

color = Image.new(128, 128) {
    self.image_type = TrueColorType
    #self.background_color = '#707070'
    self.background_color = '#00FF00'
}

color_draw = Draw.new
color_draw.fill = 'white'
color_draw.rectangle(43, 0, 84, 41)
color_draw.draw(color)

color_draw = Draw.new
color_draw.fill = 'black'
color_draw.rectangle(86, 0, 127, 41)
color_draw.draw(color)

color_draw = Draw.new
color_draw.fill = '#404040'
color_draw.rectangle(0, 86, 41, 127)
color_draw.draw(color)

color.write(ARGV[1])
