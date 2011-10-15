require 'RMagick'
include Magick

alpha = Image.new(16, 16) {
    self.image_type = Magick::GrayscaleType
    self.background_color = '#000000'
}

alpha_draw = Draw.new
alpha_draw.fill('white')
alpha_draw.circle(7.5, 7.5, 7.5, 1)
alpha_draw.draw(alpha)

alpha.alpha(Magick::CopyAlphaChannel)

color = Image.new(16, 16) {
    self.image_type = TrueColorType
    self.background_color = '#FFFFFF'
}

color.composite!(alpha, 0, 0, Magick::CopyOpacityCompositeOp)

color.write(ARGV[1])
