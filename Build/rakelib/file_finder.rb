class FileFinder
    def find(directory, pattern)
        full_pattern = File.join(directory, pattern)
        Dir.glob(full_pattern)
    end
end
