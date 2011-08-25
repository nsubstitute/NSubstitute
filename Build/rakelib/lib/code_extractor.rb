
class CodeExtractor
    NewLine = "\n"
    def extract(text)
        block = nil
        lines = text.split(NewLine)
        lines.inject(Array.new) do |blocks, line|
            if is_end_block? line and not block.nil?
                blocks << block
                block = nil
            elsif is_start_block? line and block.nil?
                block = CodeBlock.new(tag(line))
            else
                block << line unless block.nil?
            end
            blocks
        end
    end

    class CodeBlock
        ClassOrInterfacePattern = /(public |private |protected )?(class |interface )\w+\s*\{/m 
        def declaration?
            @tag.start_with?("requiredcode") || code().start_with?("[Test]") || !!(code() =~ ClassOrInterfacePattern)
        end
        def code
            @lines.join("\n")
        end
        def initialize(tag)
            @lines = []
            @tag = tag
        end
        def <<(line)
            @lines << line
        end
        def to_s
            code
        end
    end

    private

    StartPattern = /\{% (examplecode csharp|requiredcode) %\}/
    EndPattern = /\{% end(examplecode|requiredcode) %\}/

    def is_start_block? line
        line =~ StartPattern
    end

    def is_end_block? line
        line =~ EndPattern
    end

    def tag line
        match = StartPattern.match(line)
        match[1].split()[0]
    end

end
