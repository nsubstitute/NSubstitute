require_relative 'spec_helper'
testing 'file_converter.rb'

describe "When converting one file to another" do
    source_file = "test1.markdown"
    target_file = "test1.cs"
    source_text = "this is some text"
    converted_text = "text some is this"

    before do
        @file_reader_writer = Substitute.new
        @converter = Substitute.new

        @file_reader_writer.read(source_file).returns(source_text)
        @converter.convert(source_text).returns(converted_text)

        file_converter = FileConverter.new(@file_reader_writer, @converter)
        file_converter.convert source_file, target_file
    end

    it "should save converted text to target file" do
        @file_reader_writer.received.write(converted_text, target_file)
    end
end


