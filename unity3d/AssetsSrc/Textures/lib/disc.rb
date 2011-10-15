def number_color(size, number, color_str)
    num_offset = size / 21
    num_string = number.to_s
    
    text_draw = Magick::Draw.new() {
        self.fill = 'white'
        self.gravity = Magick::CenterGravity
        self.font_family = "Arial"
        self.font_weight = 900
        self.pointsize = size * 6 / 7
    }

    # Shadow Mask image
    shadow_mask = Magick::Image.new(size, size) {
        self.image_type = Magick::GrayscaleType
        self.background_color = 'black'
    }
    
    for i in (-num_offset..size)
        text_draw.annotate(shadow_mask, 0, 0, -i, i, num_string)
    end
    
    shadow_mask.alpha(Magick::CopyAlphaChannel)
    
    # Shadow color image
    color = Magick::Pixel.from_color(color_str)
    
    shadow_intensity = 0.4
    shadow_canvas = Magick::Image.new(size, size) {
        self.image_type = Magick::TrueColorMatteType
        self.background_color = Magick::Pixel.new(
                shadow_intensity * color.red,
                shadow_intensity * color.green,
                shadow_intensity * color.blue,
                color.opacity
            )
    }
    
    shadow_canvas.composite!(shadow_mask, 0, 0, Magick::CopyOpacityCompositeOp)
    
    # Color image
    disc = Magick::Image.new(size, size) {
        self.image_type = Magick::TrueColorType
        self.background_color = color
    }
    
    # Compose shadow image
    disc.composite!(shadow_canvas, 0, 0, Magick::OverCompositeOp)
    
    # Draw the number on top
    text_draw.annotate(disc, 0, 0, num_offset,-num_offset, num_string)
    
    return disc
end

def open_disc_alpha(size)
    scale = size / 42
    
    disc_alpha = Magick::Image.new(size, size) {
        self.image_type = Magick::GrayscaleType
        self.background_color = 'black'
    }
    
    alpha_draw = Magick::Draw.new()

    alpha_draw.fill('white')
    alpha_draw.circle((size - 1) * 0.5, (size - 1) * 0.5, (size - 1) * 0.5, 0.5)

    alpha_draw.draw(disc_alpha)

    return disc_alpha
end

def closed_disc_alpha(size, cracked)
    scale = size / 42
    
    disc_alpha = Magick::Image.new(size, size) {
        self.image_type = Magick::GrayscaleType
        self.background_color = 'black'
    }
    
    alpha_draw = Magick::Draw.new()

    alpha_draw.fill('white')
    alpha_draw.circle((size - 1) * 0.5, (size - 1) * 0.5, (size - 1) * 0.5, 7.5 * scale)

    alpha_draw.fill('none')
    alpha_draw.stroke('white')
    alpha_draw.stroke_width(4 * scale)
    if cracked
        circum = 2 * Math::PI * ((size - 1) * 0.5 - 2.5 * scale)
        #alpha_draw.stroke_dasharray(7.75 * scale, 3.875 * scale)
        alpha_draw.stroke_dasharray(circum * 0.1 * 0.75, circum * 0.1 * 0.25)
    end
    alpha_draw.circle((size - 1) * 0.5, (size - 1) * 0.5, (size - 1) * 0.5, 2.5 * scale)

    alpha_draw.draw(disc_alpha)

    return disc_alpha
end

def discs_image(size, margin)
    discs_size = size * 3 + margin * 2
    
    # Color image
    discs = Magick::Image.new(discs_size, discs_size) {
        self.image_type = Magick::TrueColorMatteType
        #self.background_color = '#999999'
        self.background_color = '#FF00FF'
    }
    
    discs.composite!(number_color(size, 1, 'green'), 0, (size + margin) * 2, Magick::CopyCompositeOp)
    discs.composite!(number_color(size, 2, 'yellow3'), size + margin, (size + margin) * 2, Magick::CopyCompositeOp)
    discs.composite!(number_color(size, 3, 'orange'), (size + margin) * 2, (size + margin) * 2, Magick::CopyCompositeOp)

    discs.composite!(number_color(size, 4, 'red'), 0, size + margin, Magick::CopyCompositeOp)
    discs.composite!(number_color(size, 5, 'magenta4'), size + margin, size + margin, Magick::CopyCompositeOp)
    discs.composite!(number_color(size, 6, 'DeepSkyBlue'), (size + margin) * 2, size + margin, Magick::CopyCompositeOp)

    discs.composite!(number_color(size, 7, 'DarkSlateBlue'), 0, 0, Magick::CopyCompositeOp)

    # Alpha image
    discs_alpha = Magick::Image.new(discs_size, discs_size) {
        self.image_type = Magick::GrayscaleType
        #self.background_color = '#000000'
        self.background_color = '#FFFFFF'
    }

    alpha_draw = Magick::Draw.new()

    alpha_draw.fill('white')
    (0...7).each do |i|
        r = 2 - (i / 3)
        c = i % 3
        x = c * (size + margin)
        y = r * (size + margin)
        discs_alpha.composite!(open_disc_alpha(size), x, y, Magick::CopyCompositeOp)
    end
    alpha_draw.draw(discs_alpha)
    
    discs_alpha.composite!(closed_disc_alpha(size, false), size + margin, 0, Magick::CopyCompositeOp)
    discs_alpha.composite!(closed_disc_alpha(size, true), (size + margin) * 2, 0, Magick::CopyCompositeOp)
    
    discs_alpha.alpha(Magick::CopyAlphaChannel)
    
    discs.composite!(discs_alpha, 0, 0, Magick::CopyOpacityCompositeOp)
    
    return discs
end
