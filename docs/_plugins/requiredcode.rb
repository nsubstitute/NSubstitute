module Jekyll
	class RequiredCodeBlock < Liquid::Block
		def render(context)
		end
	end
end
Liquid::Template.register_tag('requiredcode', Jekyll::RequiredCodeBlock)

