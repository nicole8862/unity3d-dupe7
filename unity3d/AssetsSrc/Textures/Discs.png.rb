require 'RMagick'
require 'lib/disc'

discs_image(42, 1).write(ARGV[1])
