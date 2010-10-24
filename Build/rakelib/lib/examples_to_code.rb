Dir["#{File.dirname(__FILE__)}/*.rb"].each { |file| require file }

class ExamplesToCode
    def self.create()
        file_reader_writer = FileReaderWriter.new
        converter = FileConverter.new(file_reader_writer, TextToCodeConverter.new(CodeExtractor.new))
        ExamplesToCode.new(FileFinder.new, converter)
    end

	def initialize(file_finder, converter)
        @file_finder = file_finder
        @converter = converter
	end

    def convert(example_dir, target_dir)
        puts "Convert from #{example_dir} to #{target_dir}"
        @file_finder.find(example_dir, "*.{markdown,html}").each do |file|
            file_name = File.basename(file, File.extname(file))
            target = "#{target_dir}/#{file_name}.cs"
            puts "Converting #{file} to #{target}"
            @converter.convert(file, target)
        end
    end
end

