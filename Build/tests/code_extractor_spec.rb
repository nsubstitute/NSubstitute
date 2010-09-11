require_relative 'test_helper'
testing 'code_extractor.rb'

describe "Given lines of markdown" do
    FirstCodeBlock = 'public void Main() {
        Console.WriteLine("Hello world");
    }'
    SecondCodeBlock = '//Gherkin'

    MarkdownExample = <<-EOF
        Sample markdown file.

        # A heading

        Some text. More text.
        {% highlight csharp %}
        #{FirstCodeBlock}
        {% endhighlight %}
        
        Some more text. Hooray.
        {% highlight csharp %}
        #{SecondCodeBlock}
        {% endhighlight %}
    EOF

    it "should extract blocks of code from between highlight tags" do
        extractor = CodeExtractor.new
        result = CodeExtractor.new.extract MarkdownExample
        result.should equal_ignoring_whitespace [FirstCodeBlock, SecondCodeBlock]
    end


end


