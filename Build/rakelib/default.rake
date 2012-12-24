task :default => ["clean", "all"]
task :all => [:compile, :test, :specs]

desc "Update assembly versions, build, generate docs and create directory for packaging"
task :deploy => [:version_assemblies, :default, :package, :nuget, :zip, :zip_docs, :zip_website]

task :automated_deploy => [:checkBundle, :get_build_number] do
	puts "##teamcity[buildNumber '#{@@build_number}']"
	
	puts "Building version v" + @@build_number

	# Hack for TeamCity's Git module which explicitly fetches --no-tags and screws our versioning scheme
	sh "ruby --version"
	sh "git.exe --version"
	sh "git.exe fetch --tags"

	Rake::Task["deploy"].invoke
end

task :checkBundle do
	sh "bundle install"
end

