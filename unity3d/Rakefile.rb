assets = [
    'Textures/Drop.png',
    'Textures/DropBack.png',
    'Textures/Discs.png',
    'Textures/DiscsHi.png',
    'Textures/Tiles.png',
    'Textures/TestCard.png'
]

task :default do
    assets.each do |asset|
        lib = File.expand_path('AssetsSrc/' + File.dirname(asset))
        src = 'AssetsSrc/' + asset + '.rb'
        target = 'Assets/' + asset
        unless uptodate?(target, Dir.glob(lib + '/lib/*.rb') + [src])
            puts target
            system("ruby -rubygems -I#{lib} #{src} -- #{target}")
        end
    end
end
