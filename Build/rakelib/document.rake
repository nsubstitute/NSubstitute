desc "Generates documentation for the website"
task :generate_docs => [:all, :check_examples] do
    output = File.expand_path("#{OUTPUT_PATH}/nsubstitute.github.com", File.dirname(__FILE__))
    FileUtils.cd "../Source/Docs" do
        sh "jekyll \"#{output}\""
    end
end

desc "Compile and test code from examples from last NSubstitute build"
task :check_examples => [:generate_code_examples, :compile_code_examples, :test_examples]

task :generate_code_examples do
    examples_to_code = ExamplesToCode.create
    examples_to_code.convert("../Source/Docs/", "#{OUTPUT_PATH}/CodeFromDocs")
    examples_to_code.convert("../Source/Docs/help/_posts/", "#{OUTPUT_PATH}/CodeFromDocs")
    examples_to_code.convert("../", "#{OUTPUT_PATH}/CodeFromDocs")
end

task :compile_code_examples => [:generate_code_examples] do
    #Copy references to documentation directory
	output_base_path = "#{OUTPUT_PATH}/#{CONFIG}"
    output_doc_path = "#{OUTPUT_PATH}/CodeFromDocs"
    references = %w(NSubstitute.dll nunit.framework.dll).map do |x| 
        "#{output_base_path}/NSubstitute.Specs/#{x}"
    end
    FileUtils.cp references, output_doc_path

    #Compile
    FileUtils.cd output_doc_path do
        sh "#{DOT_NET_PATH}/csc.exe /target:library /out:\"NSubstitute.Samples.dll\" /reference:\"nunit.framework.dll\" /reference:\"NSubstitute.dll\" *.cs"
    end
end
