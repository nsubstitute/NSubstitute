
desc "Packages up assembly"
task :package => [:version_assemblies, :all, :check_examples] do
	output_base_path = "#{OUTPUT_PATH}/#{CONFIG}"
	dll_path = "#{output_base_path}/NSubstitute"
	deploy_path = "#{output_base_path}/NSubstitute-#{@@build_number}"

    mkdir_p deploy_path
	cp Dir.glob("#{dll_path}/*.{dll,xml}"), deploy_path

	cp "../README.markdown", "#{deploy_path}/README.txt"
	cp "../LICENSE.txt", "#{deploy_path}"
	cp "../CHANGELOG.txt", "#{deploy_path}"
	cp "../acknowledgements.markdown", "#{deploy_path}/acknowledgements.txt"

    tidyUpTextFileFromMarkdown("#{deploy_path}/README.txt")
    tidyUpTextFileFromMarkdown("#{deploy_path}/acknowledgements.txt")
end

desc "Create NuPack package"
task :nupack => [:package] do
	output_base_path = "#{OUTPUT_PATH}/#{CONFIG}"
	dll_path = "#{output_base_path}/NSubstitute"
	deploy_path = "#{output_base_path}/NSubstitute-#{@@build_number}"
	nupack_path = "#{output_base_path}/nupack/#{@@build_number}"
	nupack_lib_path = "#{output_base_path}/nupack/#{@@build_number}/lib/35"

    #Ensure nupack path exists
    mkdir_p nupack_lib_path

    #Copy binaries into lib path
    cp Dir.glob("#{dll_path}/*.{dll,xml}"), nupack_lib_path

    #Copy nuspec and *.txt docs into package root
    cp Dir.glob("#{deploy_path}/*.txt"), nupack_path
    cp "NSubstitute.nuspec", nupack_path
    updateNuspec("#{nupack_path}/NSubstitute.nuspec")

    #Build package
    full_path_to_nupack_exe = File.expand_path(NUPACK_EXE, File.dirname(__FILE__))
    nuspec = File.expand_path("#{nupack_path}/NSubstitute.nuspec", File.dirname(__FILE__))
    FileUtils.cd "#{output_base_path}/nupack" do
        sh "#{full_path_to_nupack_exe} #{nuspec}"
    end
end

def updateNuspec(file)
    text = File.read(file)
    modified_date = DateTime.now.rfc3339
    text.gsub! /<version>.*?<\/version>/, "<version>#{@@build_number}</version>"
    text.gsub! /<modified>.*?<\/modified>/, "<modified>#{modified_date}</modified>"
    File.open(file, 'w') { |f| f.write(text) }
end

def tidyUpTextFileFromMarkdown(file)
    text = File.read(file)
    File.open(file, "w") { |f| f.write( stripHtmlComments(text) ) }
end

def stripHtmlComments(text)
    startComment = "<!--"
    endComment = "-->"

    indexOfStart = text.index(startComment)
    indexOfEnd = text.index(endComment)
    return text if indexOfStart.nil? or indexOfEnd.nil?

    text[indexOfStart..(indexOfEnd+endComment.length-1)] = ""
    return stripHtmlComments(text)
end
