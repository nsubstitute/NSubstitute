require_relative 'test_helper'
testing 'code_extractor.rb'

describe "Given lines of markdown" do
    FirstCodeBlock = 'public void Main() {
        Console.WriteLine("Hello world");
    }'
    SecondCodeBlock = '//Gherkin'
    ThirdCodeBlock = '//Code goes here'

    MarkdownExample = <<-EOF
        Sample markdown file.

        # A heading

        Some text. More text.
        {% highlight csharp %}
        #{FirstCodeBlock}
        {% endhighlight %}

        {% requiredcode %}
        #{SecondCodeBlock}
        {% endrequiredcode %}
        
        Some more text. Hooray.
        {% highlight csharp %}
        #{ThirdCodeBlock}
        {% endhighlight %}
    EOF

    it "should extract blocks of code from between highlight and requiredcode tags" do
        extractor = CodeExtractor.new
        result = CodeExtractor.new.extract MarkdownExample
        result.should equal_ignoring_whitespace [FirstCodeBlock, SecondCodeBlock, ThirdCodeBlock]
    end


end


