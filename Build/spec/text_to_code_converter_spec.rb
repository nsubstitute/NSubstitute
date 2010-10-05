require_relative 'spec_helper'
testing 'text_to_code_converter.rb'

describe "when converting text to code" do

    class Block
        def initialize(code, declaration)
            @code = code
            @declaration = declaration
        end
        def code; @code; end
        def declaration?; @declaration; end
    end

    text = "blahblah"
    declaration = Block.new("a declaration", true)
    code_block = Block.new("some code", false)

    before do
        extractor = Substitute.new
        converter = TextToCodeConverter.new(extractor)
        
        extractor
            .extract(text)
            .returns([code_block, declaration])
        @result = converter.convert(text)
    end

    it "should output all classes and interfaces from text" do
        @result.should include(declaration.code)
        @result.should include(code_block.code)
    end

    it "should output declarations before tests" do
        index_of_declaration = @result.index(declaration.code)
        index_of_first_test = @result.index("[Test]")
        index_of_declaration.should < index_of_first_test
    end

end
