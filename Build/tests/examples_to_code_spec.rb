require_relative 'test_helper'
testing 'examples_to_code.rb'

describe ExamplesToCode, "when converting examples to code" do
    TargetDir = "target/dir"
    ExampleDir = "source/dir"
    ExampleFiles = ["dir/file1.markdown", "dir/file2.markdown"]

    before do
        @file_finder = Substitute.new
        @file_finder.find(ExampleDir, "**/*.markdown").returns(ExampleFiles)
        @converter = Substitute.new

        examples_to_code = ExamplesToCode.new(@file_finder, @converter)

        examples_to_code.convert(ExampleDir, TargetDir)
    end
        
    it "converts each example file to a code file" do
        @converter.received.convert(ExampleFiles[0], "#{TargetDir}/file1.cs")
        @converter.received.convert(ExampleFiles[1], "#{TargetDir}/file2.cs")
    end

end
