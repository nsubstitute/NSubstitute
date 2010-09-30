require_relative 'spec_helper'
testing 'file_finder.rb'

describe FileFinder, "when finding files" do

    it "finds the two sample markdown files and one sample html file" do
        script_dir = File.dirname(__FILE__)
        samples_dir = File.join(script_dir, "sample_files")

        finder = FileFinder.new
        results = finder.find(script_dir, "**/*.{markdown,html}")

        results.should == ["first.markdown", "second.markdown", "first.html"].map {|x| "#{samples_dir}/#{x}"}
    end

end

