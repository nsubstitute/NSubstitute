desc "Runs tests with NUnit"
task :test => [:compile] do
    tests = FileList["#{OUTPUT_PATH}/**/NSubstitute.Specs.dll"].exclude(/obj\//)
    sh "#{NUNIT_EXE} #{tests} /nologo /exclude=Pending /xml=#{OUTPUT_PATH}/UnitTestResults.xml"
end

desc "Run acceptance specs with NUnit"
task :specs => [:compile] do
    tests = FileList["#{OUTPUT_PATH}/**/NSubstitute.Acceptance.Specs.dll"].exclude(/obj\//)
    sh "#{NUNIT_EXE} #{tests} /nologo /exclude=Pending /xml=#{OUTPUT_PATH}/SpecResults.xml"
end

desc "Runs pending acceptance specs with NUnit"
task :pending => [:compile] do
    acceptance_tests = FileList["#{OUTPUT_PATH}/**/NSubstitute.Acceptance.Specs.dll"].exclude(/obj\//)
    sh "#{NUNIT_EXE} #{acceptance_tests} /nologo /include=Pending /xml=#{OUTPUT_PATH}/PendingSpecResults.xml"
end

task :test_examples => [:compile_code_examples] do
    tests = FileList["#{OUTPUT_PATH}/**/NSubstitute.Samples.dll"].exclude(/obj\//)
    sh "#{NUNIT_EXE} #{tests} /nologo /exclude=Pending /xml=#{OUTPUT_PATH}/DocResults.xml"
end
