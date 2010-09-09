require 'rake/clean'
require_relative 'docgenerator'
require 'FileUtils'

DOT_NET_PATH = "#{ENV["SystemRoot"]}\\Microsoft.NET\\Framework\\v3.5"
NUNIT_EXE = "../ThirdParty/NUnit/bin/net-2.0/nunit-console.exe"
SOURCE_PATH = "../Source"
OUTPUT_PATH = "../Output"
DOCS_PATH = "../Docs"

ENV["config"] = "Debug" if ENV["config"].nil?
CONFIG = ENV["config"]

CLEAN.include(OUTPUT_PATH)

task :default => ["clean", "all"]
task :all => [:compile, :test, :specs]
desc "Update assembly versions, build, generate docs and create directory for packaging"
task :deploy => [:version_assemblies, :default, :generate_docs, :package]
  
desc "Build solutions using MSBuild"
task :compile do
    solutions = FileList["#{SOURCE_PATH}/**/*.sln"].exclude(/\.2010\./)
    solutions.each do |solution|
    sh "#{DOT_NET_PATH}/msbuild.exe /p:Configuration=#{CONFIG} #{solution}"
    end
end

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

desc "Generates documentation for the website"
task :generate_docs do
	generate_docs
end

desc "Updates assembly infos with build number"
task :version_assemblies => [:get_build_number] do 
	assembly_info_files = "#{SOURCE_PATH}/**/AssemblyInfo.cs"

	assembly_info_template_replacements = [
		['0.0.0.0', @@build_number]
	]
	
	Dir.glob(assembly_info_files).each do |file|
	  content = File.new(file,'r').read
	  assembly_info_template_replacements.each { |info| content.gsub!(/#{info[0]}/, info[1]) }
	  File.open(file, 'w') { |fw| fw.write(content) }
	end
end

desc "Gets build number based on git tags and commit."
task :get_build_number do
 	version_info = get_build_version
	@@build_number = "#{version_info[1]}.#{version_info[2]}.#{version_info[3]}.#{version_info[4]}"
end

def get_build_version
  /v(\d+)\.(\d+)\.(\d+)\-(\d+)/.match(`git describe --tags --long --match v*`.chomp)
end


desc "Packages up assembly and documentation"
task :package => [:compile, :version_assemblies, :generate_docs] do
	output_base_path = "#{OUTPUT_PATH}/#{CONFIG}"
	dll_path = "#{output_base_path}/NSubstitute"
	deploy_path = "#{output_base_path}/NSubstitute-#{@@build_number}"
	#mkdir deploy_path unless File.exists? deploy_path
	cp_r dll_path, deploy_path
	cp_r "#{DOCS_PATH}", "#{deploy_path}/Docs"
	mv "#{deploy_path}/Docs/README.markdown", "#{deploy_path}/README.txt"
	cp "../LICENSE", "#{deploy_path}"
end
