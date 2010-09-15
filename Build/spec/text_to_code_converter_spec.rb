require_relative 'spec_helper'
testing 'text_to_code_converter.rb'

describe "when converting text to code" do
    text = "blahblah"
    class_declaration = "public class Foo{}"
    interface_declaration = "interface IBar {}"
    first_code_block = "some code"
    second_code_block = "more code"

    before do
        extractor = Substitute.new
        converter = TextToCodeConverter.new(extractor)
        
        extractor
            .extract(text)
            .returns([class_declaration, first_code_block, interface_declaration, second_code_block])
        @result = converter.convert(text)
    end

    it "should output all classes and interfaces from text" do
        @result.should include([class_declaration, interface_declaration].join("\n"))
    end

    it "should output non-class/interface code blocks as individual tests" do

    end

end
