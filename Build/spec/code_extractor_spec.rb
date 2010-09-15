require_relative 'spec_helper'
testing 'code_extractor.rb'

describe "Given lines of markdown" do
    first_code_block = 'public void Main() {
        Console.WriteLine("Hello world");
    }'
    second_code_block = '//Gherkin'
    third_code_block = '//Code goes here'

    markdown_example = <<-EOF
        Sample markdown file.

        # A heading

        Some text. More text.
        {% highlight csharp %}
        #{first_code_block}
        {% endhighlight %}

        {% requiredcode %}
        #{second_code_block}
        {% endrequiredcode %}
        
        Some more text. Hooray.
        {% highlight csharp %}
        #{third_code_block}
        {% endhighlight %}
    EOF

    it "should extract blocks of code from between highlight and requiredcode tags" do
        extractor = CodeExtractor.new
        result = CodeExtractor.new.extract markdown_example
        result.should equal_ignoring_whitespace [first_code_block, second_code_block, third_code_block]
    end
end


