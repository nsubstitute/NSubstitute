class ExamplesToCode
    
	def initialize(file_finder, converter)
        @file_finder = file_finder
        @converter = converter
	end

    def convert(example_dir, target_dir)
        @file_finder.find(example_dir, "**/*.markdown").each do |file|
            file_name = File.basename(file, File.extname(file))
            @converter.convert(file, "#{target_dir}/#{file_name}.cs")
        end
    end
end

def generate_docs
	files_for_generation = FileList.new ["#{SOURCE_PATH}/Docs/*.template"]

	Dir.glob(files_for_generation).each do |original_file| 
	  new_file = DOCS_PATH + "/" + File.basename(original_file, '.template')
	  is_html = new_file =~ /\.html$/

	  cp original_file, new_file
	  content = File.new(new_file,'r').read	
	  
	  replacement_tags = content.scan(/\{CODE:.+\}/)
	  
	  replacement_tags.each do |tag|
		replacement_content = get_replacement(tag)
		replacement_content = to_html(replacement_content) if (is_html)
		content.gsub!(tag.to_s, replacement_content)
	  end

	  File.open(new_file, 'w') { |fw| fw.write(content) }
	end
end
