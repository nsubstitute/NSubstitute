require_relative 'spec_helper'
testing 'code_extractor.rb'

describe "Given lines of markdown" do
    example_code_block = 'public void Main() {
        Console.WriteLine("Hello world");
    }'
    required_block = '//Gherkin'
    example_code_class_block = 'public class SomeClass { }'

    markdown_example = <<-EOF
        Sample markdown file.

        # A heading

        Some text. More text.
        {% examplecode csharp %}
        #{example_code_block}
        {% endexamplecode %}

        {% requiredcode %}
        #{required_block}
        {% endrequiredcode %}
        
        Some more text. Hooray.
        {% examplecode csharp %}
        #{example_code_class_block}
        {% endexamplecode %}
    EOF

    subject { CodeExtractor.new.extract markdown_example }

    it "should extract blocks of code from between examplecode and requiredcode tags" do
        subject.map {|x| x.code}.should equal_ignoring_whitespace [example_code_block, required_block, example_code_class_block]
    end

    it "should identify required code, interfaces and classes as declarations" do
        subject.map {|x| x.declaration?}.should == [false, true, true]
    end
end


