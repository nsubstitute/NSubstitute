
desc "Packages up assembly"
task :package => [:all, :check_examples] do
	output_base_path = "#{OUTPUT_PATH}/#{CONFIG}"
	deploy_path = "#{output_base_path}/NSubstitute-#{@@build_number}"

    mkdir_p deploy_path

    TARGETS.each do |target|
        destination = "#{deploy_path}/lib/#{target}/"
        mkdir_p destination
        cp Dir.glob("#{output_base_path}/#{target}/NSubstitute/*.{dll,xml}"), destination
    end
    
	cp "../README.markdown", "#{deploy_path}/README.txt"
	cp "../LICENSE.txt", "#{deploy_path}"
	cp "../CHANGELOG.txt", "#{deploy_path}"
	cp "../BreakingChanges.txt", "#{deploy_path}"
	cp "../acknowledgements.markdown", "#{deploy_path}/acknowledgements.txt"

    tidyUpTextFileFromMarkdown("#{deploy_path}/README.txt")
    tidyUpTextFileFromMarkdown("#{deploy_path}/acknowledgements.txt")
end

desc "Create NuGet package"
task :nuget => [:package] do
	output_base_path = "#{OUTPUT_PATH}/#{CONFIG}"
	deploy_path = "#{output_base_path}/NSubstitute-#{@@build_number}/."
	nuget_path = "#{output_base_path}/nuget/#{@@build_number}"

    mkdir_p nuget_path

    #Copy binaries into lib path
    cp_r deploy_path, nuget_path

    #Copy nuspec into package root
    cp "NSubstitute.nuspec", nuget_path
    updateNuspec("#{nuget_path}/NSubstitute.nuspec")

    #Build package
    full_path_to_nuget_exe = File.expand_path(NUGET_EXE, File.dirname(__FILE__))
    nuspec = File.expand_path("#{nuget_path}/NSubstitute.nuspec", File.dirname(__FILE__))
    FileUtils.cd "#{output_base_path}/nuget" do
        sh "\"#{full_path_to_nuget_exe}\" pack \"#{nuspec}\""
    end
end

desc "Create .ZIP package of binaries"
task :zip => [:package] do
	output_base_path = "#{OUTPUT_PATH}/#{CONFIG}"
	
	zip_path = "#{output_base_path}/zip"
	mkdir_p zip_path

	sh "\"#{ZIP_EXE}\" a -r \"#{zip_path}/NSubstitute-#{@@build_number}.zip\" \"#{output_base_path}/NSubstitute-#{@@build_number}\""
end

desc "Create .ZIP package of code from docs"
task :zip_docs => [:package] do
	sh "\"#{ZIP_EXE}\" a -r \"#{OUTPUT_PATH}/NSubstitute-CodeFromDocs-#{@@build_number}.zip\" \"#{OUTPUT_PATH}/CodeFromDocs\""
end

desc "Create .ZIP package of website"
task :zip_website => [:package] do
	sh "\"#{ZIP_EXE}\" a -r \"#{OUTPUT_PATH}/nsubstitute.github.com-#{@@build_number}.zip\" \"#{OUTPUT_PATH}/nsubstitute.github.com\""
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
