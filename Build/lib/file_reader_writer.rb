require 'FileUtils'
class FileReaderWriter
    def read(file)
        File.read(file)
    end
    def write(text, file)
        FileUtils.mkdir_p File.dirname(file)
        File.open(file, "w") { |f| f.puts text }
    end
end
