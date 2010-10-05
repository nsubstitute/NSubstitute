require_relative 'spec_helper'
testing 'code_extractor.rb'

describe "Given lines of markdown" do
    highlight_code_block = 'public void Main() {
        Console.WriteLine("Hello world");
    }'
    required_block = '//Gherkin'
    highlight_class_block = 'public class SomeClass { }'

    markdown_example = <<-EOF
        Sample markdown file.

        # A heading

        Some text. More text.
        {% highlight csharp %}
        #{highlight_code_block}
        {% endhighlight %}

        {% requiredcode %}
        #{required_block}
        {% endrequiredcode %}
        
        Some more text. Hooray.
        {% highlight csharp %}
        #{highlight_class_block}
        {% endhighlight %}
    EOF

    subject { CodeExtractor.new.extract markdown_example }

    it "should extract blocks of code from between highlight and requiredcode tags" do
        subject.map {|x| x.code}.should equal_ignoring_whitespace [highlight_code_block, required_block, highlight_class_block]
    end

    it "should identify required code, interfaces and classes as declarations" do
        subject.map {|x| x.declaration?}.should == [false, true, true]
    end
end


