require 'lib/mesh'

#
# 1 --- 0
# |  |/ |
# |  +- |
# | /   |
# 2 --- 3
#

Mesh.new(
    :indices => [
        0, 1, 2,
        0, 2, 3,
    ],
    :positions => [
        0.5, 0.5, 0.0,
       -0.5, 0.5, 0.0,
       -0.5,-0.5, 0.0,
        0.5,-0.5, 0.0,
    ],
    :uvs => [
        1.0, 1.0,
        0.0, 1.0,
        0.0, 0.0,
        1.0, 0.0,
    ]
).write_obj(ARGV[1])
