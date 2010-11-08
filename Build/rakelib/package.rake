require 'rubygems'
require 'rake/gempackagetask'

OUTPUT_BASE_PATH = "#{OUTPUT_PATH}/#{CONFIG}"
DLL_PATH = "#{OUTPUT_BASE_PATH}/NSubstitute"
DEPLOY_PATH = "#{OUTPUT_BASE_PATH}/NSubstitute-#{get_build_number}"

desc "Packages up assembly"
task :package_assembly => [:version_assemblies, :all, :check_examples] do

    mkdir_p DEPLOY_PATH
	cp Dir.glob("#{DLL_PATH}/*.{dll,xml}"), DEPLOY_PATH

	cp "../README.markdown", "#{DEPLOY_PATH}/README.txt"
	cp "../LICENSE.txt", "#{DEPLOY_PATH}"
	cp "../CHANGELOG.txt", "#{DEPLOY_PATH}"
	cp "../BreakingChanges.txt", "#{DEPLOY_PATH}"
	cp "../acknowledgements.markdown", "#{DEPLOY_PATH}/acknowledgements.txt"

    tidyUpTextFileFromMarkdown("#{DEPLOY_PATH}/README.txt")
    tidyUpTextFileFromMarkdown("#{DEPLOY_PATH}/acknowledgements.txt")
end

desc "Create NuPack package"
task :nupack => [:package] do
	nupack_path = "#{OUTPUT_BASE_PATH}/nupack/#{@@build_number}"
	nupack_lib_path = "#{OUTPUT_BASE_PATH}/nupack/#{@@build_number}/lib/35"

    #Ensure nupack path exists
    mkdir_p nupack_lib_path

    #Copy binaries into lib path
    cp Dir.glob("#{DLL_PATH}/*.{dll,xml}"), nupack_lib_path

    #Copy nuspec and *.txt docs into package root
    cp Dir.glob("#{DEPLOY_PATH}/*.txt"), nupack_path
    cp "NSubstitute.nuspec", nupack_path
    updateNuspec("#{nupack_path}/NSubstitute.nuspec")

    #Build package
    full_path_to_nupack_exe = File.expand_path(NUPACK_EXE, File.dirname(__FILE__))
    nuspec = File.expand_path("#{nupack_path}/NSubstitute.nuspec", File.dirname(__FILE__))
    FileUtils.cd "#{OUTPUT_BASE_PATH}/nupack" do
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

#Defines gemspec for building our gem, also copies files into the correct dir structure for said gem
gemspec = Gem::Specification.new do |spec|

    #First copy files to gem dir structure 

    nuproj_lib_path = '/lib/35'
    nuproj_docs_path = '/docs'

    #Ensure nuproj paths exist
    mkdir_p nuproj_lib_path
    mkdir_p nuproj_docs_path

    #Copy binaries into lib path
    cp Dir.glob("#{DLL_PATH}/**/*.{dll,xml}"), nuproj_lib_path

    #Copy *.txt docs into package root
    cp Dir.glob("#{DEPLOY_PATH}/*.txt"), nuproj_docs_path

    #Then define gem

    spec.platform    = Gem::Platform::RUBY
    spec.name        = 'nsubstitute'
    spec.date        = DateTime.now.rfc3339
    spec.version     = get_build_number
    spec.files       = Dir["#{nuproj_docs_path}/**/*"] + Dir["#{nuproj_lib_path}/**/*"]
    spec.summary     = 'NSubstitute is a friendly substitute for .NET mocking frameworks.'
    spec.description = <<-EOF
      It's like a stub with property behaviour.
      With nice semantics for setting return values.
      It only has one mode - loose semantics, which you can query afterwards.
      It's meant to be simple, succinct and pleasant to use.
    EOF
    spec.authors           = ['Dave Tchepak', 'Anthony Egerton']
    spec.email             = 'nsubstitute@googlegroups.com'
    spec.homepage          = 'http://nsubstitute.github.com/'
    spec.rubyforge_project = 'nsubstitute'
end

#sets up dependencies for generated package task
task :package => [:package_assembly]

desc "Creates package task that creates nuproj gem"
Rake::GemPackageTask.new(gemspec) do |pkg|
    pkg.package_dir = "#{OUTPUT_BASE_PATH}/gems"
end