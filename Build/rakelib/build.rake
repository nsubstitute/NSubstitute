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

desc "Build solutions using MSBuild"
task :compile do
    solutions = FileList["#{SOURCE_PATH}/**/*.sln"] 
    solutions.each do |solution|
        TARGETS.each do |target|
            sh "#{DOT_NET_PATH}/msbuild.exe /p:Configuration=#{target}-#{CONFIG} #{solution}"
        end
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
