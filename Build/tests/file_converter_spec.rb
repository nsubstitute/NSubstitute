require_relative 'test_helper'
testing 'file_converter.rb'

describe "When converting one file to another" do
    SourceFile = "test1.markdown"
    TargetFile = "test1.cs"
    SourceText = "this is some text"
    ConvertedText = "text some is this"

    before do
        @file_reader_writer = Substitute.new
        @converter = Substitute.new

        @file_reader_writer.read(SourceFile).returns(SourceText)
        @converter.convert(SourceText).returns(ConvertedText)

        file_converter = FileConverter.new(@file_reader_writer, @converter)
        file_converter.convert SourceFile, TargetFile
    end

    it "should save converted text to target file" do
        @file_reader_writer.received.write(ConvertedText, TargetFile)
    end
end


