require 'RMagick'
include Magick

color = Image.new(256, 256) {
    self.image_type = TrueColorMatteType
    self.background_color = 'red'
}

alpha = Image.new(256, 256) {
    self.image_type = GrayscaleType
    self.background_color = 'black'
}

alpha_draw = Draw.new
alpha_draw.fill = 'white'
alpha_draw.circle(127.5, 127.5, 127.5, 0.5)
alpha_draw.draw(alpha)

# According to docs, shouldn't need this:
# grayscale intensity should be used in the absence of an alpha channel
alpha.alpha(CopyAlphaChannel)

color.composite(alpha, 0, 0, CopyOpacityCompositeOp).write(ARGV[1])
