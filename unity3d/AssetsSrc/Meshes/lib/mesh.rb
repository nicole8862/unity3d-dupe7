require 'FileUtils'

class Mesh
    def initialize(data = {})
        @indices = data[:indices]
        @positions = data[:positions]
        @uvs = data[:uvs] || []
    end
    
    def write_obj(file_name)
        file = File.open(file_name, 'w')

        # write positions - negate x
        (0...@positions.length).step(3) do |i|
            file << "v #{-@positions[i]} #{@positions[i+1]} #{@positions[i+2]}\n"
        end

        # write uvs
        if @uvs.length
            (0...@uvs.length).step(2) do |i|
                file << "vt #{@uvs[i]} #{@uvs[i+1]}\n"
            end
        end
        
        # write faces
        (0...@indices.length).step(3) do |i|
            a, b, c = @indices[i]+1, @indices[i+1]+1, @indices[i+2]+1
            if @uvs.length
                file << "f #{a}/#{a} #{b}/#{b} #{c}/#{c}\n"
            else
                file << "f #{a} #{b} #{c}\n"
            end
        end            
    end
    
    attr_accessor :positions, :indices, :uvs
end
