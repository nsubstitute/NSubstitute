desc "Runs tests with NUnit"
task :test => [:compile] do
    TARGETS.each do |target|
        tests = FileList["#{OUTPUT_PATH}/#{CONFIG}/#{target}/**/NSubstitute.Specs.dll"].exclude(/obj\//)
        sh "#{NUNIT_EXE} #{tests} /nologo /framework=net-4.0 /exclude=Pending /xml=#{OUTPUT_PATH}/#{CONFIG}/#{target}/UnitTestResults.xml"
    end
end

desc "Run acceptance specs with NUnit"
task :specs => [:compile] do
    TARGETS.each do |target|
        tests = FileList["#{OUTPUT_PATH}/#{CONFIG}/#{target}/**/NSubstitute.Acceptance.Specs.dll"].exclude(/obj\//)
        sh "#{NUNIT_EXE} #{tests} /nologo /framework=net-4.0 /exclude=Pending /xml=#{OUTPUT_PATH}/#{CONFIG}/#{target}/SpecResults.xml"
    end
end

desc "Runs pending acceptance specs with NUnit"
task :pending => [:compile] do
    TARGETS.each do |target|
        tests = FileList["#{OUTPUT_PATH}/#{CONFIG}/#{target}/**/NSubstitute.Acceptance.Specs.dll"].exclude(/obj\//)
        sh "#{NUNIT_EXE} #{acceptance_tests} /framework=net-4.0 /nologo /include=Pending /xml=#{OUTPUT_PATH}/#{CONFIG}/#{target}/PendingSpecResults.xml"
    end
end

task :test_examples => [:compile_code_examples] do
    tests = FileList["#{OUTPUT_PATH}/**/NSubstitute.Samples.dll"].exclude(/obj\//)
    sh "#{NUNIT_EXE} #{tests} /nologo /framework=net-4.0 /exclude=Pending /xml=#{OUTPUT_PATH}/DocResults.xml"
end
