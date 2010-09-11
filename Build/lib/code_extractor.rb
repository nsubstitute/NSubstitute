class CodeExtractor
    NewLine = "\n"
    def extract(text)
        block = nil
        lines = text.split(NewLine)
        lines.inject(Array.new) do |blocks, line|
            if is_end_block? line and not block.nil?
                blocks << block.join(NewLine)
                block = nil
            elsif is_start_block? line and block.nil?
                block = []
            else
                block << line unless block.nil?
            end
            blocks
        end
    end

    private

    def is_start_block? line
        line =~ /\{% highlight csharp %\}/
    end

    def is_end_block? line
        line =~ /\{% endhighlight %\}/
    end

end
