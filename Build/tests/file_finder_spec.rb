require_relative 'test_helper'
testing 'file_finder.rb'

describe FileFinder, "when finding markdown files" do

    it "finds the two sample markdown files in the current directory" do
        script_dir = File.dirname(__FILE__)
        samples_dir = File.join(script_dir, "sample_files")

        finder = FileFinder.new
        results = finder.find(script_dir, "**/*.markdown")

        results.should == ["#{samples_dir}/first.markdown", "#{samples_dir}/second.markdown"]
    end

end
