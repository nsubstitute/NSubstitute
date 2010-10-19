task :default => ["clean", "all"]
task :all => [:compile, :test, :specs]

desc "Update assembly versions, build, generate docs and create directory for packaging"
task :deploy => [:version_assemblies, :default, :generate_docs, :package, :nupack]
