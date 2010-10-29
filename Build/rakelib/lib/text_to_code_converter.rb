class TextToCodeConverter
    @@converted = 0
    def initialize(extractor)
        @extractor = extractor
    end
    def convert(text)
        @@converted += 1
        test_number = 0
        code_blocks = @extractor.extract(text)
        declarations = code_blocks \
                    .select { |x| x.declaration?} \
                    .map { |x| x.code } \
                    .join("\n")
        tests = code_blocks \
                    .select { |x| not x.declaration? } \
                    .map { |x| to_test(x, test_number += 1) } \
                    .join("\n")

        <<-EOF
using System;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

namespace NSubstitute.Documentation.Samples {
    public class Tests_#{@@converted} {
        #{declarations}

        #{tests}
    }
}
        EOF
    end

    private
    def to_test(code_block, test_number)
        "[Test] public void Test_#{test_number}() {
            #{code_block.code}
        }"
    end
end
