require_relative 'spec_helper'
testing 'examples_to_code.rb'

describe ExamplesToCode, "when converting examples to code" do
    target_dir = "target/dir"
    example_dir = "source/dir"
    example_files = ["dir/file1.markdown", "dir/file2.markdown"]

    before do
        @file_finder = Substitute.new
        @file_finder.find(example_dir, "*.{markdown,html}").returns(example_files)
        @converter = Substitute.new

        examples_to_code = ExamplesToCode.new(@file_finder, @converter)

        examples_to_code.convert(example_dir, target_dir)
    end
        
    it "converts each example file to a code file" do
        @converter.received.convert(example_files[0], "#{target_dir}/file1.cs")
        @converter.received.convert(example_files[1], "#{target_dir}/file2.cs")
    end

end
