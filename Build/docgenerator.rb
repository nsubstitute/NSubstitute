
def generate_docs
	files_for_generation = FileList.new ["#{SOURCE_PATH}/Docs/*.template"]

	Dir.glob(files_for_generation).each do |original_file| 
	  new_file = DOCS_PATH + "/" + File.basename(original_file, '.template')
	  
	  cp original_file, new_file
	  content = File.new(new_file,'r').read	
	  
	  replacement_tags = content.scan(/(\{CODE:.+\})/)
	  
	  replacement_tags.each do |tag|
		replacement_content = get_replacement(tag)
		
		content.gsub!(tag.to_s, replacement_content)
	  end

	  File.open(new_file, 'w') { |fw| fw.write(content) }
	end
end

def get_replacement(tag)
	print ":: Finding replacement for :: " + tag.to_s + "\r\n"
	source_files = FileList.new ["#{SOURCE_PATH}/NSubstitute.Acceptance.Specs/**/*.cs"]

	location = nil
	source_file = nil
	Dir.glob(source_files).each do |source_file| 
		file_content = File.new(source_file,'r').read
		location = file_content.index(tag.to_s);
		
		break if (location != nil) 
	end
	
	raise "Tag " + tag.to_s + " not found in source code" if (location == nil)
	
	replacement_content = extract_code_block(source_file, location + tag.to_s.length + 4) 
	## MUST FIX LENGTH TO MOVE ONTO NEXT LINE
end

def extract_code_block(source_file, start_point)
	file_content = File.new(source_file,'r').read

	block_count = nil;
	end_point = 0;
	
	for i in start_point..file_content.length
		char = file_content[i, 1].to_s;
		
		if (char == "{")
			block_count = 0 if (block_count == nil)
			block_count += 1
		elsif (char == "}")
			raise "invalid syntax encountered - closing brace before opening brace" if (block_count == nil)
			block_count -= 1
		end
		
		if (block_count == 0)
			end_point = i
			break;
		end;
	end

	code_block = file_content[start_point..end_point]
	
	return indent_block(code_block)
end

def indent_block(code_block)
	tab_count = code_block.index(/[^\s]/)
	code_block = code_block.strip.gsub(/\n {#{tab_count}}/m, "\n")
	code_block = "    " + code_block.gsub("\n", "\n    ")	
end