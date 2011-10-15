require 'RMagick'
include Magick

color = Image.new(128, 128) {
    self.image_type = TrueColorType
    self.background_color = '#000000'
}

color_draw = Draw.new

color_draw.fill = 'white'

(0...5).each do |v|
    xo = ((v * 2) % 3) * 43
    yo = ((v * 2) / 3) * 43
    (0...41).step(2) do |l|
        color_draw.line(l + xo, yo, l + xo, 41 + yo)
    end
end

(0...4).each do |v|
    xo = (((v * 2) + 1) % 3) * 43
    yo = (((v * 2) + 1) / 3) * 43
    (0...41).step(2) do |l|
        color_draw.line(xo, l + yo, 41 + xo, l + yo)
    end
end

color_draw.draw(color)

color_draw = Draw.new
color_draw.fill = 'red'
color_draw.line(42, 0, 42, 127)
color_draw.line(85, 0, 85, 127)
color_draw.line(0, 42, 127, 42)
color_draw.line(0, 85, 127, 85)
color_draw.draw(color)

color.write(ARGV[1])
