class FileConverter
    def initialize(file_reader_writer, converter)
        @file_reader_writer = file_reader_writer
        @converter = converter
    end
    def convert(source_file, target_file)
        original_text = @file_reader_writer.read(source_file)
        converted_text = @converter.convert(original_text)
        @file_reader_writer.write(converted_text, target_file)
    end

end
